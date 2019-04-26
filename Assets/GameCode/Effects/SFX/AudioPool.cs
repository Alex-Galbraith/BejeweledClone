using UnityEngine;
using System.Collections.Generic;

namespace TSwapper { 
    /// <summary>
    /// Holds references to AudioSources, endexed by Enum.
    /// </summary>
    public class AudioPool : MonoBehaviour {
        public enum SFXType {
            None,
            Chink,
            Shwing
        }

        //We use a struct array in place of a dictionary in the inspector, since the inspector
        //does not support displaying dictionaries.
        [System.Serializable]
        private struct EffectToPS {
            public SFXType type;
            public AudioSource source;
        }
        [SerializeField]
        private EffectToPS[] Effects;

        private Dictionary<SFXType, AudioSource> dict;

        //Hack to convert inspector "dictionary" to real dictionary.
        public void Awake() {
            dict = new Dictionary<SFXType, AudioSource>();
            foreach (var v in Effects) {
                dict.Add(v.type, v.source);
            }
        }

        public AudioSource this[int i] {
            get { return dict[(SFXType)i]; }
        }

        public AudioSource this[SFXType i] {
            get { return dict[i]; }
        }
    }
}