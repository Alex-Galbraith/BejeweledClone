using UnityEngine;
using System.Collections;

namespace TSwapper { 
    /// <summary>
    /// Creates sets of particle pools.
    /// </summary>
    public class EffectManager : MonoBehaviour {
        private EffectPool[] pools;
        public EffectSet set;
        private void Awake() {
            set.Init();
            pools = new EffectPool[System.Enum.GetValues(typeof(VFXType)).Length];
            for (int i = 1; i< pools.Length; i++) {
                pools[i] = new EffectPool(this.transform, set[i]);
            }
        }

        public ParticleSystem GetEffect(VFXType t) {
            if ((int) t == 0) {
                return null;
            }
            return pools[(int)t].GetEffect().System;
        }
    }
}