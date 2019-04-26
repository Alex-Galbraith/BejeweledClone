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
        [EnumFlags]
        public TileType tileType;
        public PoolGroup poolGroup;
        private MaterialPropertyBlock mpb;

        /// <summary>
        /// What tiles does this tile match with? If tile A matches with Tile B,
        /// but B not with A, then the match is successful.
        /// </summary>
        [Tooltip("What tiles does this tile match with?")]
        [EnumFlags]
        public TileType matchesWith;
        /// <summary>
        /// Is this a basic tile, or do we need to treat it with
        /// greater care. E.G., do we need to call PostFlip
        /// </summary>
        public bool isComplex = false;

        /// <summary>
        /// This particle will be automatically triggered on death. Only plays if isComplex.
        /// </summary>
        [Tooltip("This particle will be automatically triggered on death.")]
        public VFXType onDeathParticle;

        /// <summary>
        /// This sound will be automatically played on death. Only plays if isComplex.
        /// </summary>
        [Tooltip("This sound will be automatically played on death.")]
        public AudioPool.SFXType onDeathSound;

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

        
        public SpriteRenderer spriteRenderer;

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
        //Auto hook up spriteRenderer
        private void OnValidate() {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif

        private void Awake() {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            mpb = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Only called if isComplex is true. Use this function to perform special actions after
        /// this tile has been flipped. We could use events for this, but it would be a little 
        /// over the top for this project.
        /// </summary>
        public virtual void PostFlip(TileManager tm) {

        }

        /// <summary>
        /// This tile has been matched! Spawn the particle effect, animate out, and destroy. Only called if isComplex is true.
        /// </summary>
        public virtual void OnMatched(TileManager tm) {

        }

        /// <summary>
        /// Used for making the tile gleam.
        /// </summary>
        /// <param name="gleam"></param>
        public void SetGleaming(bool gleam) {
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_EffectToggle", gleam?1:0);
            spriteRenderer.SetPropertyBlock(mpb);
        }

        /// <summary>
        /// Populates the specified tile with data from the current tile.
        /// Used by the TilePool.
        /// </summary>
        /// <param name="to"></param>
        public virtual void PopulateTile(Tile to) {
            if (this.poolGroup != to.poolGroup) {
                throw new System.ArgumentException("Cannot populate " + to.poolGroup + " from a " + this.poolGroup + ". Poolgroups must match.");
            }
            to.gameObject.name = gameObject.name;
            to.transform.rotation = transform.rotation;
            to.transform.localScale = transform.localScale;
            //Copy sprite renderer
            to.spriteRenderer.sprite = this.spriteRenderer.sprite;
            to.spriteRenderer.size = this.spriteRenderer.size;
            to.spriteRenderer.sharedMaterial = this.spriteRenderer.sharedMaterial;
            to.spriteRenderer.color = this.spriteRenderer.color;
            to.spriteRenderer.sortingLayerID = this.spriteRenderer.sortingLayerID;
            to.spriteRenderer.sortingOrder = this.spriteRenderer.sortingOrder;
            to.spriteRenderer.sharedMaterial = this.spriteRenderer.sharedMaterial;
            to.SetGleaming(false);
            //copy tile data
            to.tileType = this.tileType;
            to.baseScoreValue = this.baseScoreValue;
            to.isComplex = this.isComplex;
            to.matchesWith = this.matchesWith;
            to.onDeathParticle = this.onDeathParticle;
        }

        /// <summary>
        /// Populates a tile facade with data from a tile.
        /// </summary>
        public static void PopulateFacade(TileFacade to, Tile u) {
            to.spriteRenderer.sprite = u.spriteRenderer.sprite;
            to.spriteRenderer.size = u.spriteRenderer.size;
            to.spriteRenderer.sharedMaterial = u.spriteRenderer.sharedMaterial;
            to.spriteRenderer.color = u.spriteRenderer.color;

            to.gameObject.name = "facade_"+u.gameObject.name;
            to.transform.rotation = u.transform.rotation;
            to.transform.localScale = u.transform.localScale;
            to.transform.position = u.transform.position;
        }
    }

   
}