#define SAFE_MODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TSwapper { 
    public class TilePool
    {
        //Array of pools indexed by pool type
        private List<Tile>[] tilePools;
        private int poolCount;
        GameObject tileParent;

        /// <summary>
        /// Create a new TilePool.
        /// </summary>
        /// <param name="tileParent">Parent object for spawned tiles for organization purposes.</param>
        public TilePool(GameObject tileParent) {
            this.tileParent = tileParent;
            poolCount = System.Enum.GetValues(typeof(PoolGroup)).Length;
            tilePools = new List<Tile>[poolCount];
            for (int i = 0; i < poolCount; i++) {
                tilePools[i] = new List<Tile>();
            }
        }

        /// <summary>
        /// Gets a tile matching the specified prefab from the pool. If no 
        /// suitable tile is free, one will be created.
        /// </summary>
        /// <param name="prefab">Type of tile to spawn.</param>
        /// <returns>A tile of the specified type.</returns>
        public Tile GetTile(Tile prefab) {
            List<Tile> list = tilePools[(int)prefab.poolGroup];
            //we have no tiles of that type, create a new one
            if (list.Count == 0) {
                Tile newTile = GameObject.Instantiate<Tile>(prefab, tileParent.transform);
                return newTile;
            }
            Tile t = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            prefab.PopulateTile(t);
            return t;
        }

        /// <summary>
        /// Returns a tile to the pool, making it innactive in the scene.
        /// </summary>
        /// <param name="t">Tile to return.</param>
        public void ReturnTile(Tile t) {
            if (t == null)
                return;
            t.gameObject.SetActive(false);

#if SAFE_MODE
            //Check we arent adding duplicates
            if (tilePools[(int)t.poolGroup].Contains(t)) {
                throw new System.InvalidOperationException("Object already in pool.");
            }
#endif
            tilePools[(int)t.poolGroup].Add(t);
            t.InGrid = false;
        }
    }
}