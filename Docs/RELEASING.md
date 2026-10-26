# 分支与发布

本文是 `main` / `dev` 两条线的约定，以及一次发版的完整清单。改动分支结构前请先改这份文档。

## 分支模型

| 分支 | 角色 | 入口规则 | 说明 |
| --- | --- | --- | --- |
| `main`（默认分支） | **稳定线**，始终可安装 | 只接受来自 `dev` 的合并（发版时快进）与 `hotfix/*` | 对外展示、`#main` 安装语义、issue 的基准分支 |
| `dev` | **集成线**，日常开发 | 直接提交或功能分支合入 | 允许短暂不可编译；发布时快进到 `main` |
| `hotfix/*` | 紧急修复 | 从 `main` 的 tag 切出 | 修完同时合回 `main` 与 `dev` |
| tag `vX.Y.Z` | 发布点 | 只在 `main` 上打 | 与 `Assets/CosmosFramework/package.json` 版本一致 |

### 为什么 `main` 是默认分支，`dev` 不是

默认分支是项目的对外契约：别人 `git clone` 拿到的内容、README 的权威版本、`#main` 装出来的东西、提 issue 时被默认引用的代码，都指向它。`dev` 是集成线，重构期间不保证可编译（例如 2.0 的“移除旧资源加载方案”），把它设成默认，就等于让访客默认拿到半成品。

“想让人看到最新进展”这个需求由 tag、GitHub Release 和 README 承担，不由默认分支承担。**真正要控制的是 `dev` 与 `main` 的差距：以周计，而不是以年计。** 差距小的时候，谁当默认分支都不痛；差距 2.5 年的时候，怎么设都痛。

若将来确实想走单线（只保留 `dev` + tag，让 `main` 退休），那就把 `main` 删掉并在 README 写明“默认分支为开发版，安装请固定 tag”，不要留一条长期停滞的 `main` 当摆设。

## 发版清单

版本号规则：`MAJOR.MINOR.PATCH`，破坏性变更升 `MAJOR`。**tag 名 = `v` + `package.json` 里的 `version`**，两者不一致 CI 会拒绝发布。

按顺序执行，前一步不过不要进下一步。

1. **确认 `dev` 是绿的**

   ```bash
   dotnet run --project tools/RoslynCheck -- editor
   python3 tools/ci/release.py verify
   ```

2. **定版本并写 CHANGELOG**

   - 改 `Assets/CosmosFramework/package.json` 的 `version`；
   - 在 `CHANGELOG.md` 顶部加 `## [X.Y.Z] - YYYY-MM-DD` 段（破坏性变更写在最前面，并同步 README 的“破坏性更新”表）；
   - 若 `?path=` 安装地址需要指向新版本，同步改 README / README_EN 里的 `#vX.Y.Z`。

3. **快进合并到 `main`**（`dev` 落后 `main` 时改用 merge）

   ```bash
   git switch main
   git merge --ff-only dev
   ```

4. **打 tag 并推送**

   ```bash
   git tag -a vX.Y.Z -m "vX.Y.Z"
   git push origin main
   git push origin vX.Y.Z
   ```

5. **等 Release 工作流跑完**

   推送 tag 会触发 `.github/workflows/release.yml`：跑 Roslyn 编译校验 → 校验 tag 与 package.json / CHANGELOG 一致 → 从 CHANGELOG 生成正文并创建 GitHub Release。

6. **回填**：`dev` 继续开发；确认 README 的安装地址、Release 正文、`package.json` 的 `changelogUrl` 都能打开。

### 紧急修复

```bash
git switch -c hotfix/xxx vX.Y.Z
# 修 + 校验
git switch main && git merge --no-ff hotfix/xxx
git tag -a vX.Y.(Z+1) -m "vX.Y.(Z+1)" && git push origin main vX.Y.(Z+1)
git switch dev && git merge --no-ff hotfix/xxx   # 别让修复只活在 main 上
```

## 仓库设置（GitHub 网页，一次性）

- **Settings → General → Default branch**：保持 `main`。
- **Settings → Branches → Branch protection rules**：
  - `main`：禁止 force push、禁止删除；要求状态检查 `Roslyn asmdef compile check` 通过。
  - `dev`：至少禁止 force push（单人开发也建议开，一次误操作能丢掉整条开发线）。
- **Settings → General → Releases**：无需特殊设置，工作流用 `GITHUB_TOKEN` 创建。

## 待清理项（与分支模型无关，但影响可维护性）

- `tools/CompileCheck/**/bin`、`**/obj` 曾被提交进仓库（`.gitignore` 已补规则）。取消跟踪：

  ```bash
  git rm -r --cached tools/CompileCheck/*/bin tools/CompileCheck/*/obj
  ```

- `tools/CompileCheck` 是 1.3 时代的 stub 编译工程，当前编不过（缺 `Operation`、`ModuleConstant` 等）。CI 已改用 `tools/RoslynCheck`；这一套要么修好，要么整个删掉，别留着当陷阱。
- 远端 `v1.3.8` 分支与 tag `v1.3.8` 同名且多一个提交（塞了 `Packages/manifest.json`）。建议改名 `v1.3.8-project` 或删除。
- 远端 `v0.1`、`v1.0-archive`、`v1.1-archive`、`v1.2-archive` 都是 2022-12-04 的单提交快照，若无人依赖，转成 tag 后删除分支。
- 已废弃的 `upm` 分支（停在 1.3.2）：安装地址已改用 `?path=/Assets/CosmosFramework`，可删除，避免“分支列表里有个看起来是最新的 upm”误导。
