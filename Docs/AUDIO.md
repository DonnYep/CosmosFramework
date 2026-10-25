# 音频模块（显式状态机 + 每帧推进）

> 重构后，音频模块**不再用协程驱动播放状态**：播放 / 暂停 / 恢复 / 停止都是同步命令，
> 淡入淡出由管理器每帧推进，推进完成的**同一帧**内完成状态迁移与播放器回收。

## 1. 为什么不用协程

旧实现的每个播放实体持有一个 `Coroutine`，任何命令都是"停掉旧协程 + 起一条新协程"。
于是**状态机隐含在协程的执行位置里**——协程被 `StopCoroutine` 打断后，状态信息随之丢失。
下面每一条都是这条设计带来的实际缺陷：

| 现象 | 根因 |
| --- | --- |
| 停止 / 暂停后状态仍报"播放中"，实体永不回收 | 过渡时长为 0 时协程在写状态**之前**就 `yield break`（而默认 `FadeInSeconds` 恰好就是 0） |
| 非循环音效播完不回收，`AudioSource` 无限增长 | 同上：`EnumPlay` 在 `fade<=0` 时直接结束，永远走不到 `!isPlaying` 检测 |
| 静音开关完全无效 | `AudioGroup.Mute` 把**旧值**写回实体（`mute = value` 写在循环之后） |
| 暂停游戏 / 断点后过渡卡住，声音停不掉 | 用 `Time.deltaTime` 推进，`timeScale=0` 时步长为 0，循环永不退出 |
| 暂停→停止→恢复后音量丢失甚至爆音 | 每条协程各自记 `startVolume`，互相打断后振幅信息丢失，且恢复时按"参数音量"覆盖 |
| 恢复一个已停止的声音会变成"状态在播、实际无声"的僵尸 | 命令没有状态校验 |
| 场景卸载后抛 `MissingReferenceException`，音效变僵尸 | 协程 `WaitUntil(() => !AudioSource.isPlaying)` 等待的播放器已被销毁 |
| 播放结束后必须等下一次播放才清理 | 回收只在 `PlayAudio` 里"顺便"做 |

除此之外，协程还有三个结构性问题：`StopCoroutine` 不保证清理（没有 `finally` 语义）、
每个操作至少分配一次迭代器状态机 + 闭包 + `WaitUntil`、以及**状态变更被推迟到下一帧**
（协程体要到下一次调度才执行），导致 `HasAudio` / `GetAudioPlayInfo` 与真实状态不一致。

## 2. 新结构

| 层 | 职责 |
| --- | --- |
| `AudioManager` | 每帧 `Tick` 所有声音组；资源注册 / 注销；组管理；`unscaledTime` 时钟 |
| `AudioGroup` | 容器 + 混音总线（组音量 / 静音 / 并发上限）+ 每帧推进 + **同帧回收** |
| `AudioPlayEntity` | 单个声音的**显式状态机** + 音量包络 + 一次性播放器绑定 |
| `AudioSourcePool` | `AudioSource` 对象池（常驻 `DontDestroyOnLoad` 场景，归还时脱离原父物体） |

```text
MonoGameManager.Update
        │
        ▼
GameManager.TickRefreshHandler ──► AudioManager.OnUpdate
                                     │  delta = Time.unscaledTime 差值（钳制 0.1s）
                                     ▼
                              foreach AudioGroup
                                     │
                                     ├─► entity.Tick(delta, groupVolume)   推进包络 / 判定播完
                                     │       └─ 返回 true ⇒ 就地压缩列表 + ReferencePool.Release
                                     └─► （同帧内完成回收，无需等下一次播放）
```

## 3. 状态机

```text
                    ┌──────────── Play / Replay ────────────┐
                    ▼                                       │
   None ────────► Playing ──Pause──► Pausing ──► Paused     │
                    ▲                  │           │        │
                    │                  └──Resume───┤        │
                    └──── Resuming ◄───────────────┘        │
   Playing / Resuming / Pausing / Paused ──Stop──► Stopping ─┴─► Stopped
   Playing（非循环且已播完）──────────────────────────────────► Stopped
   Stopped ──（同一帧内）──► 被组回收
```

