using UnityEngine;
using System.Collections;
namespace TSwapper {
    /// <summary>
    /// "Wrapper" for ParticleSystems that automatically returns the ParticleEffect to its <see cref="EffectPool"/> when the 
    /// ParticleSystem performs a callback.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemWrapper : MonoBehaviour {
        private ParticleSystem system;
        internal EffectPool pool;

        public ParticleSystem System { get => system; private set => system = value; }

        void Awake() {
            System = GetComponent<ParticleSystem>();
        }

        public void OnParticleSystemStopped() {
            pool?.ReturnEffect(this);
        }
    }
}
