using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace RoslynCheck
{
    /// <summary>
    /// 使用 Roslyn (Microsoft.CodeAnalysis.CSharp) 按 Unity asmdef 边界编译 CosmosFramework 程序集。
    /// <para>用法：dotnet run --project tools/RoslynCheck -- [core|runtime|editor]</para>
    /// <para>core    —— 仅 Cosmos.Resource.Core（独立核心库）</para>
    /// <para>runtime —— Cosmos.Resource.Core + Cosmos.Runtime（全量运行时，含全部模块）</para>
    /// <para>editor  —— 上述两者 + Cosmos.Editor（全量编辑器）</para>
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

            // Cosmos.Resource.Core —— 独立核心库
            var coreTrees = LoadFiles(GetCoreFiles(), parseOptions);
            var coreReferences = new List<MetadataReference>(references);
            coreReferences.Add(stubCompilation.ToMetadataReference());
            var coreCompilation = CSharpCompilation.Create(
                "Cosmos.Resource.Core",
                coreTrees,
                coreReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("Cosmos.Resource.Core", coreCompilation, dumpPath);

            if (mode == "core")
                return errors == 0 ? 0 : 1;

            // Cosmos.Runtime —— 全量运行时（引用 Core）
            var runtimeTrees = LoadFiles(GetRuntimeFiles(), parseOptions);
            // 框架级桩（因目标框架不支持而替换的运行时文件 + BCL 兼容扩展）编译进 Runtime 程序集
            runtimeTrees.AddRange(LoadFiles(GetFrameworkStubFiles(), parseOptions));
            var runtimeReferences = new List<MetadataReference>(references);
            runtimeReferences.Add(stubCompilation.ToMetadataReference());
            runtimeReferences.Add(coreCompilation.ToMetadataReference());
            var runtimeCompilation = CSharpCompilation.Create(
                "Cosmos.Runtime",
                runtimeTrees,
                runtimeReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            errors += Report("Cosmos.Runtime", runtimeCompilation, dumpPath);

            if (mode == "runtime")
                return errors == 0 ? 0 : 1;

            // Cosmos.Editor —— 全量编辑器（引用 Core + Runtime）
            var editorTrees = LoadFiles(GetEditorFiles(), parseOptions);
            var editorReferences = new List<MetadataReference>(runtimeReferences);
            editorReferences.Add(runtimeCompilation.ToMetadataReference());
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

        static IEnumerable<string> GetCoreFiles()
        {
            return Directory.GetFiles(Path.Combine(AssetsPath, "Runtime", "Base", "ResourceCore"), "*.cs", SearchOption.AllDirectories)
                .OrderBy(f => f);
        }

        static IEnumerable<string> GetRuntimeFiles()
        {
            var runtimeRoot = Path.Combine(AssetsPath, "Runtime");
            var excludes = new[]
            {
                Path.Combine(runtimeRoot, "Base", "ResourceCore"),
                Path.Combine(runtimeRoot, "3rdParty", "Hydrogen"),
                Path.Combine(runtimeRoot, "Architeture", "LiteMVC"),
                Path.Combine(runtimeRoot, "Architeture", "PureMVC"),
                Path.Combine(runtimeRoot, "Modules", "Network", "KCP"),
                Path.Combine(runtimeRoot, "Modules", "Network", "RUDP"),
                Path.Combine(runtimeRoot, "Modules", "Network", "SUDP"),
                Path.Combine(runtimeRoot, "Modules", "Network", "Telepathy"),
            };
            var files = Directory.GetFiles(runtimeRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !excludes.Any(e => f.StartsWith(e + Path.DirectorySeparatorChar) || f.StartsWith(e + "/")))
                .Where(f => !IsStubbedOut(f))
                .OrderBy(f => f);
            return files;
        }

        static IEnumerable<string> GetEditorFiles()
        {
            var editorRoot = Path.Combine(AssetsPath, "Editor");
            return Directory.GetFiles(editorRoot, "*.cs", SearchOption.AllDirectories)
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
