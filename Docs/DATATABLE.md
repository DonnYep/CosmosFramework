# 数据表模块（只读共享表 + 可插拔解析）

> 结论：**"一张表一个实例、所有消费者共享"是对的；错的是把共享对象做成了"可回收复用的享元"。**
> 本轮去掉行对象池与逐行增删，把行改成**加载后只读、永不回收**；
> 同时补齐生产真正需要的四件事：表头驱动的解析（反射只做一次）、加载状态与失败原因、
> 幂等加载/重载、以及不再"晚订阅收不到回调"的事件语义。

## 1. 旧设计（享元 + 行池）为什么和生产对不上

旧实现：`DataTable<T>` 用 `Dictionary<int,T>` 存行，行由 `Pool<T>` 创建；
`RemoveRow` 把行 `Despawn`（调 `Dispose` 重置字段），之后 `Spawn` 会**复用同一个实例**。
这是经典 Flyweight 写法，但配置表并不满足它的前提：

| Flyweight 的前提 | 配置表的实际 |
| --- | --- |
| 内在状态**不可变** | 行对象字段可写，`Dispose()` 还会把它重置成默认值 |
| 共享对象**不会被收回** | `RemoveRow` / 释放表会把行还池，`Spawn` 后**同一个对象变了内容** |
| 有大量**外在状态**要分离 | 配置行就是纯数据，没有外在状态可分离 |
| 创建成本高，值得池化 | 表只在启动/切图时加载一次，每行 `new T()` 一次，池化几乎不省任何东西 |

后果（都是生产会踩的）：

1. **引用失效/串行**：业务缓存了 `table[100]`，某处 `RemoveRow(100)` 或释放重建后，手里的引用可能已经是另一行的数据；
2. **`RemoveAllRows` 只 `Clear()` 不还池**、`OnRelease` 直接 `rowPool.Clear()` —— 行对象既不回收也不释放，池本身成了纯负担；
3. **逐行反射**：示例行在 `ParseData(string)` 里**每行**调 `GetTypeFieldsNameAndTypeMapping()` + `SetFieldValue()`，
   一万行 × 六列就是六万次反射 + 每行构建字典；这是加载卡顿的主要来源；
4. **按列号硬塞**：解析按字段声明顺序取值，CSV 列顺序一变就整体错列，而且不会报错；
5. **失败原因丢失**：`OnReadFailure(table, message)` 里 `message` 根本没交给表，业务只能知道"失败了"；
6. **事件触发后置空**：`onReadSuccess = null`，晚一步订阅就永远收不到（`Loading` 期间订阅也一样）；
7. **重复加载**：`ReadDataTableAssetAsync` 没有加载中标记，连续调用会并行加载同一张表；
8. **类型不匹配静默**：`GetDataTable<T>` 用 `as` 转换，类型不对直接返回 null，没有提示。

## 2. 新设计

```text
DataTableManager（模块，注册表 + 加载状态机 + 解析器注册表）
   │
   ├─ DataTableBase（非泛型视图：Name/State/Error/RowCount/事件/Dispose）
   │      └─ DataTable<T>（行数组 + 主键索引；只读）
   │             └─ IDataRowParser<T>（可插拔解析）
   │                    ├─ CsvDataRowParser<T>（默认：表头驱动、容错、支持引号）
   │                    │      └─ DataRowBinder<T>（反射只做一次；行实现 IRawDataRow 则零反射）
   │                    └─ 自定义（二进制/JSON/Protobuf…）
   └─ IDataTableHelper（资源加载，默认走 Resource 模块的 TextAsset）
```

### 2.1 行：只读共享，不回收

```csharp
public interface IDataRow { int Id { get; } }          // 去掉了 ParseData / IDisposable
public interface IRawDataRow {                          // 可选：零反射解析
    bool ParseFields(string[] fields, IReadOnlyDictionary<string,int> headerIndex);
}
```

- 行只在加载时创建一次，之后**不再回收复用**，业务缓存的引用永远有效；
- 表不再提供 `AddRow/RemoveRow`：配置表运行时只读（运行时数据请用普通集合）；
- 需要改配置？改资源 + `ReloadDataTableAsync`。

### 2.2 存储：数组 + 主键索引

```csharp
T[] rows;                         // 连续存储，遍历/筛选无哈希开销
Dictionary<int,int> idIndex;      // id → 下标，O(1)
IReadOnlyList<T> Rows { get; }    // 直接返回数组（零分配）
```

旧版 `Dictionary<int,T>` 每次 `GetAllRowDatas()` 都 `ToArray()` 分配一次；现在 `Rows` / `GetAllRowDatas()`
返回同一份数组（文档标注只读，需要副本自行 Clone）。

### 2.3 解析：表头驱动 + 反射只做一次

```csharp
// 默认解析器：首行是表头，按列名绑定；列顺序变化、多列少列都不影响
var table = manager.CreateDataTable<ItemRow>("Item");
manager.ReadDataTableAssetAsync(new DataTableAssetInfo("Item"), table, t => { ... });
```

