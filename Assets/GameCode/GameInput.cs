using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper {
    public class GameInput : MonoBehaviour {
        public TileGrid tileGrid;
        public TileManager tileManager;

        public GameObject selectionMarker;

        public IntReference actionQueueLength;
        public SetPaused pauseRef;

        Vector2Int  SelectedPosition;
        bool isPositionSelected = false;

        Vector2Int ClickDownPosition;

        /// <summary>
        /// Check if we can swap based on distance, then try swap
        /// </summary>
        private bool EvaluateAndSwap(Vector2Int a, Vector2Int b) {
            Vector2Int delta = b - a;
            //are we one manhattan distance away
            if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1) {
                //swap
                return tileManager.TrySwapTiles(a, b);
            }
            return false;
        }


        // Update is called once per frame
        private void Update() {
            if (actionQueueLength.Value > 0 || pauseRef.IsPaused)
                return;
            Vector3 mouseWPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = tileGrid.WorldspaceToGridPos(mouseWPos);
            bool inBounds = tileGrid.CheckBounds(gridPos.x, gridPos.y);
            Vector3 wGridPos = tileGrid.GridPosToWorldRect(gridPos.x, gridPos.y).center;

            if (Input.GetButtonUp("Fire2")) {
                StartCoroutine(tileManager.ShuffleTiles());
            }

            //check boundaries
            if (!inBounds && (Input.GetButton("Fire1")|| Input.GetButtonUp("Fire1"))) {
                isPositionSelected = false;
                selectionMarker.SetActive(false);
                return;
            }

            //we have clicked;
            if (Input.GetButtonDown("Fire1")) {
                ClickDownPosition = gridPos;
                if(isPositionSelected == false){ 
                    selectionMarker.transform.position = wGridPos;
                    selectionMarker.SetActive(true);
                }
            }
            //we have clicked;
            if (Input.GetButtonUp("Fire1")) {
                //we dragged
                if (ClickDownPosition != gridPos) {
                    if (EvaluateAndSwap(ClickDownPosition, gridPos)) { 
                        isPositionSelected = false;
                        selectionMarker.SetActive(false);
                    }
                }
                else {
                    //second click
                    if (isPositionSelected) {
                        isPositionSelected = false;
                        selectionMarker.SetActive(false);
                        if(!EvaluateAndSwap(SelectedPosition, gridPos)) {
                            SelectedPosition = gridPos;
                            isPositionSelected = true;
                            selectionMarker.transform.position = wGridPos;
                            selectionMarker.SetActive(true);
                        }
                    }
                    else {
                        SelectedPosition = gridPos;
                        isPositionSelected = true;
                        selectionMarker.transform.position = wGridPos;
                        selectionMarker.SetActive(true);
                    }
                }
            }
        }
    }
}