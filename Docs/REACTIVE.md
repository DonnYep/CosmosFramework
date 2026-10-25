# 响应式基座（Reactive）

> 适用范围：`Assets/CosmosFramework/Runtime/Base/Reactive`
> 相关文档：[使用手册 §3.1 事件](MANUAL.md)、[实体](ENTITY.md)

## 1. 定位

一套极简的 reactive 基座，作为框架的事件与状态底座：

- **`Subject<T>`**：多播事件流，订阅返回 `IDisposable`；
- **`ReactiveProperty<T>`**：持有当前值，变化时推送，订阅时立即收到当前值；
- **操作符**：`Where` / `Select` / `DistinctUntilChanged`，可链式组合；
- **`CompositeDisposable`**：把一组订阅（界面、实体、模块）统一释放。

线程约定：主线程派发；派发过程中允许订阅 / 退订。

## 2. 类型

| 类型 | 说明 |
| --- | --- |
| `Observable<T>` | 可观察序列抽象基类；`Subscribe(Action<T>)` 及重载 |
| `AnonymousObservable<T>` | 用委托构造自定义流 |
| `Subject<T>` | 多播主体，兼作 `IObserver<T>`；`OnNext` / `OnError` / `OnCompleted` |
| `ReactiveProperty<T>` | 响应式属性；`Value` 赋值即推送（相同值跳过） |
| `IReadOnlyReactiveProperty<T>` | 只读视图（只暴露 `Value` 与订阅） |
| `Disposable` | `Empty` / `Create(Action)` |
| `CompositeDisposable` | 句柄集合，`Add` / `Remove` / `Clear` / `Dispose` |

## 3. 语义

| 场景 | 行为 |
| --- | --- |
| 订阅 | 返回 `IDisposable`，释放即退订 |
| 派发中退订 | 安全；当前轮已遍历到的观察者不会再被调用 |
| 派发中新订阅 | 本轮不触发，下一轮生效 |
| `Subject` 完成 / 出错后订阅 | 新订阅立即收到对应信号，不会永久等待 |
| 观察者抛异常 | 向 `OnNext` 调用方传播（与普通委托事件一致） |
| `ReactiveProperty` 相同值 | 默认不去推送，可用自定义 `IEqualityComparer<T>` 覆盖 |
| `ReactiveProperty` 订阅 | 先注册、再立即推送当前值 |

## 4. 用法

### 4.1 响应式属性

```csharp
public class PlayerModel
{
    public readonly ReactiveProperty<int> Hp = new ReactiveProperty<int>(100);
    public readonly ReactiveProperty<string> Name = new ReactiveProperty<string>("player");
}

// 订阅会立即收到当前值（100），之后每次变化再收到
var subscription = model.Hp.Subscribe(hp => hpText.text = hp.ToString());

model.Hp.Value = 80;                 // 推送
model.Hp.SetValueWithoutNotify(60);  // 静默设置，不推送
subscription.Dispose();              // 退订
```

### 4.2 事件流与操作符

```csharp
var subject = new Subject<int>();
var subscription = subject
    .Where(v => v % 2 == 0)
    .Select(v => v * 10)
    .DistinctUntilChanged()
    .Subscribe(v => Debug.Log(v));

subject.OnNext(1);   // 被 Where 过滤
subject.OnNext(2);   // 20
subject.OnNext(2);   // 被去重
subject.OnNext(4);   // 40
```

### 4.3 生命周期统一释放

```csharp
public class HudPanel : MonoBehaviour
{
    readonly CompositeDisposable subscriptions = new CompositeDisposable();

    void Start()
    {
        subscriptions.Add(CosmosEntry.EventManager
            .Observe<HpChangedEventArgs>()
            .Where(args => args.Target == this)
            .Subscribe(args => Refresh(args)));

        subscriptions.Add(model.Name.Subscribe(OnNameChanged));
    }
    void OnDestroy() => subscriptions.Dispose();
}
```

### 4.4 与 Event 模块

Event 模块直接建立在本基座上：

```csharp
var subscription = CosmosEntry.EventManager.Subscribe<LoginEventArgs>(OnLogin);
CosmosEntry.EventManager.Publish(new LoginEventArgs { Account = "a" });

CosmosEntry.EventManager.Observe<LoginEventArgs>()
    .Where(args => args.Account != null)
    .Subscribe(OnLogin);
```

## 5. 边界

- 不提供调度器、节流 / 防抖 / 延时等时间操作符（需要时用 `Async.WaitForSeconds` 或在模块里实现）；
- 不提供背压与异步序列（`IAsyncEnumerable`）；
- 派发为同步调用，观察者里的耗时逻辑会阻塞发布方；
- 引用类型去重使用 `EqualityComparer<T>.Default`（默认引用相等），需要值语义时传入自定义比较器。
