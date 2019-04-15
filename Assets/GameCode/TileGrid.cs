using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

namespace TSwapper {
    public class TileGrid : MonoBehaviour {
        [Tooltip("Dimensions of this grid.")]
        public Vector2Int dimensions = new Vector2Int(9, 9);

        private Tile[,] tiles;

        void Awake() {
            tiles = new Tile[dimensions.x, dimensions.y];
        }

        /// <summary>
        /// Check if the specified positions are inside the valid boundaries.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns> True if the position is valid, otherwise false.</returns>
        public bool CheckBounds(int x, int y) {
            return x >= 0 && x < dimensions.x && y >= 0 && y < dimensions.y;
        }

        /// <summary>
        /// Get a tile at the specified position.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>A Tile, or null if the position is empty or out of bounds.</returns>
        public Tile GetTile(int x, int y) {
            return CheckBounds(x,y) ? tiles[x, y] : null;
        }

        /// <summary>
        /// Set a tile at the specified position. Returns a tile if the position was already occupied.
        /// This DOES NOT destroy the replaced tile, nor trigger any callbacks. This must be done manually.
        /// Out of bounds positions will return null.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="t">Tile to set</param>
        /// <returns>The replaced tile</returns>
        public Tile SetTile(int x, int y, Tile t) {
            if (!CheckBounds(x, y))
                return null;
            Tile old = tiles[x, y];
            tiles[x, y] = t;
            t.InGrid = true;
            t.GridPos = new Vector2Int(x, y);
            return old;
        }

        /// <summary>
        /// Removes the tile at the specified location. This DOES NOT destroy the object,
        /// nor trigger any callbacks. This must be done manually.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>False if the position was out of bounds or unoccupied, otherwise true.</returns>
        public bool RemoveTile(int x, int y) {
            if (!CheckBounds(x, y))
                return false;
            if (tiles[x, y] == null)
                return false;
            tiles[x, y].InGrid = false;
            tiles[x, y] = null;
            return true;
        }

        /// <summary>
        /// Shift the specified area a number of tiles. Only works if the destination range is clear.
        /// </summary>
        /// <param name="fromX">X pos from inclusive</param>
        /// <param name="fromY">Y pos from inclusive</param>
        /// <param name="toX">X pos to exclusive</param>
        /// <param name="toY">Y pos to exclusive</param>
        /// <param name="shiftX">X shift</param>
        /// <param name="shiftY">Y shift</param>
        /// <returns>List of updated tiles if successful. Null if there were occupied tiles in the destination range.</returns>
        public Tile[] ShiftTilesSafe(int fromX, int fromY, int toX, int toY, int shiftX, int shiftY) {
            if (!CheckBounds(fromX, fromY))
                return null;
            if (!CheckBounds(toX, toY))
                return null;

            if (!CheckBounds(fromX + shiftX, fromY + shiftY))
                return null;
            if (!CheckBounds(toX + shiftX, toY + shiftY))
                return null;

            

            int minX = Min(fromX, toX);
            int minY = Min(fromY, toY);
            int maxX = Max(fromX, toX);
            int maxY = Max(fromY, toY);

            int sizeX = maxX - minX;
            int sizeY = maxY - minY;

            int incX = (int)Sign(shiftX);
            int incY = (int)Sign(shiftY);
            //This is an inefficient way to go about this, but we are dealing with such a small 
            //number of items, and this is far more readable
            for (int i = fromX + shiftX; i < toX + shiftX; i += incX) { 
                for (int j = fromY + shiftY; j < toY + shiftY; j += incY) {
                    //We only need to check non overlapping regions
                    if (i < toX && i >= fromX && j < toY && j >= fromY)
                        continue;
                    //ensure unoccupied
                    if (tiles[i, j] != null)
                        return null;
                }
            }

            Tile[] rTiles = new Tile[sizeX * sizeY];

            int startAtX = shiftX > 0 ? maxX : minX;
            int startAtY = shiftY > 0 ? maxY : minY;

            //Shift tiles
            int tcount = 0;
            for (int i = sizeX; i >=0; i++) {
                for (int j = sizeY; j >= 0; j++) {
                    Vector2Int p = new Vector2Int(startAtX + i * incX + shiftX, startAtY + j * incY + shiftY);
                    tiles[p.x, p.y] = tiles[startAtX + i * incX, startAtY + j * incY];
                    tiles[p.x, p.y].GridPos = p;
                    tiles[startAtX + i * incX, startAtY + j * incY] = null;
                    rTiles[tcount++] = tiles[p.x, p.y];
                }
            }

            return rTiles;
        }

    }
}