- **按列名绑定**：`Hp,Id,Name` 与 `Id,Name,Hp` 等价；表里多出来的列被忽略（允许表结构演进）；
- **反射一次**：`DataRowBinder<T>` 在读到表头时构建「列 → 成员（字段/属性）」计划，
  之后每行只做取值 + 赋值。想彻底零反射就让行实现 `IRawDataRow`（生产建议用代码生成）；
- **容错可控**：空行、`#` 注释行跳过；坏行默认**整表失败**（避免"少了几行没人发现"），
  可 `SkipBadRows = true` 跳过；主键重复、缺主键列都会报错；
- **错误可定位**：`DataTableParseException` 带表名/行号/列名，例如
  `DataTable [Item] 第 3 行 : The input string 'abc' was not in a correct format.`；
- **支持引号**：`"a,b"`、`""` 转义（Excel 导出常见）。

### 2.4 加载：状态机 + 幂等 + 重载 + 事件不丢

```csharp
public enum DataTableState { None, Loading, Loaded, Failed }

table.State        // 加载状态
table.Error        // 失败原因（不再丢失）
table.OnReadSuccess += ...   // 已加载时订阅会立即回调
manager.ReadDataTableAssetAsync(info, table, onCompleted)  // 一次性回调，成功/失败都触发
manager.ReloadDataTableAsync("Item");                      // 热更/重进
manager.ReleaseAllDataTables();
```

- **幂等**：`Loading` 中重复请求直接忽略；已 `Loaded/Failed` 直接回调，不重复加载；
- **晚订阅可收到**：事件不再触发后置空，已加载/已失败时订阅会立即回调；
- **失败原因保留**：解析异常转成 `Failed` + `Error`，不再让异常穿透资源回调；
- **热重载**：`ReloadDataTableAsync` 清空旧数据后重走加载流程；
- **批量释放**：`ReleaseAllDataTables`，模块终止时自动调用。

### 2.5 AOT/IL2CPP

去掉了 `MakeGenericType + Activator` 的 `CreateDataTable(string name, Type rowType)`：
创建表只走泛型重载，`T` 在编译期确定，IL2CPP 下不会因为运行时泛型实例化失败。

## 3. API 迁移表

| 旧 | 新 |
| --- | --- |
| `IDataRow : IDisposable` + `ParseData(byte[])` + `ParseData(string)` | `IDataRow { int Id { get; } }`；解析交给 `IDataRowParser<T>`，零反射用 `IRawDataRow` |
| `AddRow` / `RemoveRow` / `RemoveAllRows` | 已移除（配置表只读）；运行时数据请用普通集合 |
| `GetAllRowDatas()`（每次新数组） | `Rows`（零分配）或 `GetAllRowDatas()`（返回内部数组） |
| `GetDataTable<T>` 类型不符静默 null | 返回 null + `LogWarning`；`CreateDataTable<T>` 冲突返回 null + `LogError` |
| `CreateDataTable(string name, Type rowType)` | 已移除（AOT 不安全），只用泛型重载 |
| `OnReadSuccess`（触发后置空） | 订阅晚于完成会立即回调 |
| 失败只发事件 | `table.Error` 保留原因 |
| `ReadDataTableAssetAsync` 可重复触发 | 幂等 + `onCompleted` 回调 + `ReloadDataTableAsync` |
| 行对象池（`Pool<T>`） | 已移除：行只读共享、不再回收 |

## 4. 行为验证

**14 用例 / 55 断言**的临时工程：把 `DataTable<T>` / `DataTableBase` / `DataTableManager` /
`CsvDataRowParser<T>` / `DataRowBinder<T>` 的**真实源码**编进控制台程序，用假帮助体控制加载时机。
位置：`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\datatable\harness`（仓库外，可删）。

```powershell
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\datatable\harness\DataTableHarness.csproj
# 断言：55 通过 / 0 失败  === ALL-PASS ===
```

覆盖：表头列序变化、干扰列、引号/转义、注释/空行、坏行两种策略、重复主键、缺主键列、
主键查询与零分配视图、反射绑定与枚举/空值转换、晚订阅、幂等加载、重载、失败原因、类型不匹配、
释放与批量释放、**释放重建后旧行引用仍是原数据**（享元误用的核心回归）。

并用**变异测试**验证用例有效：重新注入 7 个旧缺陷
（按列号硬塞、不查主键重复、晚订阅不回调、加载不幂等、丢失败原因、忽略 SkipBadRows、关闭引号解析）
**全部被检出**。

## 5. 如果以后要更"生产"（未做）

| 方向 | 说明 |
| --- | --- |
| 代码生成行类 | 编辑器从表头生成 `ParseFields`，彻底零反射（当前 `IRawDataRow` 就是这个钩子） |
| 结构化格式 | 大表用二进制/MessagePack 替代 CSV，加载快一个量级（`IDataRowParser<T>` 直接换实现） |
| 二级索引 | `CreateIndex<TKey>(selector)` 支持"按职业查所有技能"这类查询，替代全表 `GetRowDatas` |
| 分表/分包 | 按模块拆表并按需加载（当前 `ReleaseDataTable` 已支持单表卸载） |
| 只读强约束 | 行类型用 `readonly struct` 或生成只读属性；`IRawDataRow` 版本可以做到 |
