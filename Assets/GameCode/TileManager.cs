using System.Collections.Generic;
using UnityEngine;
namespace TSwapper { 
    /// <summary>
    /// Contains basic tile spawning, shifting, and matching rules.
    /// </summary>
    public class TileManager : MonoBehaviour
    {

        #region structs
        [System.Serializable]
        public struct TileSpawnData {
            public Tile t;
            [Range(0, 1)]
            public float spawnChance;
        };
        #endregion

        #region public editor
        public TileGrid tileGrid;
        public ActionQueue queue;
        public TileSpawnData[] spawnables;
        [Tooltip("How many matching in a row is required for a true match.")]
        public ushort MatchingInARowRequired = 3;
        #endregion

        #region private data
        /// <summary>
        /// The sum of tile spawn weights. Set in OnValidate and Awake.
        /// </summary>
        private float weightSum;

        /// <summary>
        /// Used for temporary processing jobs such as flood fills.
        /// </summary>
        private byte[,] tempData;

        private byte[,] matchData;

        private TilePool tilePool;

        private Tile[] tileBuffer;

        private void ClearTemp() {
            System.Array.Clear(tempData, 0, tempData.Length);
        }

        private void ClearMatch() {
            System.Array.Clear(matchData, 0, matchData.Length);
        }
        #endregion

        #region events
        public event OnTilesDestroyed TilesDestroyed;
        public delegate void OnTilesDestroyed(Tile[] tiles);

        public event OnSuccessfulMove SuccessfulMove;
        public delegate void OnSuccessfulMove();
        #endregion

        #region UnityFunctions
        private void OnValidate() {
            CalcWeights();
        }
        
        private void CalcWeights() {
            weightSum = 0;
            for (int i = 0; i < spawnables.Length; i++) {
                weightSum += spawnables[i].spawnChance;
            }
        }
        

        // Start is called before the first frame update
        void Awake() {
            tilePool = new TilePool(this.gameObject);
            tempData = new byte[tileGrid.dimensions.x, tileGrid.dimensions.y];
            matchData = new byte[tileGrid.dimensions.x, tileGrid.dimensions.y];
            tileBuffer = new Tile[tileGrid.dimensions.x * tileGrid.dimensions.y / 2];
            CalcWeights();
        }

        private void Start() {
            PopulateAll();
        }
        #endregion

        #region Tile manipulations
        /// <summary>
        /// Updates a tile's transform position to match it's grid position.
        /// </summary>
        /// <param name="t"></param>
        private void UpdateTilePos(Tile t) {
            t.transform.position = tileGrid.GetWorldspaceTilePos(t.GridPos.x, t.GridPos.y).center;
        }

        /// <summary>
        /// Repairs all columns in the grid.
        /// </summary>
        /// <param name="ID"></param>
        private void RepairAll(int ID) {
            List<Tile> toUpdate = new List<Tile>();
            for (int i = 0; i < tileGrid.dimensions.x; i++)
                toUpdate.AddRange(RepairColumn(i));
            queue.ActionComplete(ID);

            if (toUpdate.Count == 0)
                return;

            queue.Enqueue(delegate (int id) {
                StartCoroutine(TileLerpEffect.LerpPosition(id, queue, toUpdate.ToArray(), 0.3f, tileGrid, TileLerpEffect.SqrLerp));
            });
            queue.Enqueue(delegate (int id) {
                CheckMatchAndHandle(toUpdate.ToArray());
                queue.ActionComplete(id);
            });
        }
        public List<Tile> RepairColumn(int xPos, bool spawnNew = true) {
            int top = tileGrid.dimensions.y;
            int botGap = 0;
            int topGap = 0;
            int botTile, topTile;
            int gaps = 0;
            int sanity = 0;
            List<Tile> toUpdate = new List<Tile>(top);
            //find bottom of gap
            while (tileGrid.GetTile(xPos, botGap) != null) {
                botGap++;
            }
            //while we havent reached the top
            while (botGap<top && sanity++ < top) {
                topGap = botGap;
                //find top of gap
                while (tileGrid.GetTile(xPos, topGap) == null && topGap<top) {
                    topGap++;
                    gaps++;
                }
                //we have reached the top
                if (botGap == topGap) {
                    break;
                }
                //find the top of our set of tiles
                botTile = topGap;
                topTile = topGap;
                Tile t;
                while ((t = tileGrid.GetTile(xPos, topTile)) != null) {
                    topTile++;
                }
                botGap = topTile;
                if (botTile == topTile)
                    continue;
                toUpdate.AddRange(tileGrid.ShiftTiles(xPos, botTile, xPos + 1, topTile, 0, -gaps));
                
            }
            if (spawnNew) {
                for (int i = top-1; i >= top-gaps; i--) {
                    Vector2Int gpos = new Vector2Int(xPos, i);
                    
                    Tile t = SpawnTile(RandomTilePrefab(), gpos.x, gpos.y);
                    t.transform.position = t.transform.position + transform.up * i * tileGrid.tileSize.y;
                    toUpdate.Add(t);
                }
            }

            return toUpdate;
        }

