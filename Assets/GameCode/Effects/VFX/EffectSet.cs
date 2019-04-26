using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TSwapper {
    /// <summary>
    /// Hold a mapping from <see cref="VFXType"/> to <see cref="ParticleSystemWrapper"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Effect Set", fileName = "Effect Set")]
    public class EffectSet : ScriptableObject
    {

        //We use a struct array in place of a dictionary in the inspector, since the inspector
        //does not support displaying dictionaries.
        [System.Serializable]
        private struct EffectToPS {
            public VFXType type;
            public ParticleSystemWrapper system;
        }
        [SerializeField]
        private EffectToPS[] Effects;

        private Dictionary<VFXType, ParticleSystemWrapper> dict;

        //Hack to convert inspector "dictionary" to real dictionary.
        public void Init() {
            dict = new Dictionary<VFXType, ParticleSystemWrapper>();
            foreach (var v in Effects) {
                dict.Add(v.type, v.system);
            }
        }

        public ParticleSystemWrapper this[int i] {
            get { return dict[(VFXType)i]; }
        }

        public ParticleSystemWrapper this[VFXType i] {
            get { return dict[i]; }
        }
    }
}