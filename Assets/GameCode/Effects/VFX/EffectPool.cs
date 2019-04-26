#define SAFE_MODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper {
    /// <summary>
    /// Particle effect pool.
    /// </summary>
    public class EffectPool {
        private ParticleSystemWrapper prefab;
        private List<ParticleSystemWrapper> effects = new List<ParticleSystemWrapper>();

        private Transform parent;

        public EffectPool(Transform parent, ParticleSystemWrapper p) {
            this.parent = parent;
            prefab = p;
        }

        /// <summary>
        ///Get an effect from the pool.
        ///</summary>
        public ParticleSystemWrapper GetEffect() {
            if (effects.Count == 0) {
                var eff = GameObject.Instantiate<ParticleSystemWrapper>(prefab, parent);
                eff.pool = this;
                return eff;
            }
            var ps = effects[effects.Count - 1];
            effects.RemoveAt(effects.Count - 1);
            ps.gameObject.SetActive(true);
            return ps;
        }

        /// <summary>
        /// Returns an effect to the pool, deactivates, stops, and clears it.
        /// </summary>
        /// <param name="ps">Effect to return.</param>
        public void ReturnEffect(ParticleSystemWrapper ps) {
#if SAFE_MODE
            if (effects.Contains(ps))
                throw new System.InvalidOperationException("System already in pool.");
#endif
            ps.System.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effects.Add(ps);
            ps.gameObject.SetActive(false);
        }
    }
}
