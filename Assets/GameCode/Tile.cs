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
        public PoolGroup poolGroup;

        /// <summary>
        /// What tiles does this tile match with? If tile A matches with Tile B,
        /// but B not with A, then the match is successful.
        /// </summary>
        [Tooltip("What tiles does this tile match with?")]
        [EnumFlags]
        public TileType matchesWith;
        /// <summary>
        /// Is this a basic matching tile, or do we need to treat it with
        /// greater care. E.G., do we need to call PostFlip
        /// </summary>
        public bool isComplex = false;

        /// <summary>
        /// Is this a basic matching tile, or does it have special
        /// match conditions. Will call CheckMatch if true;
        /// </summary>
        public bool isComplexMatch = false;

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

        /// <summary>
        /// Multiplies the score of all broken tiles by this number.
        /// </summary>
        [Tooltip("Multiplies the score of all broken tiles by this number.")]
        public float scoreMultiplier = 1;

        [SerializeField]
        SpriteRenderer spriteRenderer;

        /// <summary>
        /// Used for tracking position in TileGrid
        /// </summary>
        internal Vector2Int GridPos {
            get;
            set;
        }

        /// <summary>
        /// Used for tracking whether the tile exists in a TileGrid
        /// </summary>
        internal bool InGrid {
            get;
            set;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif

        private void Awake() {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Check if this block has a match. Populates the specified array with
        /// the matching tiles. Array must have space for all matched tiles.
        /// </summary>
        /// <param name="t">Tilegrid we are checking.</param>
        /// <param name="array">Array to populate with matched tiles. Must have space for all
        /// matched tiles. Non allocating.</param>
        /// <returns>Number of tiles matched</returns>
        public int CheckMatch(TileGrid t, Tile[] array) {
            return 0;
        }

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

        /// <summary>
        /// This tile has been matched! Spawn the particle effect, animate out, and destroy.
        /// </summary>
        public void OnMatched() {

        }

        /// <summary>
        /// Populates the specified tile with data from the current tile.
        /// Used by the TilePool.
        /// </summary>
        /// <param name="to"></param>
        public void PopulateTile(Tile to) {
            if (this.poolGroup != to.poolGroup) {
                throw new System.ArgumentException("Cannot populate " + to.poolGroup + " from a " + this.poolGroup + ". Poolgroups must match.");
            }
            to.gameObject.name = gameObject.name;
            //Copy sprite renderer
            to.spriteRenderer.sprite = this.spriteRenderer.sprite;
            to.spriteRenderer.size = this.spriteRenderer.size;
            to.spriteRenderer.sharedMaterial = this.spriteRenderer.sharedMaterial;
            to.spriteRenderer.color = this.spriteRenderer.color;
            to.spriteRenderer.sortingLayerID = this.spriteRenderer.sortingLayerID;
            to.spriteRenderer.sortingOrder = this.spriteRenderer.sortingOrder;
            //copy tile data
            to.tileType = this.tileType;
            to.baseScoreValue = this.baseScoreValue;
            to.isComplex = this.isComplex;
            to.isComplexMatch = this.isComplexMatch;
            to.matchesWith = this.matchesWith;
            to.onDeathParticle = this.onDeathParticle;
        }
    }

   
}