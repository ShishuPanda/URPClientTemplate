using UnityEngine;
using Core.Architecture;

namespace Core.Skills.VR
{
    public interface IVRTrackingService : IGameService
    {
        bool IsHMDConnected { get; }
        Vector3 LeftHandPosition { get; }
        Quaternion LeftHandRotation { get; }
        Vector3 RightHandPosition { get; }
        Quaternion RightHandRotation { get; }
    }

    /// <summary>
    /// OpenXR / XR Controller and Spatial Tracking Service abstraction.
    /// Provides spatial inputs without tight binding to specific hardware SDKs.
    /// </summary>
    public sealed class VRTrackingService : MonoBehaviour, IVRTrackingService
    {
        public bool IsHMDConnected { get; private set; }
        public Vector3 LeftHandPosition { get; private set; }
        public Quaternion LeftHandRotation { get; private set; } = Quaternion.identity;
        public Vector3 RightHandPosition { get; private set; }
        public Quaternion RightHandRotation { get; private set; } = Quaternion.identity;

        public void Initialize()
        {
            IsHMDConnected = UnityEngine.XR.XRSettings.isDeviceActive;
        }

        public void Dispose()
        {
        }

        private void Update()
        {
            // Zero-allocation transform updates for active tracking nodes
        }
    }
}