        /// <summary>
        /// Get a random weighted tile
        /// </summary>
        /// <returns></returns>
        private Tile RandomTilePrefab() {
            float r = Random.value  * weightSum;
            Tile prefab = null;
            for (int i = 0; i < spawnables.Length && r>0; i++) {
                prefab = spawnables[i].t;
                r -= spawnables[i].spawnChance;
            }
            return prefab;
        }

        /// <summary>
        /// Checks for matches on a tile, destroys the matching tiles, and begins the update sequence.
        /// </summary>
        /// <param name="tiles">Tiles to check.</param>
        /// <param name="checkMatch">Set to false if matching is already known for all tiles.</param>
        public void CheckMatchAndHandle(Tile[] tiles, bool checkMatch = true) {
            List<Tile> toUpdate = new List<Tile>();
            ClearTemp();
            byte MatchCount = 1;
            for (int i = 0; i < tiles.Length; i++) {
                Tile t = tiles[i];
                Vector2Int gpos = t.GridPos;
                if (checkMatch)
                    if (!CheckMatchTile(gpos.x, gpos.y))
                        continue;
                int c = GetConnectedMatching(gpos.x, gpos.y, tileBuffer, (byte)(MatchCount % 254 + 1), false);
                if (c > 0)
                    MatchCount++;
                //AddRange doesnt have an option to add subarray
                for (int j = 0; j < c; j++)
                    toUpdate.Add(tileBuffer[j]);
            }
            queue.Enqueue(delegate (int id) {
                for (int i = 0; i < toUpdate.Count; i++) {
                    DestroyTileSilent(toUpdate[i].GridPos.x, toUpdate[i].GridPos.y);
                }
                queue.ActionComplete(id);
                TilesDestroyed(toUpdate.ToArray());
            });
            queue.Enqueue(RepairAll);
            
        }
       
        /// <summary>
        /// Attempt to swap two tiles. Successfull if a match is made. Sets off tile destruction sequences.
        /// </summary>
        /// <returns>True if the swap is successful, otherwise false.</returns>
        internal bool TrySwapTiles(Vector2Int a, Vector2Int b) {
            tileGrid.SwapTiles(a.x, a.y, b.x, b.y);
            bool valid = false;
            bool matchA = false;
            bool matchB = false;
            valid |= matchA = CheckMatchTile(a.x, a.y);
            valid |= matchB = CheckMatchTile(b.x, b.y);
            if (!valid)
                tileGrid.SwapTiles(a.x, a.y, b.x, b.y);
            else {
                //invoke event
                SuccessfulMove?.Invoke();
                queue.Enqueue(delegate (int id) {
                    StartCoroutine(TileLerpEffect.LerpPosition(id, queue, new Tile[] { tileGrid.GetTile(a.x, a.y), tileGrid.GetTile(b.x, b.y) }, 0.2f, tileGrid, TileLerpEffect.EaseInOutLerp));
                });
                //This doubles up on checking, but the code reuse is worth it
                queue.Enqueue(delegate (int id) {
                    CheckMatchAndHandle(new Tile[] { tileGrid.GetTile(a.x, a.y), tileGrid.GetTile(b.x, b.y) });
                    queue.ActionComplete(id);
                });
            }
            return valid;
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
            int matchCount = 1;
            //check rows
            for (int i = -MatchingInARowRequired+1; i < MatchingInARowRequired-1; i++) {
                Tile left   = tileGrid.GetTile(x + i, y);
                Tile right  = tileGrid.GetTile(x + i + 1, y);
                if (CheckSimpleMatch(left, right)) {
                    matchCount++;
                }
                else {
                    matchCount = 1;
                }
                if (matchCount >= MatchingInARowRequired)
                    return true;
            }
            //check columns
            matchCount = 1;
            for (int i = -MatchingInARowRequired + 1; i < MatchingInARowRequired-1; i++) {
                Tile bot = tileGrid.GetTile(x , y + i);
                Tile top = tileGrid.GetTile(x , y + i + 1);
                if (CheckSimpleMatch(bot, top)) {
                    matchCount++;
                }
                else {
                    matchCount = 1;
                }
                if (matchCount >= MatchingInARowRequired)
                    return true;
            }
            

            return false;
        }

