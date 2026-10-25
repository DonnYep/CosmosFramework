using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace RoslynCheck
{
    /// <summary>
    /// 使用 Roslyn (Microsoft.CodeAnalysis.CSharp) 按 Unity asmdef 边界编译 CosmosFramework 程序集。
    /// <para>用法：dotnet run --project tools/RoslynCheck -- [core|runtime|editor|adapters|examples]</para>
    /// <para>core     —— 仅 Cosmos.Resource.Abstractions（契约 + 通用异步底座）</para>
    /// <para>runtime  —— Cosmos.Resource.Abstractions + Cosmos.Runtime（全量运行时，含全部模块）</para>
    /// <para>editor   —— 上述两者 + Cosmos.Editor（全量编辑器）</para>
    /// <para>adapters —— 三个可选资源适配器（Addressables / YooAsset / QuarkAsset），使用第三方 API 桩单独编译</para>
    /// <para>examples —— 与资源桥相关的示例脚本（含 Entity 示例目录），验证示例代码与框架 API 一致</para>
    /// </summary>
    public static class Program
    {
        static readonly string[] Defines =
        {
            "UNITY_EDITOR",
            "UNITY_2018_1_OR_NEWER",
            "UNITY_2019_1_OR_NEWER",
            "UNITY_2020_1_OR_NEWER",
            "UNITY_2021_1_OR_NEWER",
        };

        static readonly string Root = FindRepoRoot();

        static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "Assets", "CosmosFramework", "Runtime", "Cosmos.Runtime.asmdef")))
                    return dir.FullName;
                dir = dir.Parent;
            }
            throw new InvalidOperationException("Repo root not found !");
        }

        static string AssetsPath { get { return Path.Combine(Root, "Assets", "CosmosFramework"); } }
        static string StubsPath { get { return Path.Combine(Root, "tools", "CompileCheck", "UnityStubs"); } }

        public static int Main(string[] args)
        {
            var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "runtime";
            var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp9, DocumentationMode.None, SourceCodeKind.Regular, Defines);
            var references = NetStandard21.References.All.Cast<MetadataReference>().ToList();
            var errors = 0;

            // asmdef 引用一致性（Unity 按 asmdef 划分程序集，引用缺失只有 Unity 才会报，这里提前拦）
            errors += CheckAsmdefReferences();
            if (errors != 0)
                return 1;

            // 模拟 UnityEngine 预编译程序集：Unity 桩
            var stubTrees = LoadFiles(GetStubFiles(), parseOptions);
            var stubCompilation = CSharpCompilation.Create(
                "UnityEngine.Stub",
                stubTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("UnityEngine.Stub", stubCompilation, GetDumpPath(args, "stub"));
            if (errors != 0)
                return 1;

            // 可选：导出全部诊断到文件（用于补桩分析）
            var dumpPath = GetArgValue(args, "dump");

            // Hydrogen —— 第三方 TCP 库（独立 asmdef，不参与 Cosmos.Runtime 编译）
            var hydrogenTrees = LoadFiles(GetHydrogenFiles(), parseOptions);
            if (hydrogenTrees.Count > 0)
            {
                var hydrogenReferences = new List<MetadataReference>(references);
                hydrogenReferences.Add(stubCompilation.ToMetadataReference());
                var hydrogenCompilation = CSharpCompilation.Create(
                    "Hydrogen",
                    hydrogenTrees,
                    hydrogenReferences,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                errors += Report("Hydrogen", hydrogenCompilation, dumpPath);
            }

            if (mode == "hydrogen")
                return errors == 0 ? 0 : 1;

            // Cosmos.Async —— 通用异步底座（不依赖任何模块：
            // OperationBase / OperationSystem / OperationDriver / GameAsyncOperation / OperationStatus）
            var asyncTrees = LoadFiles(GetAsyncFiles(), parseOptions);
            var asyncReferences = new List<MetadataReference>(references);
            asyncReferences.Add(stubCompilation.ToMetadataReference());
            var asyncCompilation = CSharpCompilation.Create(
                "Cosmos.Async",
                asyncTrees,
                asyncReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("Cosmos.Async", asyncCompilation, dumpPath);

            if (mode == "async")
                return errors == 0 ? 0 : 1;

            // Cosmos.Resource.Core —— 资源契约库（引用异步底座）
            var coreTrees = LoadFiles(GetCoreFiles(), parseOptions);
            var coreReferences = new List<MetadataReference>(references);
            coreReferences.Add(stubCompilation.ToMetadataReference());
            coreReferences.Add(asyncCompilation.ToMetadataReference());
            var coreCompilation = CSharpCompilation.Create(
                "Cosmos.Resource.Abstractions",
                coreTrees,
                coreReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("Cosmos.Resource.Abstractions", coreCompilation, dumpPath);

            if (mode == "core")
                return errors == 0 ? 0 : 1;

            // 可选适配器（Addressables / YooAsset / QuarkAsset）：用第三方 API 桩单独编译。
            if (mode == "adapters")
            {
                errors += CheckAdapters(references, stubCompilation, coreCompilation, asyncCompilation);
                Console.WriteLine(errors == 0
                    ? "=== ADAPTERS COMPILED SUCCESSFULLY ==="
                    : $"=== ADAPTERS COMPILE FAILED : {errors} errors ===");
                return errors == 0 ? 0 : 1;
            }

            // Cosmos.Runtime —— 全量运行时（引用 Core）
            var runtimeTrees = LoadFiles(GetRuntimeFiles(), parseOptions);
            // 框架级桩（因目标框架不支持而替换的运行时文件 + BCL 兼容扩展）编译进 Runtime 程序集
            runtimeTrees.AddRange(LoadFiles(GetFrameworkStubFiles(), parseOptions));
            var runtimeReferences = new List<MetadataReference>(references);
            runtimeReferences.Add(stubCompilation.ToMetadataReference());
            runtimeReferences.Add(asyncCompilation.ToMetadataReference());
            runtimeReferences.Add(coreCompilation.ToMetadataReference());
            var runtimeCompilation = CSharpCompilation.Create(
                "Cosmos.Runtime",
                runtimeTrees,
                runtimeReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("Cosmos.Runtime", runtimeCompilation, dumpPath);

            if (mode == "runtime")
                return errors == 0 ? 0 : 1;

            // 资源相关示例（验证示例代码与资源桥 API 一致）
            if (mode == "examples")
            {
                errors += CheckExamples(parseOptions, runtimeReferences, runtimeCompilation, asyncCompilation);
                Console.WriteLine(errors == 0
                    ? "=== EXAMPLES COMPILED SUCCESSFULLY ==="
                    : $"=== EXAMPLES COMPILE FAILED : {errors} errors ===");
                return errors == 0 ? 0 : 1;
            }

            // Cosmos.Resource.EditorFallback —— 编辑器兜底 Provider（asmdef 独立程序集）
            var fallbackTrees = LoadFiles(GetEditorFallbackFiles(), parseOptions);
            var fallbackReferences = new List<MetadataReference>(runtimeReferences);
            fallbackReferences.Add(asyncCompilation.ToMetadataReference());
            fallbackReferences.Add(coreCompilation.ToMetadataReference());
            var fallbackCompilation = CSharpCompilation.Create(
                "Cosmos.Resource.EditorFallback",
                fallbackTrees,
                fallbackReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("Cosmos.Resource.EditorFallback", fallbackCompilation, dumpPath);

            // Cosmos.Editor —— 编辑器工具（引用 Core + Async + Runtime + EditorFallback）
            var editorTrees = LoadFiles(GetEditorFiles(), parseOptions);
            var editorReferences = new List<MetadataReference>(runtimeReferences);
            editorReferences.Add(asyncCompilation.ToMetadataReference());
            editorReferences.Add(runtimeCompilation.ToMetadataReference());
            editorReferences.Add(fallbackCompilation.ToMetadataReference());
            var editorCompilation = CSharpCompilation.Create(
                "Cosmos.Editor",
                editorTrees,
                editorReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("Cosmos.Editor", editorCompilation, dumpPath);

            Console.WriteLine(errors == 0
                ? "=== ALL ASSEMBLIES COMPILED SUCCESSFULLY ==="
                : $"=== COMPILE FAILED : {errors} errors ===");
            return errors == 0 ? 0 : 1;
        }

        /// <summary>
        /// 校验 asmdef 引用一致性。
        /// <para>规则：引用 <c>Cosmos.Resource.Abstractions</c> 或 <c>Cosmos.Runtime</c> 的程序集必须同时引用
        /// <c>Cosmos.Async</c>——它们的公开/受保护 API 暴露了 <c>OperationBase</c> 等异步类型，
        /// 缺失时会得到 Unity 才报的 CS0012（本工具按目录编译，天然看不到）。</para>
        /// <para>同时校验 <c>Cosmos.*</c> 引用是否存在（拼写错误）。</para>
        /// </summary>
        /// <returns>错误数</returns>
        static int CheckAsmdefReferences()
        {
            var assetsRoot = Path.Combine(Root, "Assets");
            if (!Directory.Exists(assetsRoot))
                return 0;
            var names = new HashSet<string>(StringComparer.Ordinal);
            var entries = new List<Tuple<string, string, List<string>>>();
            foreach (var file in Directory.GetFiles(assetsRoot, "*.asmdef", SearchOption.AllDirectories).OrderBy(f => f))
            {
                using (var document = JsonDocument.Parse(File.ReadAllText(file)))
                {
                    var root = document.RootElement;
                    var name = root.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : Path.GetFileNameWithoutExtension(file);
                    var referencesOfAsmdef = new List<string>();
                    if (root.TryGetProperty("references", out var referencesElement))
                    {
                        foreach (var reference in referencesElement.EnumerateArray())
                        {
                            var value = reference.GetString();
                            if (!string.IsNullOrEmpty(value))
                                referencesOfAsmdef.Add(value);
                        }
                    }
                    names.Add(name);
                    entries.Add(new Tuple<string, string, List<string>>(name, file, referencesOfAsmdef));
                }
            }
            var errors = 0;
            foreach (var entry in entries)
            {
                var name = entry.Item1;
                var referencesOfAsmdef = entry.Item3;
                var needsAsync = referencesOfAsmdef.Contains("Cosmos.Resource.Abstractions") || referencesOfAsmdef.Contains("Cosmos.Runtime");
                if (needsAsync && !referencesOfAsmdef.Contains("Cosmos.Async"))
                {
                    Console.WriteLine($"[FAIL] asmdef {name} : 缺少 Cosmos.Async 引用（{entry.Item2}）");
                    errors++;
                }
                foreach (var reference in referencesOfAsmdef)
                {
                    if (reference.StartsWith("GUID:", StringComparison.Ordinal))
                        continue;
                    if (reference.StartsWith("Cosmos.", StringComparison.Ordinal) && !names.Contains(reference))
                    {
                        Console.WriteLine($"[FAIL] asmdef {name} : 引用了不存在的程序集 {reference}（{entry.Item2}）");
                        errors++;
                    }
                }
            }
            Console.WriteLine(errors == 0
                ? $"[OK] asmdef references : {entries.Count} asmdef(s)"
                : $"[FAIL] asmdef references : {errors} error(s)");
            return errors;
        }

        static string GetArgValue(string[] args, string key)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--" + key)
                    return args[i + 1];
            }
            return null;
        }

        static string GetDumpPath(string[] args, string suffix)
        {
            var path = GetArgValue(args, "dump");
            if (string.IsNullOrEmpty(path))
                return null;
            return Path.Combine(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(path)}_{suffix}.txt");
        }

        static int Report(string assemblyName, CSharpCompilation compilation, string dumpPath = null)
        {
            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);
                var diagnostics = result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .OrderBy(d => d.Location.SourceTree?.FilePath ?? string.Empty)
                    .ThenBy(d => d.Location.SourceSpan.Start)
                    .ToList();
                if (diagnostics.Count == 0)
                {
                    Console.WriteLine($"[OK] {assemblyName} : {compilation.SyntaxTrees.Count()} files, 0 errors");
                    return 0;
                }
                Console.WriteLine($"[FAIL] {assemblyName} : {diagnostics.Count} errors");
                if (!string.IsNullOrEmpty(dumpPath))
                {
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"== {assemblyName} : {diagnostics.Count} errors ==");
                    foreach (var d in diagnostics)
                    {
                        var location = d.Location;
                        var file = location.SourceTree != null ? Path.GetFileName(location.SourceTree.FilePath) : "?";
                        var span = location.GetLineSpan();
                        sb.AppendLine($"{d.Id} {file}({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1}) : {d.GetMessage()}");
                    }
                    File.WriteAllText(dumpPath, sb.ToString());
                    Console.WriteLine($"  full diagnostics dumped to : {dumpPath}");
                }
                var shown = 0;
                foreach (var d in diagnostics)
                {
                    if (shown++ >= 60)
                    {
                        Console.WriteLine($"  ... and {diagnostics.Count - shown + 1} more errors");
                        break;
                    }
                    var location = d.Location;
                    var file = location.SourceTree != null ? Path.GetFileName(location.SourceTree.FilePath) : "?";
                    var span = location.GetLineSpan();
                    Console.WriteLine($"  {d.Id} {file}({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1}) : {d.GetMessage()}");
                }
                return diagnostics.Count;
            }
        }

        static List<SyntaxTree> LoadFiles(IEnumerable<string> files, CSharpParseOptions parseOptions)
        {
            var trees = new List<SyntaxTree>();
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                trees.Add(CSharpSyntaxTree.ParseText(SourceText.From(text, System.Text.Encoding.UTF8), parseOptions, file));
            }
            return trees;
        }

        /// <summary>
        /// 编译与资源桥相关的示例脚本：这些示例直接调用 IResourceManager / 句柄 API，
        /// 用它们验证示例与框架 API 保持一致（示例目录不参与 Unity 程序集编译检查）。
        /// </summary>
        static int CheckExamples(CSharpParseOptions parseOptions, List<MetadataReference> runtimeReferences, CSharpCompilation runtimeCompilation, CSharpCompilation asyncCompilation)
        {
            var exampleRoot = Path.Combine(Root, "Assets", "Examples", "ExampleScripts");
            var relativePaths = new[]
            {
                "02_ResourceLoad/CFResourceExample.cs",
                "02_ResourceLoad/MyResourceLoader.cs",
                "10_Scene/LoadScenePanel.cs",
                "24_Procedure/ProcedureStates/PloyState.cs",
                "24_Procedure/ProcedureStates/YBotState.cs",
                // 可选下载插件（Cosmos.Download）的示例
                "09_Download/MyDownload.cs",
            };
            var optionalDirectories = new[]
            {
                // Entity 示例内部互相引用，整目录一起编译
                "23_Entity",
                // 输入模块：自定义绑定示例（CustomButtonName + Helper + Subscriber）
                "26_CustomInput",
                // 数据表模块：自定义行解析 + 面板用法
                "13_DataTable",
                // 统一异步方案：条件等待/定时/协程/Task（原 Awaitable + FutureTask 示例）
                "11_Awaitable",
                "22_FutureTask",
                // 自定义异步操作示例（GameAsyncOperation）
                "27_AsyncOp",
            };
            var files = relativePaths
                .Select(p => Path.Combine(exampleRoot, p.Replace('/', Path.DirectorySeparatorChar)))
                .Where(File.Exists)
                .OrderBy(f => f)
                .ToList();
            foreach (var directory in optionalDirectories)
            {
                var path = Path.Combine(exampleRoot, directory.Replace('/', Path.DirectorySeparatorChar));
                if (Directory.Exists(path))
                    files.AddRange(Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).OrderBy(f => f));
            }
            if (files.Count == 0)
            {
                Console.WriteLine("[SKIP] Examples.Resource : no example file found");
                return 0;
            }
            var trees = LoadFiles(files, parseOptions);
            var references = new List<MetadataReference>(runtimeReferences);
            references.Add(asyncCompilation.ToMetadataReference());
            references.Add(runtimeCompilation.ToMetadataReference());
            var compilation = CSharpCompilation.Create(
                "Examples.Resource",
                trees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return Report("Examples.Resource", compilation);
        }

        /// <summary>
        /// 编译可选适配器程序集：每个适配器单独编译，并引用第三方 API 桩。
        /// </summary>
        static int CheckAdapters(List<MetadataReference> references, CSharpCompilation stubCompilation, CSharpCompilation coreCompilation, CSharpCompilation asyncCompilation)
        {
            var errors = 0;
            var thirdPartyParse = new CSharpParseOptions(LanguageVersion.CSharp9, DocumentationMode.None, SourceCodeKind.Regular, Defines);
            var thirdPartyTrees = LoadFiles(new[] { Path.Combine(StubsPath, "ThirdPartyStubs.cs") }, thirdPartyParse);
            var thirdPartyReferences = new List<MetadataReference>(references);
            thirdPartyReferences.Add(stubCompilation.ToMetadataReference());
            var thirdPartyCompilation = CSharpCompilation.Create(
                "ThirdParty.Stub",
                thirdPartyTrees,
                thirdPartyReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("ThirdParty.Stub", thirdPartyCompilation);
            if (errors != 0)
                return errors;

            var adapters = new[]
            {
                new { Directory = "Addressables", Define = "CF_ADDRESSABLES", Assembly = "Cosmos.Resource.Addressables" },
                new { Directory = "YooAsset", Define = "CF_YOOASSET", Assembly = "Cosmos.Resource.YooAsset" },
                new { Directory = "QuarkAsset", Define = "CF_QUARKASSET", Assembly = "Cosmos.Resource.QuarkAsset" },
            };
            foreach (var adapter in adapters)
            {
                var directory = Path.Combine(AssetsPath, "Runtime", "Integrations", adapter.Directory);
                if (!Directory.Exists(directory))
                    continue;
                var defines = Defines.Concat(new[] { adapter.Define }).ToArray();
                var adapterParseOptions = new CSharpParseOptions(LanguageVersion.CSharp9, DocumentationMode.None, SourceCodeKind.Regular, defines);
                var trees = LoadFiles(Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories).OrderBy(f => f), adapterParseOptions);
                var adapterReferences = new List<MetadataReference>(references);
                adapterReferences.Add(stubCompilation.ToMetadataReference());
                adapterReferences.Add(asyncCompilation.ToMetadataReference());
                adapterReferences.Add(coreCompilation.ToMetadataReference());
                adapterReferences.Add(thirdPartyCompilation.ToMetadataReference());
                var compilation = CSharpCompilation.Create(
                    adapter.Assembly,
                    trees,
                    adapterReferences,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                errors += Report(adapter.Assembly, compilation);
            }
            return errors;
        }

        #region 文件集合

        static IEnumerable<string> GetStubFiles()
        {
            // 仅 Unity API 桩进入 UnityEngine.Stub 模拟程序集
            return new[] { Path.Combine(StubsPath, "UnityStubs.cs") };
        }

        static IEnumerable<string> GetFrameworkStubFiles()
        {
            // 框架级桩编译进 Cosmos.Runtime：Utility.Encryption（RealRemoting不可用）与BCL兼容扩展
            return new[]
            {
                Path.Combine(StubsPath, "Utility.Encryption.Stub.cs"),
                Path.Combine(StubsPath, "CompatShims.cs"),
            };
        }

        static IEnumerable<string> GetHydrogenFiles()
        {
            // 第三方 TCP 库（独立 asmdef）
            var hydrogenRoot = Path.Combine(AssetsPath, "Runtime", "3rdParty", "Hydrogen");
            if (!Directory.Exists(hydrogenRoot))
                return new string[0];
            return Directory.GetFiles(hydrogenRoot, "*.cs", SearchOption.AllDirectories).OrderBy(f => f);
        }

        static IEnumerable<string> GetAsyncFiles()
        {
            // 通用异步底座：不依赖资源/任何模块
            return Directory.GetFiles(Path.Combine(AssetsPath, "Runtime", "Base", "Async"), "*.cs", SearchOption.AllDirectories)
                .OrderBy(f => f);
        }

        static IEnumerable<string> GetCoreFiles()
        {
            return Directory.GetFiles(Path.Combine(AssetsPath, "Runtime", "Base", "ResourceAbstractions"), "*.cs", SearchOption.AllDirectories)
                .OrderBy(f => f);
        }

        static IEnumerable<string> GetRuntimeFiles()
        {
            var runtimeRoot = Path.Combine(AssetsPath, "Runtime");
            //以下目录不是 Cosmos.Runtime 程序集的一部分：
            //- Base/Async、Base/ResourceAbstractions：各自有 asmdef，单独编译；
            //- 3rdParty/Hydrogen：有 asmdef，本工具用 GetHydrogenFiles 单独编译；
            //- Architeture/LiteMVC、Architeture/PureMVC：各自有 asmdef；
            //- Integrations：资源适配器，各自有 asmdef，由 adapters 模式单独编译。
            //注意：KCP / RUDP / SUDP / Telepathy 等第三方网络库已移出 Runtime（现在位于 Assets/CosmosLibs），
            //不参与本程序集编译，因此这里不再需要为它们保留排除项。
            var excludes = new[]
            {
                Path.Combine(runtimeRoot, "Base", "Async"),
                Path.Combine(runtimeRoot, "Base", "ResourceAbstractions"),
                Path.Combine(runtimeRoot, "3rdParty", "Hydrogen"),
                Path.Combine(runtimeRoot, "Architeture", "LiteMVC"),
                Path.Combine(runtimeRoot, "Architeture", "PureMVC"),
                Path.Combine(runtimeRoot, "Integrations"),
            };
            var files = Directory.GetFiles(runtimeRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !excludes.Any(e => f.StartsWith(e + Path.DirectorySeparatorChar) || f.StartsWith(e + "/")))
                .Where(f => !IsStubbedOut(f))
                .OrderBy(f => f);
            return files;
        }

        static IEnumerable<string> GetEditorFallbackFiles()
        {
            var fallbackRoot = Path.Combine(AssetsPath, "Editor", "ResourceFallback");
            if (!Directory.Exists(fallbackRoot))
                return new string[0];
            return Directory.GetFiles(fallbackRoot, "*.cs", SearchOption.AllDirectories).OrderBy(f => f);
        }

        static IEnumerable<string> GetEditorFiles()
        {
            var editorRoot = Path.Combine(AssetsPath, "Editor");
            // ResourceFallback 有自己的 asmdef（Cosmos.Resource.EditorFallback），单独编译
            var fallbackRoot = Path.Combine(editorRoot, "ResourceFallback");
            return Directory.GetFiles(editorRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.StartsWith(fallbackRoot + Path.DirectorySeparatorChar) && !f.StartsWith(fallbackRoot + "/"))
                .OrderBy(f => f);
        }

        /// <summary>
        /// 因目标框架(BCl)不支持而被桩替换的运行时文件：
        /// <para>Utility.Encryption.cs 使用 System.Runtime.Remoting（netstandard2.1 无此命名空间），检查时以桩代替。</para>
        /// </summary>
        static bool IsStubbedOut(string filePath)
        {
            var name = Path.GetFileName(filePath);
            return name == "Utility.Encryption.cs";
        }
        #endregion
    }
}