| 阶段 | 含义 | 可接受的命令 |
| --- | --- | --- |
| `Playing` | 播放中（可能正在淡入） | Pause / Stop / Replay |
| `Pausing` | 暂停过渡中 | Resume（打断，从当前音量回升）/ Stop |
| `Paused` | 已暂停 | Resume / Stop / Replay |
| `Resuming` | 恢复过渡中 | Pause / Stop / Replay |
| `Stopping` | 停止过渡中 | Stop（按新时长重新计时）/ Replay（打断停止） |
| `Stopped` | 已停止，等待回收 | — |

四条不变式：

1. **命令同步生效**：状态在调用返回前已经改写（过渡时长 0 时连 `Pause()` / `Stop()` 都已调用）。
2. **幂等**：暂停中的暂停、暂停完成的再暂停、播放中的恢复，都是空操作（不会压低音量再拉起）。
3. **打断不丢信息**：后到的命令总是从**当前包络值**继续（last-command-wins），不存在两条过渡互相覆盖。
4. **终止即回收**：进入 `Stopped` 的那一帧被组回收，`HasAudio` 随即返回 `false`。

## 4. 音量模型

```text
最终写入 AudioSource.volume = 参数音量(AudioParams.Volume) × 组音量(IAudioGroup.Volume) × 包络(0~1)
```

- 淡入淡出**只改包络因子**，因此过渡进行中 `SetAudioParams` 改音量、或改组音量，都不会跳变。
- 静音用 `AudioSource.mute`，不占用音量位，静音/取消静音不改变当前振幅。
- 写回带相等性判断：目标音量没变就不写 `volume`（空闲时没有多余的原生属性调用）。

## 5. 时间模型

- 推进时间取 `Time.unscaledTime` 的差值，**不受 `timeScale` 影响**：暂停菜单把 `timeScale` 设为 0 时，
  淡出仍然会走完、声音仍然停得掉。
- 单帧推进量上限 `AudioConstant.MAX_TICK_DELTA_TIME = 0.1s`：断点 / 卡顿 / 长时间挂起后，
  过渡不会在一帧内"跳完"。
- 自然播完判定用 `AudioSource.isPlaying`，但**排除 `AudioListener.pause`**：
  全局音频暂停时所有 `isPlaying` 会变成 `false`，不排除会把正在播的音效全部误回收。
- 播放器在 `Play` 之后的**第一帧不做播完判定**，避免流式加载时 `isPlaying` 尚未生效导致误回收。

## 6. 声音组能力

| 成员 | 说明 |
| --- | --- |
| `Volume` | 组音量 0~1，与参数音量相乘；过渡中的音效平滑跟随 |
| `Mute` | 组静音，写 `AudioSource.mute`；**新建的组会继承管理器当前的静音状态** |
| `MaxConcurrentAudios` | 组内并发上限，0 = 不限制 |
| `PauseAudios / ResumeAudios / StopAudios (float)` | 整组批处理 |
| `AudioPlayEntityCount` | 组内实体数（含过渡中与暂停中的） |

并发上限的抢占规则（与 Unity 自身"满员丢低优先级"一致）：

1. 取组内 `priority` 数值最大（即优先级最低）的实体作为牺牲者，同优先级时保留先来的；
2. 新音效的优先级**不低于**牺牲者（数值更小或相等）→ 立即停止牺牲者并播放新音效；
3. 否则本次播放被放弃，`PlayAudio` 返回 `0`；
4. 牺牲者会被立刻回收，播放器直接归还对象池。

## 7. API 速查

