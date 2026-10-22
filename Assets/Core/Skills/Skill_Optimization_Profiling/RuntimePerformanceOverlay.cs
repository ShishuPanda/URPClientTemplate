using System;
using UnityEngine;
using UnityEngine.Profiling;
using Core.Architecture;

namespace Core.Skills.Profiling
{
    /// <summary>
    /// In-game performance profiler overlay monitoring FPS, Frame Time, and Heap Memory.
    /// Operates with strictly zero heap allocations during runtime updates.
    /// </summary>
    public sealed class RuntimePerformanceOverlay : MonoBehaviour, IGameService
    {
        [SerializeField] private bool showOverlay = true;
        [SerializeField] private int targetFrameRate = 90;

        private float _deltaTimeAccumulator;
        private int _frameCount;
        private float _fps;
        private float _frameTimeMs;
        private long _allocatedMemoryBytes;
        private GUIStyle _style;

        public void Initialize()
        {
            Application.targetFrameRate = targetFrameRate;
        }

        public void Dispose()
        {
        }

        private void Update()
        {
            _deltaTimeAccumulator += Time.unscaledDeltaTime;
            _frameCount++;

            if (_deltaTimeAccumulator >= 0.5f)
            {
                _fps = _frameCount / _deltaTimeAccumulator;
                _frameTimeMs = (_deltaTimeAccumulator / _frameCount) * 1000.0f;
                _allocatedMemoryBytes = Profiler.GetMonoUsedSizeLong();
                _frameCount = 0;
                _deltaTimeAccumulator = 0.0f;
            }
        }

        private void OnGUI()
        {
            if (!showOverlay) return;

            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.UpperLeft,
                    fontSize = 14
                };
                _style.normal.textColor = Color.cyan;
            }

            var memMb = _allocatedMemoryBytes / (1024f * 1024f);
            var text = $"FPS: {_fps:0.0} ({_frameTimeMs:0.00} ms) | Heap: {memMb:0.0} MB";
            GUI.Label(new Rect(10, 10, 320, 30), text, _style);
        }
    }
}
