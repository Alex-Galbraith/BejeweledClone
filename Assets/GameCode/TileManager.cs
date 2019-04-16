using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TSwapper { 
    public class TileManager : MonoBehaviour
    {
        private TilePool tilePool;
        public TileGrid tileGrid;
        
        public struct TileSpawnData {
            public Tile t;
            public float spawnChance;
        };

        public TileSpawnData[] spawnables;

        // Start is called before the first frame update
        void Awake()
        {
            tilePool = new TilePool(this.gameObject);
        }

        public void SpawnTile(Tile prefab, int x, int y) {
            Tile t = tilePool.GetTile(prefab);
            tileGrid.SetTile(x, y, t);
            t.gameObject.transform.position = tileGrid.GetWorldspaceTilePos(x, y).center;
            t.gameObject.SetActive(true);
        }
    }
}
