using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper { 
    public class GameManager : MonoBehaviour
    {
        public TileManager tileManager;
        public IntReference goalScore;
        public int startingTurns;

        public IntReference currentScore;
        public IntReference currentTurns;

        private void OnTurn() {
            currentTurns.Value--;
            if (currentTurns.Value <= 0) {
                if (currentScore.Value >= goalScore.Value) {
                    //win
                }
                else {
                    //lose
                }
            }
        }

        private void OnTilesBroke(Tile[] tiles) {
            int accum = 0;
            float mult = 1;
            foreach (Tile t in tiles) {
                accum += t.baseScoreValue;
                mult *= t.scoreMultiplier;
            }
            currentScore.Value += (int)(accum * mult);
        }

        // Start is called before the first frame update
        void Start()
        {
            tileManager.TilesDestroyed += OnTilesBroke;
            tileManager.SuccessfulMove += OnTurn;
            Setup();
        }
        //unsub from events
        private void OnDestroy() {
            tileManager.TilesDestroyed -= OnTilesBroke;
            tileManager.SuccessfulMove -= OnTurn;
        }

        private void Setup() {
            currentScore.Value = 0;
            currentTurns.Value = startingTurns;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}