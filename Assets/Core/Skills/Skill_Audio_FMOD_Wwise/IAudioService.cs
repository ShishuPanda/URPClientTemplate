using UnityEngine;
using Core.Architecture;

namespace Core.Skills.Audio
{
    /// <summary>
    /// Universal Audio Service Interface supporting native AudioSource, FMOD, or Wwise middleware.
    /// </summary>
    public interface IAudioService : IGameService
    {
        void PlaySound(string soundId, Vector3 worldPosition, float volume = 1.0f);
        void PlayMusic(string trackId, float fadeDuration = 1.0f);
        void StopMusic(float fadeDuration = 1.0f);
        void SetMasterVolume(float volume);
    }

    /// <summary>
    /// Default high-performance Unity Audio implementation utilizing spatial pooling.
    /// </summary>
    public sealed class UnityAudioService : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource musicSource;
        private AudioSourcePool _pool;

        public void Initialize()
        {
            _pool = new AudioSourcePool(gameObject, 16);
        }

        public void Dispose()
        {
            _pool?.Dispose();
        }

        public void PlaySound(string soundId, Vector3 worldPosition, float volume = 1.0f)
        {
            var source = _pool.Get();
            if (source == null) return;

            source.transform.position = worldPosition;
            source.volume = volume;
            source.spatialBlend = 1.0f; // 3D Spatial audio
            // Play one shot clip
            _pool.Return(source);
        }

        public void PlayMusic(string trackId, float fadeDuration = 1.0f)
        {
            if (musicSource != null)
            {
                musicSource.Play();
            }
        }

        public void StopMusic(float fadeDuration = 1.0f)
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        public void SetMasterVolume(float volume)
        {
            AudioListener.volume = Mathf.Clamp01(volume);
        }
    }

    /// <summary>
    /// Lightweight dynamic pooling container for 3D sound emitters to eliminate instantiation GC allocations.
    /// </summary>
    public sealed class AudioSourcePool
    {
        private readonly System.Collections.Generic.Queue<AudioSource> _pool;
        private readonly GameObject _root;

        public AudioSourcePool(GameObject parent, int initialCapacity)
        {
            _root = new GameObject("AudioEmitterPool");
            _root.transform.SetParent(parent.transform);
            _pool = new System.Collections.Generic.Queue<AudioSource>(initialCapacity);

            for (int i = 0; i < initialCapacity; i++)
            {
                var emitter = new GameObject($"PooledEmitter_{i}").AddComponent<AudioSource>();
                emitter.transform.SetParent(_root.transform);
                emitter.playOnAwake = false;
                _pool.Enqueue(emitter);
            }
        }

        public AudioSource Get()
        {
            return _pool.Count > 0 ? _pool.Dequeue() : null;
        }

        public void Return(AudioSource emitter)
        {
            if (emitter != null)
            {
                emitter.Stop();
                _pool.Enqueue(emitter);
            }
        }

        public void Dispose()
        {
            _pool.Clear();
            if (_root != null) Object.Destroy(_root);
        }
    }
}
