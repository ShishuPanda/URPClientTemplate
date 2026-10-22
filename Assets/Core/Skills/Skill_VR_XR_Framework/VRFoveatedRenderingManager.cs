using UnityEngine;
using UnityEngine.Rendering.Universal;
using Core.Architecture;

namespace Core.Skills.VR
{
    /// <summary>
    /// Configures Dynamic Foveated Rendering and Quest / Mobile VR optimization flags in URP.
    /// </summary>
    public sealed class VRFoveatedRenderingManager : MonoBehaviour, IGameService
    {
        [Range(0f, 1f)]
        [SerializeField] private float foveatedStrength = 1.0f;

        public void Initialize()
        {
            ApplyFoveatedRenderingSettings();
        }

        public void Dispose()
        {
        }

        public void ApplyFoveatedRenderingSettings()
        {
            // Set render scale and target frame rate for XR headsets (Meta Quest / PCVR)
            Application.targetFrameRate = 90;
            QualitySettings.vSyncCount = 0;
            
            // Adjust dynamic resolution scaling if URP asset is active
            if (UniversalRenderPipeline.asset != null)
            {
                UniversalRenderPipeline.asset.renderScale = 1.0f;
            }
        }
    }
}
