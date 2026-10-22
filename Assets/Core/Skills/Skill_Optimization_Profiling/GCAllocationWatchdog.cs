using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Core.Skills.Profiling
{
    /// <summary>
    /// Watchdog component tracking garbage collector allocations in debug builds.
    /// Alerts when per-frame allocations exceed the designated budget.
    /// </summary>
    public sealed class GCAllocationWatchdog : MonoBehaviour
    {
        [SerializeField] private long alertThresholdBytes = 1024; // 1 KB threshold
        private long _previousAllocatedMemory;

        private void Start()
        {
            _previousAllocatedMemory = Profiler.GetMonoUsedSizeLong();
        }

        private void LateUpdate()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var currentAllocated = Profiler.GetMonoUsedSizeLong();
            var delta = currentAllocated - _previousAllocatedMemory;

            if (delta > alertThresholdBytes)
            {
                Debug.LogWarning($"[GCAllocationWatchdog] High GC Allocation detected: {delta} bytes in frame {Time.frameCount}!");
            }

            _previousAllocatedMemory = currentAllocated;
#endif
        }
    }
}
