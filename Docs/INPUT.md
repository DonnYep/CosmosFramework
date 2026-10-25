# 输入模块（薄门面 + 后端适配 + 输入闸门）

> 结论：**输入模块有意义，但它的价值必须从"兼容各种设备"转向"框架级输入治理"。**
> 设备兼容（手柄/触屏/重绑定/glyph）交给 Unity Input System 或 Rewired；
> 框架只保留**虚拟动作名、边沿判定、输入闸门（上下文屏蔽）、帮助体装配**这四件与框架其它模块耦合的事。

## 1. 调研：其它框架是怎么做的

对 GitHub 上的仓库树做全量扫描（`git/trees?recursive=1`，按 `Input` 匹配 `.cs` 文件），结论很一致：

| 框架 | 星标量级 | 输入相关 .cs | 实际内容 |
| --- | --- | --- | --- |
| [EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework) | 老牌中文框架 | **0**（366 条目） | 模块只有 Asset/Config/DataNode/…/Sound/UI/WebRequest，**没有输入模块** |
| [GameFrameX/GameFrameX](https://github.com/GameFrameX/GameFrameX) | GameFramework 的模块化续作 | **0**（2173 条目） | 同上，模块包里也没有 Input |
| [liangxiegame/QFramework](https://github.com/liangxiegame/QFramework) | 中文老牌 | 0（7 个命中全是 `InputField`/zip 流） | 没有输入模块 |
| [Alex-Rachel/TEngine](https://github.com/Alex-Rachel/TEngine) | GameFramework + YooAsset | 0（命中全是 Debugger 的输入信息窗口） | 没有输入模块 |
| [egametang/ET](https://github.com/egametang/ET) | 双端/热更框架 | 只有两个**业务级**模块 | `cn.etetet.lockstep` 的帧输入（`LSInput`/`OneFrameInputs`）、`cn.etetet.mapplay` 的 `InputSystemComponent`（包新输入系统） |

也就是说：**通用框架普遍不做输入模块**。真正提供输入层的是这三类：

| 方案 | 提供什么 | 定位 |
| --- | --- | --- |
| [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/Installation.html)（官方） | Action / Control Scheme / 设备配对 / 运行时重绑定 / 触摸 / 多玩家 | 设备层标准答案 |
| [Rewired](https://guavaman.com/projects/rewired/docs/)（商业） | Players / Actions / Controller Maps / Layouts / Control Mapper / Glyphs / Touch / User Data Store | 生产级输入中间件 |
| [InControl](https://github.com/pbhogan/InControl)（开源） | InputDevice / InputControl / 设备 Profile / XInput | 轻量跨设备抽象 |

原因不难理解：输入层里"设备 → 动作"的映射是**项目玩法定制最重**的一块（手感、连招、重绑定、本地多人、手柄 glyph、平台认证），
框架不可能收敛；而 Unity 已经提供了设备层抽象，插件又把生产特性做完了。框架硬做，只会做成一个"又一个半成品输入库"。

**但有一件事插件不会替框架做**：输入与 UI/流程的耦合——
"全屏面板打开时屏蔽游戏输入""过场动画期间屏蔽全部输入""重绑定对话框期间只允许 UI 输入"。
这需要框架级的**输入闸门（context stack）**，插件只提供 action 查询，不知道你的 UI 栈。
这正是输入模块应该保留的核心价值。

另有一个现实约束：本仓库的 6 个示例目录（`05_Input`、`17_PlayerController`、`21_Network` 的 lockstep、
`23_Entity`、`26_CustomInput`）都在用这个模块，删除会连带改一批示例。

## 2. 现有实现的问题（改之前）

| 问题 | 后果 |
| --- | --- |
| 帮助体里硬编码 `UnityEngine.Input` | 工程把 Active Input Handling 设为 **Input System Package (New)** 后，每次调用都抛 `InvalidOperationException`，而且**每帧刷屏**（官方文档明确：启用新后端会禁用旧后端） |
| 只有 7 个按钮 + 6 个轴（`InputButtonType`/`InputAxisType` 写死） | 新增一个键就要写一个 helper 类 + 注册/注销样板（`26_CustomInput` 里整整 54 行就是为了 5 个键） |
| 边沿判定 `lastPressedFrame - frameCount == -1` | `GetButtonDown/Up` **永远晚一帧**；同一帧的按下+抬起还会同时为真 |
| 未注册的名字查询时**静默自动注册** | 按键名拼错永远读 false，不报错，排查成本极高 |
| 没有输入闸门，只有 `IsEnableInputDevice` 全局开关 | UI 打开只能整体关输入；关闭时不清状态，按住不放的键会**卡在按下** |
| 没有手柄/多设备/本地多人/重绑定/glyph | 这些只能靠插件 |
| 移动端只是把 1 指当左键、2 指当右键 | 无拖动增量、无双指手势；`SetButtonUp(_MouseRight)` 每帧都在调（误触发抬起边沿） |
| helper 在字段初始化时抓 `CosmosEntry.InputManager` | 构造早于模块启动就永久拿到 null |
| `_UpperLower` 这种项目专用轴混在框架里 | 框架被单个项目的需求污染 |

## 3. 三个可选方案

| 方案 | 做法 | 适合 | 代价 |
| --- | --- | --- | --- |
| **A. 删掉模块** | 业务直接用 Input System / Rewired；`CosmosEntry.InputManager` 移除 | 已经确定用插件的团队 | 6 个示例要改；框架失去统一输入入口与闸门 |
| **B. 薄门面 + 后端适配**（本轮已实施） | 保留虚拟动作名/边沿/闸门；后端全部外置为 `IInputHelper`；旧 Input 后端做可用性兜底 | 大多数项目；可平滑接插件 | 需要为"新输入系统/插件"各写一个适配器（约 100 行） |
| **C. 自研完整输入层** | Action/Control Scheme/重绑定/glyph/多设备全自己做 | 不建议 | 等于重写 Rewired，维护成本极高 |

推荐 **B**，并在需要时把适配器做成可选程序集（`Cosmos.Input.InputSystem`、`Cosmos.Input.Rewired`），
框架本体永远不依赖具体输入包。

## 4. 本轮已实施（方案 B 的地基）

| 改动 | 说明 |
| --- | --- |
| `IInputHelper.OnInitialization(IInputManager inputManager)` | 管理器由框架注入，不再从 `CosmosEntry` 抓静态引用；切换帮助体时自动注销旧的 |
| 电平式 `SetButtonPressed(name, pressed)` | 帮助体只推"是否按住"，边沿由模块推导：**同帧生效**、重复推送不重复触发 |
| `InputVirtualButton` 重写 | `GetButtonDown/Up` 只在该帧为真；未按下时不会产生抬起边沿 |
| **输入闸门** | `PushInputBlock(owner)` / `PopInputBlock(owner)` / `ClearInputBlocks()` / `IsInputBlocked`：可嵌套、同 owner 幂等；屏蔽期间查询返回中性值，但帮助体**继续刷新原始状态**（解除后按住的键仍然按住） |
| `IsEnableInputDevice=false` 清状态 | 关闭输入后不会残留"卡住的按键" |
| 未注册名称报一次错 | 返回中性值 + 一次 `LogError`，拼写错误立刻暴露 |
| `LegacyInputSafe` | 统一拦截：工程只启用新输入系统时**只报一次明确错误**（附修复建议）；Input Manager 里缺失的轴也只报一次并返回 0 |
| `LegacyBindingInputHelper` + `LegacyInputBinding` | 用**绑定表**取代"一个键一段 if"：示例从 54 行降到 15 行 |
| `MonoGameManager` 执行顺序 `-20000` | 模块 tick 先于业务脚本，业务在 `Update` 里就能读到**本帧**按下（不再是"下一帧才响应"） |
| 移动端帮助体修正 | 触摸改为电平推送；单指拖动映射为归一化的鼠标增量；不再每帧调 `SetButtonUp` |

### 4.1 用法

```csharp
var input = CosmosEntry.InputManager;

// 查询（与后端无关）
if (input.GetButtonDown("Jump")) { }
float move = input.GetAxis("MoveX");

// 输入闸门：全屏 UI 打开时屏蔽游戏输入
input.PushInputBlock(panel);      // 同一个 owner 重复 push 只算一次
...
input.PopInputBlock(panel);

// 自定义绑定：加一行数据即可
input.SetInputHelper(new LegacyBindingInputHelper(
    LegacyInputBinding.Button("Jump", KeyCode.Space),
    LegacyInputBinding.Button("Dash", KeyCode.LeftShift),
    LegacyInputBinding.Axis("MoveX", "Horizontal"),
    LegacyInputBinding.Axis("LookY", "Mouse Y", -1f)));
```

### 4.2 迁移注意

| 旧 | 新 |
| --- | --- |
| `void OnInitialization()` | `void OnInitialization(IInputManager inputManager)` |
| 在 `OnRefresh` 里 `SetButtonDown/Up`（边沿） | 推荐 `SetButtonPressed` 推电平（`SetButtonDown/Up` 仍保留给合成输入/虚拟摇杆） |
| 字段里 `IInputManager inputManager = CosmosEntry.InputManager;` | 用 `OnInitialization` 的参数 |
| `IsEnableInputDevice=false` 仅停止刷新 | 同时清空所有状态 |
| 查询未注册名称时自动注册 | 报错 + 中性值（需要预先 `RegisterVirtualButton/Axis`） |

## 5. 行为验证

仓库内没有该模块的单测工程。本轮验证是 **9 用例 / 53 断言的临时工程**：
把 `InputManager`/`InputVirtualDevice`/`InputVirtualButton`/`LegacyBindingInputHelper` 的**真实源码**
编进控制台程序，配可编程的 `UnityEngine.Input` 替身（可模拟按键、缺失轴、帧推进）。
位置：`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\input\harness`（仓库外，可删）。

```powershell
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\input\harness\InputHarness.csproj
# 断言：53 通过 / 0 失败  === ALL-PASS ===
```

并用**变异测试**验证用例有效：重新注入 6 个旧缺陷（边沿晚一帧、电平不去重、无按下也抬起、
闸门失效、闸门不去重、关闭不清状态）**全部被检出**。

## 6. 还需要做的事（按优先级）

1. **`Cosmos.Input.InputSystem` 适配器**（可选程序集）：`#if ENABLE_INPUT_SYSTEM`，把 `InputActionAsset` 的
   `performed/canceled` 桥接成 `SetButtonPressed`，轴桥接成 `SetAxis`。这样"新输入系统"不再是坑。
2. **`Cosmos.Input.Rewired` 桥**：把 `Player.GetButtonDown(actionId)` 桥接成同名虚拟按键（约 60 行）。
3. **输入闸门接进 UI 模块**：全屏面板 `OnOpen` 自动 `PushInputBlock(panel)`、`OnClose` 自动 `Pop`，
   业务不再手写屏蔽逻辑（这是"框架级输入"最值钱的一步）。
4. **重绑定/持久化**：框架只负责"动作名 ↔ 设备元素"的序列化（落到 `Setting` 模块），
   采集与冲突检测交给插件或适配器。
5. **手柄 glyph / 多玩家**：明确不做，文档里指向 Rewired。
