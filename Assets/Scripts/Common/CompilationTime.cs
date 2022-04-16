using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Common
{
    public class CompilationTime : MonoBehaviour
    {
        private readonly Dictionary<string, float> _assemblyCompilationTimes = new();
        private readonly Stopwatch _stopwatch = new();

        private bool _logsEnabled;

        [Button(ButtonSizes.Medium)]
        private void EnableCompilationLogs()
        {
            if (_logsEnabled)
            {
                return;
            }
            
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            CompilationPipeline.compilationFinished += OnCompilationFinished;

            _logsEnabled = true;
        }
        
        [Button(ButtonSizes.Medium)]
        private void DisableCompilationLogs()
        {
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished -= OnAssemblyCompilationFinished;
            CompilationPipeline.compilationFinished -= OnCompilationFinished;

            _logsEnabled = false;
        }

        private void OnCompilationStarted(object context)
        {
            _stopwatch.Start();
        }

        private void OnAssemblyCompilationFinished(string value, CompilerMessage[] messages)
        {
            _assemblyCompilationTimes.Add(value, _stopwatch.ElapsedMilliseconds / 1000f);
        }

        private void OnCompilationFinished(object context)
        {
            var elapsed = _stopwatch.Elapsed;

            _stopwatch.Stop();
            _stopwatch.Reset();

            foreach (var pair in _assemblyCompilationTimes)
            {
                Debug.Log($"Assembly {pair.Key.Replace("Library/ScriptAssemblies/", string.Empty)} " +
                          $"built after {pair.Value:F} seconds.");
            }

            Debug.Log($"Total compilation time: {elapsed.TotalSeconds:F} seconds.");
        }
    }
}
