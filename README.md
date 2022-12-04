# 此分支停止维护，已存档

# CosmosFramework
CosmosFramework是一款基于Unity的轻量级游戏框架。内置常用模块、算法工具类等。自定义模块可与原生模块完全同等优先级，共享相同的生命周期。

## 环境

- Unity版本：2017及以上； .NET API版本：4.x。

## 模块简介

- **Audio**： 框架中的音效管理模块，提供游戏内音效的播放、暂停等常用功能。

- **Config**： 游戏常用配置模块。用户可在游戏初始化时读取配置文件，并缓存于配置模块。运行时在其他所需位置读取对应配置的数据。

- **Event**： 事件中心模块。使用标准事件模型，提供了监听、移除、派发等常用事件功能。

- **FSM**： 有限状态机模块。对状态机进行创建、回收、管理。

- **Mono**：MONO模块。提供常用的开启、关闭协程等功能。协程可在此进行统一管理，其中内置的协程可以直接进行调用，无需额外写协程方法。

- **ObjectsPool**：对象池模块。提供常用的对象生成回收等功能。

- **ReferencePool**：引用池模块。为减少GC，框架特意提供引用池对实现了IReference接口的对象进行回收重复利用。减少由于重复生成对象导致的GC性能损耗。

- **Resource**：资源加载模块。框架集成了基于Resources与AB两种加载模式，在使用时切换加载模式即可。并提供通过特性"PrefabAssetAttribute"加载资源的方式。

- **Scene**：场景加载模块。提供常用的异步、同步加载嵌入的场景功能。

- **Data**：数据缓存模块。提供树状结构的数据缓存中心。

- **Entity**：游戏实体模块。管理游戏运行时的实体对象。

- **Input**：输入适配模块。用于适配不同平台的输入方式。

- **Hotfix**：热更新模块。此模块适用于基于C#的热更方案，如ILRuntime、JEngine等。

- **Network**：网络模块。当前模块提供了可靠的UDP协议，并集成了KCP协议。TCP协议持续更新中。

- **UI**：UI模块。基于UGUI实现。提供UI常用功能，如优先级、现实隐藏、获取等功能。

- **Main**：模块中心。自定义模块与扩展模块都存于此。自定义模块按照内置模块相同格式写入后，可享有完全同等与内置模块的生命周期与权限。几乎与内置模块无异。此主模块的内置轮询池：FixedRefreshHandler、LateRefreshHandler、RefreshHandler、ElapseRefreshHandler可对需要统一进行轮询管理的对象进行统一轮询，减少由于过多的Update等mono回调导致的新能损耗。

- **Controller**：控制器模块。提供常用需要轮询(Update)对象的统一管理。

## 内置数据、工具

- **Utility**：提供了反射、算法、断言、转换、Debug富文本、IO、加密、Json、MessagePack、Time、Text、Unity等常用工具函数。是非常实用的工具包。

- **Singleton**：单例基类。提供了线程安全、非线程安全、MONO单例基类。使用时仅继承即可。

- **DataStructure**：常用数据结构。链表、双向链表、二叉树、LRU、线程锁等数据结构。

- **Behaviour**：内置生命周期函数。生命周期优先级为：OnInitialization > OnActive > OnPreparatory > OnFixRefresh > OnRefresh > OnLateRefresh > OnDeactive > OnTermination。此生命周期可参考Unity的MONO生命周期。需要注意，此内置生命周期适用于原生模块与自定义模块，相对于Unity生命周期是独立的。

- **Extension**：静态扩展工具。提供unity的扩展以及.NETCore对unity.NET的扩展。

-**EventCore** ：轻量级事件模块，可自定义监听的数据类型；

## 内置软件架构 MVVM

- MVVM是基于PureMVC改进的更适于理解的软件架构。对Command、Mediator、Proxy注册使用基本与PureMVC相同。框架提供了基于特性更加简洁的注册方式，即：MVVMCommandAttribute、MVVMMediatorAttribute、MVVMProxyAttribute分别对应各自的注册类型，在入口调用MVVM.RegisterAttributedMVVM()方法即可。

- 需要注意，MVVM.RegisterAttributedMVVM()方法需要传入对应的程序集。目前已经验证跨程序集反射MVVM成员是可行且稳定的。

## 注意事项

- 所有带Default开头的模块、方法，都需要在开始时都需要传入其对应的默认类型的Hepler传入。

- 框架的AB方案正在开发中。

- 框架提供第三方适配，如Utility.Json，用户可自定义任意JSON方案。框架建议使用的高速传输协议为MessagePack，包含适配方案。
MessagePack 链接地址：https://github.com/neuecc/MessagePack-CSharp

- 默认请使用 V1.0 版本，V0.1 已经停止维护。Master暂停维护，所有最新的功能都在V1.0中。

- 内置案例地址：Assets\Examples\ExampleScripts 。

## 其他

- MVVM的纯C#版本：https://github.com/DonHitYep/Cosmos_SimpleMVVM

- CosmosFramework的服务器版本：https://github.com/DonHitYep/CosmosFramework4Server 
服务器版本已提供可靠UDP协议，并集成了KCP协议。TCP持续更新中。内置模块与Unity客户端类似，内置类型都为线程安全类型。

- KCP地址：https://github.com/skywind3000/kcp

- 服务器版本的KCP与客户端版本的KCP皆为参考自Mirror。
    Mirror地址:https://github.com/vis2k/Mirror


