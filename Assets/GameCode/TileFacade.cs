using UnityEngine;
using System.Collections;
namespace TSwapper {
    /// <summary>
    /// Fake tile used for special effects
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileFacade : MonoBehaviour {
        public SpriteRenderer spriteRenderer;
    }
}