```csharp
var audio = CosmosEntry.AudioManager;

// 播放（返回 0 表示：资源未注册 / 组不存在 / 并发上限已满且优先级不足）
AudioParams p = AudioParams.Default;
p.Volume = 0.8f;
p.Loop = true;
p.PlayTime = 0f;          // 从第几秒开始播（本次重构后生效）
p.FadeInSeconds = 1f;     // 淡入 1 秒
int serialId = audio.PlayAudio("BGM_Main", "Music", p, AudioPositionParams.Default);

// 暂停 / 恢复 / 停止 / 重播（过渡时长逐次传入）
audio.PauseAudio(serialId, 0.5f);
audio.ResumeAudio(serialId, 0.5f);
audio.StopAudio(serialId, 1f);
audio.ReplayAudio(serialId, 0f);

// 状态与进度
if (audio.GetAudioPlayInfo(serialId, out var info))
    Utility.Debug.LogInfo($"{info.AudioPlayStatusType} vol={info.Volume:0.00} t={info.Time:0.00}");

// 总线
var music = audio.AddAudioGroup("Music");
music.Volume = 0.6f;
music.MaxConcurrentAudios = 8;
music.StopAudios(1f);      // 整组淡出停止（换 BGM 时"中断播放"用它）
```

## 8. 相对旧版的语义变化（迁移必看）

| 变更点 | 旧 | 新 |
| --- | --- | --- |
| `PlayAudio` 返回 0 | 资源未注册 / 组不存在 | 另加"并发上限已满且优先级不足" |
| `HasAudio` | 停止后也常常返回 `true`（等下次播放才清理） | 停止完成的那一帧即返回 `false` |
| `ResumeAudio`（播放中） | 从 0 淡入，制造音量下陷 | 幂等空操作 |
| `PauseAudio` / `StopAudio`（过渡 0） | 状态不更新，实体变僵尸 | 当帧完成状态迁移，下一帧回收 |
| `AudioParams.PlayTime` | 被完全忽略 | 作为播放起点生效（`Replay` 同样从头定位） |
| `DeregisterAudioAsset` | 只注销不卸载 → 资源引用泄漏 | 释放使用该资源的实体后调用 `IAudioAssetHelper.UnloadAudio` |
| `RemoveAudioGroup` | 组内实体的播放器不归还 | 释放全部实体并归还播放器 |
| `AudioPlayInfo` | 4 个字段 | 新增 `Volume`（实际音量）、`Time`（播放位置） |
| `IAudioGroup` | `Mute` / 名称 / 计数 | 新增 `Volume`、`MaxConcurrentAudios`、整组 `PauseAudios` / `ResumeAudios` / `StopAudios` |
| 模块终止 | 无清理 | `OnTermination` 释放全部实体、加载任务与播放器池 |

## 9. 行为验证

仓库内没有该模块的自动化测试工程。本次重构的验证是一个 **18 用例 / 84 断言的临时工程**
（把模块真实源码编译进控制台程序，配一套可运行的 `UnityEngine` 模拟实现，逐帧驱动）：
`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\audio\harness`，**位于仓库之外，可整体删除**。

```powershell
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\audio\harness\AudioHarness.csproj
# 断言：84 通过 / 0 失败  === ALL-PASS ===
```

另外用**变异测试**验证了这些用例不是空转：把"旧实现的缺陷"重新注入源码副本，
7 个变异（终端状态不迁移、静音写旧值、不做播完判定、永不回收、恢复不校验、暂停已播完音效、
静音不继承）**全部被检出**。脚本：`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\audio\mutate.ps1`。

## 10. 边界与已知限制

- **绑定对象被销毁**（`AudioPositionParams.BindParent` 随场景卸载）：实体在下一次 `Tick` 识别出
  播放器已成"销毁引用"并回收，最多延迟 1 帧；不会抛 `MissingReferenceException`。
- **框架级暂停**：`GameManager.Pause`（`MonoGameManager.Pause`）为 `true` 时模块不会被 Tick，
  过渡会冻结——这与框架其它模块一致；`Time.timeScale = 0` 的暂停不受影响（见 §5）。
- **`AudioSourcePool` 无上限**：并发数由业务与组上限控制，池只负责复用；
  池内对象常驻 `DontDestroyOnLoad`，模块终止时统一销毁。
- **`pitch <= 0`**：会被钳到 `AudioConstant.MIN_PITCH`，否则播放位置永不推进，
  会出现"永久在播却静音"的幽灵音效。
- 同一资源多次 `PlayAudio` 会创建多个独立实体（与旧版一致），互不影响；
  需要"同一时刻只有一个"请配合组并发上限或先 `StopAudios`。