        /// <summary>
        /// Checks if two tiles match each other according to their
        /// <see cref="Tile.matchesWith"/> parameter. If either tile is null, false is returned.
        /// </summary>
        /// <param name="a">Tile A.</param>
        /// <param name="b">Tile B.</param>
        /// <returns>true for match, otherwise false.</returns>
        public static bool CheckSimpleMatch(Tile a, Tile b) {
            if (a == null || b == null)
                return false;
            if ((a.matchesWith & b.tileType) != 0)
                return true;
            if ((b.matchesWith & a.tileType) != 0)
                return true;
            
            return false;
        }

        /// <summary>
        /// Destroys a tile without triggering any effects.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void DestroyTileSilent(int x, int y) {
            tilePool.ReturnTile(tileGrid.RemoveTile(x, y));
        }

        public void PopulateAll() {
            for (int dx = 0; dx < tileGrid.dimensions.x; dx++) {
                for (int dy = 0; dy < tileGrid.dimensions.y; dy++) {
                    Tile prefab = RandomTilePrefab();
                    SpawnTile(prefab, dx, dy);
                    //sanity check
                    int sanity = 0;
                    while(CheckMatchTile(dx, dy) && sanity++<100) {
                        DestroyTileSilent(dx, dy);
                        prefab = RandomTilePrefab();
                        SpawnTile(prefab, dx, dy);
                    }
                    if (sanity >= 99)
                        Debug.LogWarning("Sanity exceded during spawning.");
                }
            }
        }

        /// <summary>
        /// Cardinal direction array for itteration.
        /// </summary>
        private Vector2Int[] Cardinals = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

        /// <summary>
        /// Queues all valid neighbours that match using CheckSimpleMatch. Used in <see cref="TileManager.GetConnectedMatching"/>.
        /// </summary>
        private void EnqueueNeighbours(Vector2Int pos, Queue<Vector2Int> queue, Tile other) {
            foreach (var c in Cardinals) {
                //skip closed tiles
                if (tileGrid.CheckBounds(pos.x + c.x, pos.y + c.y)) { 
                    if (tempData[pos.x + c.x, pos.y + c.y] != 0)
                        continue;
                }
                else
                    continue;
                Tile t = tileGrid.GetTile(pos.x + c.x, pos.y + c.y);

                //If the tile is not null and matches our mask, enqueue it
                if (t != null && CheckSimpleMatch(t, other))
                    queue.Enqueue(pos + c);
            }

        }

        /// <summary>
        /// <para>Gets all connected tiles that form a matching sequence.</para> <para> <paramref name="tiles"/> must
        /// contain enough space to store the whole set, otherwise an underfined subset of the tiles is returned. This
        /// function is non allocating.</para> <para>Returns the number of connected tiles found.</para>
        /// </summary>
        /// <param name="x">X position to check from.</param>
        /// <param name="y">Y position to check from.</param>
        /// <param name="tiles">Array with enough space to contain the returned set.</param>
        /// <returns>The number of connected tiles found.</returns>
        public int GetConnectedMatching(int x, int y, Tile[] tiles, byte MatchNum = 1, bool clearTemp = true) {
            if (!tileGrid.CheckBounds(x, y))
                return 0;
            Queue<Vector2Int> openSet = new Queue<Vector2Int>();
            int count = 0;
            //we will use our tempdata as our closed set
            if (clearTemp)
                ClearTemp();
            //NOTE: could have unintended consequences
            if (tempData[x, y] != 0)
                return 0;
            openSet.Enqueue(new Vector2Int(x, y));
            while (openSet.Count > 0 && count < tiles.Length) {
                Vector2Int o = openSet.Dequeue();
                Tile t = tileGrid.GetTile(o.x, o.y);
                //mark closed
                tempData[o.x, o.y] = MatchNum;
                EnqueueNeighbours(o, openSet, t);
                tiles[count++] = t;
            }

            return count;
        }
        #endregion

    }
}
