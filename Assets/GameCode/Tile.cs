using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper { 
    /// <summary>
    /// Tile superclass. Since this is a simple project, all of the tile
    /// data and logic is stored here.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour
    {
        public TileType tileType;
        /// <summary>
        /// Is this a basic matching tile, or do we need to treat it with
        /// greater care. E.G., do we need to call PostFlip
        /// </summary>
        [Tooltip("Is this a basic matching tile, or do we need to treat it with greater care?")]
        public bool isComplex = false;

        /// <summary>
        /// This particle will be automatically triggered on death.
        /// </summary>
        [Tooltip("This particle will be automatically triggered on death.")]
        public ParticleSystem onDeathParticle;

        /// <summary>
        /// How much is destroying this tile worth?
        /// </summary>
        [Tooltip("How much is destroying this tile worth?")]
        public int baseScoreValue;

        SpriteRenderer spriteRenderer;

        /// <summary>
        /// Only called if isComplex is true. Use this function to perform special actions after
        /// this tile has been flipped. We could use events for this, but it would be a little 
        /// over the top for this project.
        /// </summary>
        public void PostFlip() {

        }

        /// <summary>
        /// Only called if isComplex is true. Use this function to perform special actions after
        /// a neighbouring tile has been flipped. We could use events for this, but it would be a little 
        /// over the top for this project.
        /// </summary>
        public void NeighbourFlipped() {

        }
    }

   
}