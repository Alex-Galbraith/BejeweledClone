using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TSwapper { 
    public class TileManager : MonoBehaviour
    {
        private TilePool tilePool;
        public TileGrid tileGrid;
        
        [System.Serializable]
        public struct TileSpawnData {
            public Tile t;
            [Range(0,1)]
            public float spawnChance;
        };

        /// <summary>
        /// Used for temporary processing jobs such as flood fills.
        /// </summary>
        private byte[,] tempData;

        private void ClearTemp() {
            System.Array.Clear(tempData, 0, tempData.Length);
        }

        private void OnValidate() {
            float sum = 0.001f;
            for (int i = 0; i < spawnables.Length; i++) {
                sum += spawnables[i].spawnChance;
            }
            for (int i = 0; i < spawnables.Length; i++) {
                spawnables[i].spawnChance = Mathf.Min(spawnables[i].spawnChance/sum,1);
            }
        }

        public TileSpawnData[] spawnables;

        // Start is called before the first frame update
        void Awake()
        {
            tilePool = new TilePool(this.gameObject);
            tempData = new byte[tileGrid.dimensions.x, tileGrid.dimensions.y];
        }

        private void Start() {
            PopulateAll();
        }

        /// <summary>
        /// Spawn a tile of the specified type at the given location.
        /// Uses tiles from TilePool.
        /// </summary>
        /// <param name="prefab">Type of tile to spawn.</param>
        /// <param name="x">X position to spawn at.</param>
        /// <param name="y">Y position to spawn at.</param>
        /// <returns>The spawned tile instance.</returns>
        public Tile SpawnTile(Tile prefab, int x, int y) {
            Tile t = tilePool.GetTile(prefab);
            tileGrid.SetTile(x, y, t);
            t.gameObject.transform.position = tileGrid.GetWorldspaceTilePos(x, y).center;
            t.gameObject.SetActive(true);
            return t;
        }

        /// <summary>
        /// Check if the tile at the specified location completes a match.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>True if a match is found, otherwise false</returns>
        public bool CheckMatchTile(int x, int y) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Check if the row at the specified location contains matches. Populates array
        /// with ONE tile from each group. 
        /// </summary>
        /// <param name="y">Y position</param>
        /// <param name="array"></param>
        /// <returns>Number of matching groups found</returns>
        public int CheckMatchRow(int y, Tile[] array) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Check if the column at the specified location contains matches. Populates array
        /// with ONE tile from each group. 
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="array"></param>
        /// <returns>Number of matching groups found</returns>
        public int CheckMatchColumn(int x, Tile[] array) {
            throw new System.NotImplementedException();
        }

        public void PopulateAll() {
            for (int dx = 0; dx < tileGrid.dimensions.x; dx++) {
                for (int dy = 0; dy < tileGrid.dimensions.y; dy++) {
                    Tile prefab = spawnables[Random.Range(0, spawnables.Length)].t;
                    SpawnTile(prefab, dx, dy);
                }
            }
        }

        /// <summary>
        /// Cardinal direction array for itteration.
        /// </summary>
        private Vector2Int[] Cardinals = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

        private void EnqueueNeighbours(Vector2Int pos, Queue<Vector2Int> queue, TileType mask) {
            foreach (var c in Cardinals) {
                //skip closed tiles
                if (tempData[pos.x + c.x, pos.y + c.y] != 0)
                    continue;
                Tile t = tileGrid.GetTile(pos.x + c.x, pos.y + c.y);

                //If the tile is not null and matches our mask, enqueue it
                if (t != null && (mask & t.matchesWith) != 0)
                    queue.Enqueue(pos + c);
            }

        }

        /// <summary>
        /// Gets all connected tiles that form a matching sequence.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public int GetConnectedMatching(int x, int y, Tile[] tiles) {
            if (!tileGrid.CheckBounds(x, y))
                return 0;
            Queue<Vector2Int> openSet = new Queue<Vector2Int>();
            int count = 0;
            //we will use our tempdata as our closed set
            ClearTemp();
            openSet.Enqueue(new Vector2Int(x, y));
            while (openSet.Count > 0 && count < tiles.Length) {
                Vector2Int o = openSet.Dequeue();
                Tile t = tileGrid.GetTile(o.x, o.y);
                //mark closed
                tempData[o.x, o.y] = 1;
                EnqueueNeighbours(o, openSet, t.matchesWith);
                count++;
            }

            return count;
        }


    }
}
