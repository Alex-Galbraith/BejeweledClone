using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper {
    public class GameInput : MonoBehaviour {
        public TileGrid tileGrid;
        public TileManager tileManager;

        Vector2Int  SelectedPosition;
        bool isPositionSelected = false;

        Vector2Int ClickDownPosition;
        bool isClickDown = false;

        private void EvaluateAndSwap(Vector2Int a, Vector2Int b) {
            Vector2Int delta = b - a;
            //are we one manhattan distance away
            if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1) {
                //swap
                tileManager.TrySwapTiles(a, b);
            }
        }


        // Update is called once per frame
        private void Update() {
            Vector3 mouseWPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = tileGrid.WorldspaceToGridPos(mouseWPos);
            //we have clicked;
            if (Input.GetButtonDown("Fire1")) {
                ClickDownPosition = gridPos;
                isClickDown = true;
            }
            //we have clicked;
            if (Input.GetButtonUp("Fire1")) {
                isClickDown = false;
                //we dragged
                if (ClickDownPosition != gridPos) {
                    EvaluateAndSwap(ClickDownPosition, gridPos);
                }
                else {
                    //second click
                    if (isPositionSelected) {
                        isPositionSelected = false;
                        EvaluateAndSwap(SelectedPosition, gridPos);
                    }
                    else {
                        SelectedPosition = gridPos;
                        isPositionSelected = true;
                    }
                }
            }
        }
    }
}