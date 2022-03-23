[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/DonnYep/CosmosFramework/blob/V1.1/LICENSE)
[![Issues:Welcome](https://img.shields.io/badge/Issues-welcome-blue.svg)](https://github.com/DonnYep/CosmosFramework/issues)

# [中文](https://github.com/DonnYep/CosmosFramework/blob/V1.1/README_CN.md) 

# CosmosFramework

CosmosFramework is a lightweight Unity development framework . Has a rich Unity method extensions and toolchain. async/await syntax support, multi-network channel support.The framework has been supported UMP, it is recommended to put it into the Packages directory to use.
* [CosmosFramework Wiki](https://github.com/DonnYep/CosmosFramework/wiki)<br/>

## Master, V0.1, V1.0 branches are closed for maintenance. Please switch to V1.1 branch for the latest content.

## Environment

- Unity version:2018 and above,DOTNET API version:4.x.

## Module Introduction

- **Audio** : Game audio module. By registering audio information, you can automatically load audio resources to play by passing in the audio name when playing. audio effects support grouping, and the whole group can be played.

- **Config** : Game configuration module. The user can read the configuration file at game initialization and cache it in the configuration module. At runtime the data corresponding to the configuration is read at other desired locations.

- **Event** : Event Center module. Using the standard event model, it provides common event functions such as listening, removing and dispatching. Provides event observation methods to detect event status in real time.

- **FSM** : Finite state machine module. Fully abstract finite state machines that can do state machine implementations for different types of owners.

- **ObjectsPool** : Object pool module. Provides commonly used functions such as entity object generation and recycling. The underlying implementation uses the data structure Pool.

- **Resource** : Resource loading module. Built-in provides Resources and AB two loading modes, by switching the loading mode can change the current default loading method. Support resources multi-channel custom loading channel, can be customized to adapt to other types of resource management solutions.

- **Scene** : Scene loading module. Provide common asynchronous and synchronous loading of embedded scenes. Support custom implementation of loading methods.

- **DataNode** : Data caching module. Provides a tree-structured data caching center.

- **Entity** : Game Entity Module. Manage the game runtime entity objects. Entity support group classification, by passing in the resource address can directly manage the resource entity object generation, recycling and other operations, built-in object pool generation.

- **Input** : Input adaptation module. The input of each platform is simulated by virtual keys, and different input adapters can be passed in to adapt the input methods of different platforms.

- **Hotfix** : Hot update module. This module is for C#-based hot update solutions. (This solution is not completed.)

- **Network** : Network module. A variety of high-speed and reliable UDP protocols are provided, such as RUDP, SUDP, KCP, TCP, etc. The KCP protocol is used by default. Network in the form of channel (Channel) to distinguish each connection, support a variety of network types connected at the same time. Can be implemented (Client-Server) mode. Support async/await syntax.

- **UI** : UI module. Based on UGUI implementation. Provide UI common functions, such as priority, reality hiding, getting and setting group. Extension method extends some common components such as buttons, so that you can listen to the interface implementation without manually implementing buttons lift, press, etc. Support commonly used UIBehaiour type triggerEvent.

- **Main** : Module Center. Both custom modules and extensions are stored here. Custom modules are written in the same format as the built-in modules and have exactly the same lifecycle and permissions as the built-in modules. It is almost identical to the built-in module. The built-in polling pools of this main module: FixedRefreshHandler, LateRefreshHandler, RefreshHandler, ElapseRefreshHandler can poll the objects that need to be polled uniformly, reducing the performance loss caused by too many update and other mono callbacks. performance loss caused by too many update and other mono callbacks.

- **Controller** : Controller module. By registering with this module, polling can be managed without generating a GameObject. This module provides update polling.

- **WebRequest** : UnityWebRequest module, can be used to load persistent resources, network resources download and other needs. It supports getting AssetBundle, AudioClip, Texture2D, string, and when the resource is obtained, the user can operate on the resource through WebRequestCallback.

- **Download** : Download module. Support localhost local file download and http file download. Asynchronous incremental writing to localhost in byte stream when downloading files. Support for dynamic addition and removal of download tasks during downloading.

## Built-in data structures, tools

- **Utility** : Provides common tool functions such as Reflection, Algorithm, Assertion, Conversion, Debug Rich Text, IO, Encryption, Json, MessagePack, Time, Text, Unity Coroutine, Unity Component, etc.

- **Singleton** : Singleton base class. Provides thread-safe, non-thread-safe, MONO singleton base classes.

- **DataStructure** : Common data structures. Data structures such as linked table, bidirectional linked table, bidirectional dictionary, binary tree, quadtree, AStar, LRU, thread lock, etc.

- **Extensions** : Static extension tool. Provides extensions for Unity and native extensions for common data structures in C# Collections.

- **Awaitable** : This tool provides support for async/await syntax in the unity environment. You can write asynchronous in Unity just like c# native asynchronous. Support Task asynchronous, Task execution will return to the main thread after completion, when using the normal format can be written.

- **EventCore** : Fully abstract event data structure. Contains normal, standard and thread-safe types.
```CSharp
    //Declare a class so that it derives from EventCore and does type constraints.
    public class MyEventCore :EventCore<string,string, MyEventCore>{}
    //Once implemented, you can use the custom event listener.
```
- **ReferencePool** : Global reference pool module.

- **Editor** : Editor provides methods to retrieve objects and components commonly used in Hierarchy, and EditorConfig provides the ability to automatically create code headers for code generation.

- **QuarkAsset** : QuarkAsset is a set of AssetBundle resource management solutions. Editor mode and AB mode can be quickly switched between, and AB dependency issues are automatically handled when loading resources. Download support for large file breakpoints. Loading can be done by resource name without passing in the full address. If the resource is renamed, it will be loaded fully qualified by filename + suffix.

- **FutureTask**:Asynchronous task detection, supports multi-threaded and concurrent asynchronous progress detection. The detection function needs to be passed in Func<bool> format, and asynchronous detection ends when the condition returns true; Note: FutureTask itself is not a concurrent process, and cannot execute asynchronous tasks instead of a concurrent process. The await/async syntax is not supported at the moment.

- **Pool**:Pool data structure. Includes thread-safe and non-thread-safe types. Object pools, reference pools and cache pools of other modules in the framework are implemented using "Pool".
    
## Built-in Architecture PureMVC

- A more understandable architecture based on the original PureMVC improvements.
    The framework provides a more concise feature-based registration:
    - 1.MVCCommandAttribute, corresponding to the Command, i.e. C layer.
    - 2.MVCMediatorAttribute, corresponding to the Mediator, i.e. the V layer.
    - 3.MVCProxyAttribute, corresponding to the Proxy, i.e. the M layer.
    
- The derived proxy class needs to override the constructor and pass in the NAME parameter.

- The derived proxy class needs to override the constructor and pass in the NAME parameter.

- Note that the MVC.RegisterAttributedMVC() method needs to be passed in the corresponding assembly. Multi-assembly reflection is supported.

## Cautions

- Project Launch:
    Mount CosmosConfig on a suitable GameObject and run Unity. if PrintModulePreparatory on CosmosConfig is in true state, the console will show the initialization message.  Since then, the project startup is complete.
    
- Some of the modules with Helper can be custom implemented by the user or can use the Default object provided.

- Please use V1.1 version for the latest, V0.1, 1.0 stop maintenance. master suspend maintenance.

- Built-in case address:Assets\Examples\.

## Other

- V1.1 supports UPM. select Assets/CosmosFramework folder and copy it to Packages directory of the project to use this framework.
    
- For a demonstration, please watch the video:
    - https://www.bilibili.com/video/BV1x741157eR
    - https://www.bilibili.com/video/BV17u411Z7Ni/
    
## Library link

- CosmosEngine:https://github.com/DonnYep/CosmosEngine

- KCP C:https://github.com/skywind3000/kcp
    
- KCP CSharp:https://github.com/vis2k/kcp2k
    
- TCP:https://github.com/vis2k/Telepathy

- PureMVC:https://github.com/DonnYep/PureMVC

- Mirror:https://github.com/vis2k/Mirror
