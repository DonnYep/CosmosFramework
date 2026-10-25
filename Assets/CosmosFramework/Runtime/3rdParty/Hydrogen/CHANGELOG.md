# Changelog

本文件记录所有值得注意的变更。格式参考 [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)，
版本号遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

## [0.1.0] - 2026-10-25

首个可直接通过 Git URL 安装的 UPM 版本。**代码逻辑未改动**，只调整包的形态。

### Added

- 标准 UPM 包结构：`package.json` / `README.md` / `CHANGELOG.md` / `LICENSE.md`，
  源码集中在 `Runtime/`，仓库根即包根，可用 Git URL 直接安装。
- 包元数据补全：`author` / `repository` / `documentationUrl` / `changelogUrl` / `licensesUrl`，
  Package Manager 详情页可直达仓库、变更记录与许可证。
- `.gitattributes`：统一 LF 换行、区分二进制与 Unity 资源，避免跨平台无意义 diff。
- README 新增「安装」章节：Git URL、版本标签、`manifest.json` 与本地 `file:` 四种方式。

### Changed

- 包名 `com.hydrogen` → `com.donnyep.hydrogen`。Unity 推荐 `com.<组织>.<包名>` 的反向域名格式，
  改名也避免与将来可能出现的同名注册表包冲突。**已有的引用需同步改名**。
- 版本 `0.0.1` → `0.1.0`。
- 源码由仓库根移入 `Runtime/`，为以后的 `Editor/`、`Tests/`、`Samples~` 预留同级位置。
- `LICENSE` → `LICENSE.md`，符合 Unity 包的许可证文件约定。
- `Hydrogen.asmdef` 补上 `rootNamespace: "Hydrogen"`（影响新建脚本的默认命名空间，不影响编译）。

### Removed

- `package.json.meta`、`README.md.meta`：文档类文件不属于 Unity 资源，不需要 meta。

### Fixed

- README 的 `IHeaderEncoder` 示例使用了 `System.Buffers.Binary.BinaryPrimitives`，
  该类在 Unity 2020.3 默认的 .NET Standard 2.0 下**不存在**（会导致示例直接编译失败），
  改为纯字节运算实现。
- README 里指向仓库外 `Docs/HYDROGEN.md` 的失效相对链接。
- README 中「裁剪用例数」前后不一致的表述，改为不依赖具体数字的验证范围描述。

## [0.0.1] - 2026-10-25

### Added

- 首个版本：`TcpClient`、`IHeaderEncoder`、`Log`、`DisconnectResion`。
- 异步收发（`BeginRead` / `BeginWrite`）、粘包半包处理、单写者发送队列、
  连接超时、`Dispatcher` 主线程派发、`IDisposable`。

### Fixed

首发前复查并修复的实现缺陷（公开 API 未变）：

- `BeginConnect` 回调里缺 `EndConnect` —— 连接被拒/超时永远发现不了，且回调里抛异常会导致**进程崩溃**。
- `EncodeHeader` 不校验长度 —— 超过 65535 字节的消息被静默截断长度头，**整条流从此错位**。
- `Disconnect` 关掉了流但 `Connect` 不重建 —— **重连后一收到数据就抛异常**。
- 连接未就绪时发送直接丢弃 —— **首包静默丢失**。
- 连接失败不关闭 socket —— **句柄泄漏**。
- `Connected` 直接透传 `TcpClient.Connected` —— 对端关闭后仍可能为 `true`，业务会误判。
- 多线程并发 `BeginWrite`、`SendTimeout` / `ReceiveTimeout` 对异步 I/O 无效 ——
  改为单写者队列 + 真正可用的连接超时。
