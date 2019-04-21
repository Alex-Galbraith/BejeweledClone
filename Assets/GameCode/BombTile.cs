using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TSwapper { 
    public class BombTile : Tile {
        public int explosionRadius = 1;
        private bool dead = false;

        public override void OnMatched(TileManager tm) {
            PostFlip(tm);
        }

        public override void PostFlip(TileManager tm) {
            if (dead)
                return;
            dead = true;
            Tile[] tileBuffer = new Tile[(explosionRadius * 2 + 1) * (explosionRadius * 2 + 1)];
            int xPos = GridPos.x;
            int yPos = GridPos.y;
            int c = 0;
            for (int dx = -explosionRadius; dx <= explosionRadius; dx++) {
                for (int dy = -explosionRadius; dy <= explosionRadius; dy++) {
                    tileBuffer[c++] = tm.tileGrid.GetTile(xPos + dx, yPos + dy);
                }
            }

            tm.DestroyTiles(((IEnumerable<Tile>)tileBuffer).GetEnumerator());
        }

        public override void PopulateTile(Tile to) {
            base.PopulateTile(to);
            ((BombTile)to).dead = false;
            ((BombTile)to).explosionRadius = explosionRadius;
        }
    }
}