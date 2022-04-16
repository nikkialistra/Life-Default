using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Common.Editor
{
    // [InitializeOnLoad]
    public static class CompilationTime
    {
        private static readonly Dictionary<string, float> AssemblyCompilationTimes = new();
        private static readonly Stopwatch Stopwatch = new();

        static CompilationTime()
        {
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
        }
        
        private static void OnCompilationStarted(object context)
        {
            Stopwatch.Start();
        }

        private static void OnAssemblyCompilationFinished(string value, CompilerMessage[] messages)
        {
            AssemblyCompilationTimes.Add(value, Stopwatch.ElapsedMilliseconds / 1000f);
        }

        private static void OnCompilationFinished(object context)
        {
            var elapsed = Stopwatch.Elapsed;

            Stopwatch.Stop();
            Stopwatch.Reset();

            foreach (var pair in AssemblyCompilationTimes)
            {
                Debug.Log($"Assembly {pair.Key.Replace("Library/ScriptAssemblies/", string.Empty)} " +
                          $"built after {pair.Value:F} seconds.");
            }

            Debug.Log($"Total compilation time: {elapsed.TotalSeconds:F} seconds.");
        }
    }
}
