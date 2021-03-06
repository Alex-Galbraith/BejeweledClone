﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

namespace TSwapper {
    public class TileGrid : MonoBehaviour {
        [Tooltip("Dimensions of this grid.")]
        public Vector2IntReference dimensions;

        public Vector2Reference tileSize;

        private Tile[,] tiles;
        

        void Awake() {
            tiles = new Tile[dimensions.x, dimensions.y];
            
        }

        /// <summary>
        /// Gets the grid position of a point in worldspace.
        /// </summary>
        public Vector2Int WorldspaceToGridPos(Vector3 wpos) {
            wpos -= transform.position;
            wpos.x /= tileSize.x;
            wpos.y /= tileSize.y;
            return Vector2Int.FloorToInt(wpos);
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
        /// Swaps two tiles. No effect if one tile is out of bounds.
        /// </summary>
        /// <param name="x1">X pos of tile 1</param>
        /// <param name="y1">Y pos of tile 1</param>
        /// <param name="x2">X pos of tile 2</param>
        /// <param name="y2">Y pos of tile 2</param>
        public void SwapTiles(int x1, int y1, int x2, int y2) {
            if (!CheckBounds(x1, y1))
                return;
            if (!CheckBounds(x2, y2))
                return;


            Tile t = tiles[x1, y1];
            if (t == null) {
                Debug.LogWarning("Null tile at " + x1 + " " + y1);
                return;
            }
            if (tiles[x2, y2] == null) {
                Debug.LogWarning("Null tile at " + x2 + " " + y2);
                return;
            }

            tiles[x1, y1] = tiles[x2, y2];
            tiles[x1, y1].GridPos = new Vector2Int(x1, y1);
            
            tiles[x2, y2] = t;
            t.GridPos = new Vector2Int(x2, y2);
        }

        /// <summary>
        /// Removes the tile at the specified location. This DOES NOT destroy the object,
        /// nor trigger any callbacks. This must be done manually.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Null if the position was out of bounds or unoccupied, otherwise returns the tile.</returns>
        public Tile RemoveTile(int x, int y) {
            if (!CheckBounds(x, y))
                return null;
            if (tiles[x, y] == null)
                return null;
            Tile t = tiles[x, y];
            tiles[x, y].InGrid = false;
            tiles[x, y] = null;
            return t;
        }

        /// <summary>
        /// Get the worlspace position of a tile coordinate
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Rectangle specifying the worlspace rect this tile occupies.</returns>
        public Rect GridPosToWorldRect(int x, int y) {
            Rect r = new Rect {
                min = (((Vector2)transform.position) + Vector2.right * x * tileSize.x + Vector2.up * y * tileSize.y),
                max = (((Vector2)transform.position) + Vector2.right * x * tileSize.x + Vector2.up * y * tileSize.y + tileSize)
            };
            return r;
        }

        /// <summary>
        /// Shift the specified area a number of tiles. If safe is enabled, only works if the destination range is clear.
        /// If safe is disabled, will silently overwrite occupied tiles;
        /// </summary>
        /// <param name="fromX">X pos from inclusive</param>
        /// <param name="fromY">Y pos from inclusive</param>
        /// <param name="toX">X pos to exclusive</param>
        /// <param name="toY">Y pos to exclusive</param>
        /// <param name="shiftX">X shift</param>
        /// <param name="shiftY">Y shift</param>
        /// <param name="safe">Y shift</param>
        /// <returns>List of updated tiles if successful. Null if inputs were out of bounds, or, if safe is enabled, there were occupied tiles in the destination range.</returns>
        public Tile[] ShiftTiles(int fromX, int fromY, int toX, int toY, int shiftX, int shiftY, bool safe = true) {

            if (!CheckBounds(fromX, fromY))
                return null;
            if (!CheckBounds(toX-1, toY-1))
                return null;

            if (!CheckBounds(fromX + shiftX, fromY + shiftY))
                return null;
            if (!CheckBounds(toX + shiftX-1, toY + shiftY-1))
                return null;

            

            int minX = Min(fromX, toX);
            int minY = Min(fromY, toY);
            int maxX = Max(fromX, toX);
            int maxY = Max(fromY, toY);

            int sizeX = maxX - minX;
            int sizeY = maxY - minY;

            int incX = -(int)Sign(shiftX);
            int incY = -(int)Sign(shiftY);

            //if safe, do occupancy check
            if (safe) { 
                //This is an inefficient way to go about this, but we are dealing with such a small 
                //number of items, and this is far more readable
                for (int i = minX + shiftX; i < maxX + shiftX; i += 1) { 
                    for (int j = minY + shiftY; j < maxY + shiftY; j += 1) {
                        //We only need to check non overlapping regions
                        if (i < toX && i >= fromX && j < toY && j >= fromY)
                            continue;
                        //ensure unoccupied
                        if (tiles[i, j] != null)
                            return null;
                    }
                }
            }

            Tile[] rTiles = new Tile[sizeX * sizeY];

            int startAtX = shiftX > 0 ? maxX : minX;
            int startAtY = shiftY > 0 ? maxY : minY;

            //Shift tiles
            int tcount = 0;
            for (int i = 0; i < sizeX; i++) {
                for (int j = 0; j < sizeY; j++) {
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
