# 实体模块（Entity）

> 适用范围：`Assets/CosmosFramework/Runtime/Modules/Entity`
> 相关示例：`Assets/Examples/ExampleScripts/23_Entity`

## 1. 模块定位

Entity 是**框架内唯一负责「对象存活与刷新」的模块**，把「生命周期 + 分组 + 逐帧刷新」收敛在一处：

| 职责 | 说明 |
| ---- | ---- |
| 注册 | 预制体注册（异步加载）或纯逻辑注册（零资源） |
| 生成 / 隐藏 / 回收 | 实例池化复用；隐藏即回池，注销才真正释放 |
| 分组 | 注册级分组，支持按组批量生成 / 隐藏 / 冻结 / 解冻 |
| 刷新 | **集中派发** Update / FixedUpdate / LateUpdate，按需登记、无反射 |

## 2. 两种载体

| | `EntityObject` | `EntityLogic` |
| --- | --- | --- |
| 是否持有 GameObject | 是（`MonoBehaviour`） | 否（纯 C# 类） |
| 典型用途 | 模型、特效、挂点、需要 Transform 的表现 | 逻辑帧、帧同步、服务器算、纯数据/AI |
| 注册方式 | `RegisterEntityAsync<T>(EntityAssetInfo)` | `RegisterLogicEntity<T>(entityName, groupName)` |
| 生成方式 | `ShowEntity<T>(entityName)` | 同左 |
| 能否按组冻结 | 能 | 能 |
| 能否被刷新 | 能（实现刷新接口） | 能（实现刷新接口） |

两者共用同一套注册 / 生成 / 隐藏 / 回收 / 冻结流程，业务侧不需要区分两条链路。

## 3. 刷新契约

实例**实现哪个接口，就进哪个刷新列表**；不实现则不产生任何轮询开销（既不是空方法调用，也不是类型判断）：

```csharp
public class MyLogic : EntityLogic, IEntityTickable
{
    public void OnEntityTick(float deltaTime) { /* 每帧 */ }
}

public class MyMonster : EntityObject, IEntityTickable, IEntityLateTickable
{
    public void OnEntityTick(float deltaTime) { /* 每帧：AI / 状态机 */ }
    public void OnEntityLateTick(float deltaTime) { /* 帧末：跟随 / 相机 */ }
}
```

固定帧用 `IEntityFixedTickable`。

> 仓库内示范：`23_Entity` 的 `BulletEntity` 实现 `IEntityTickable` 推进子弹；
> 被隐藏或所在组被冻结时会自动停止推进。

### 刷新链（与旧 Controller 对比）

```
MonoGameManager.Update      // [DefaultExecutionOrder(-20000)]
 └→ GameManager.OnRefresh
     └→ module.Update        // 模块多播
         └→ EntityManager.OnUpdate
             └→ Entity.Refresh
                 └→ IEntityTickable.OnEntityTick   ← 一次接口调用，0 反射
```

创建实例只做 `is` 判定，每帧只派发"真正实现了该接口"的实例；未实现的实体不进入刷新列表。

### 顺序

派发顺序 = **实体注册顺序 → 实例生成顺序**。本帧要派发的实例数在派发开始时确定，
刷新中新生成的实例从下一帧开始参与刷新。

隐藏采用「末位换位删除」保证 O(1)，因此一次隐藏后剩余实例的相对顺序会变化
（同一操作序列下仍然可复现，例如同一份逻辑在各端重放）。

### 刷新中隐藏实体

允许在 `OnEntityTick` 中隐藏自身或其他实例：

- 隐藏立即从存活表移除（`HasEntityObject` / `GetEntityObject` 立刻反映），
  但**摘除刷新列表与入池延迟到本次刷新结束**，因此不会打乱正在遍历的列表；
- 已被隐藏的实例本帧后续不再被派发（待回收 id 集合过滤），不会出现"隐藏后还跑一帧"；
- 刷新过程中**不要注册 / 注销实体**（会改动外层遍历列表），需要时放到帧末或下一帧。

## 4. 暂停模型

暂停分两层，**互相独立、结果取并集**：

| | 设置方式 | 粒度 |
| --- | --- | --- |
| 个体暂停 | `entityObject.Paused = true` | 单个实例 |
| 组冻结 | `PauseEntityGroup(groupName)` / `UnPauseEntityGroup(groupName)` | 组内所有实例 |

- 组冻结期间**新生成的实例自动继承冻结状态**（`ShowEntity` 时按组名同步）；
- 解冻组**不会**把个体主动暂停的实例一起恢复；
- 实例回收进池时两种状态都会清零，避免复用后继承上一次的暂停状态；
- `Time.timeScale` 只能全停，"UI 动、场景冻结"这类需求由组冻结承担。

## 5. 生命周期时序

```
RegisterEntityAsync<T> / RegisterLogicEntity<T>   // 注册（每个实体一次）
  └─ ShowEntity<T>      : 池中取或新建 → OnInit(首次) → OnShow → 进入刷新列表
       └─ HideEntityObject / HideEntities / HideEntityGroup
                          : 移出刷新列表 → OnHide → OnRecycle → 回到池
  └─ DeregisterEntity   : 存活实例与池内实例一并释放（GameObject 载体销毁 GameObject）
```

- `OnInit` 只在**生成新实例**时调用一次，池中复用不会重复调用；
- `OnShow` / `OnHide` / `OnRecycle` 每个显示周期各一次；
- 模块终止（`OnTermination`）会释放全部实体与组，并清空对象池。

## 6. API 速查

| 目的 | API |
| --- | --- |
| 注册预制体实体 | `RegisterEntityAsync<T>(EntityAssetInfo)` |
| 注册纯逻辑实体 | `RegisterLogicEntity<T>(entityName, groupName = "")` |
| 生成 | `T ShowEntity<T>(entityName)` |
| 隐藏 | `HideEntityObject(entityName, id)` / `HideEntityObject(IEntityObject)` / `HideEntities(entityName)` |
| 按组生成 / 隐藏 | `ShowGroupEntities` / `HideGroupEntities` |
| 冻结 / 解冻 | `PauseEntityGroup` / `UnPauseEntityGroup` / `IsEntityGroupPaused` |
| 查询 | `HasEntity` / `HasEntityObject` / `GetEntityObject` / `GetEntityInfo` / `GetEntityGroupInfo` |
| 统计 | `EntityCount` / `EntityGroupCount` / `EntityObjectCount` |

`ShowEntity<T>` 在实体未注册时**抛异常**而不是静默返回 null —— 字符串写错应当立即暴露。

## 7. 从旧 Controller 模块迁移

## 7. 破坏性变更

Controller 模块已删除，实体刷新统一由 Entity 模块负责；实例不提供按名字查找：

- 参与刷新：实现 `IEntityTickable` / `IEntityFixedTickable` / `IEntityLateTickable`；
- 个体暂停：`IEntityObject.Paused`；组冻结：`PauseEntityGroup` / `UnPauseEntityGroup`；
- 隐藏与释放：`HideEntityObject(...)`（回池）、`DeregisterEntity(...)`（注销）；
- 取实例：持有强引用，或用事件 / 依赖注入。

完整清单见 [README 的破坏性更新](../README.md#破坏性更新)。
