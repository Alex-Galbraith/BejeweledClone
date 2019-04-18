using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper { 
    public class GameManager : MonoBehaviour
    {
        public TileManager tileManager;
        public int goalScore;
        public int startingTurns;
        
        private int currentScore;
        private int currentTurns;

        //For such a simple game I will run the UI from the GameManager. Not the nicest, but it will work fine.
        #region UI
        public Text scoreText;
        public Text turnsText;
        #endregion

        private void OnTurn() {
            currentTurns--;
            turnsText.text = "" + currentTurns;
            if (currentTurns <= 0) {
                if (currentScore >= goalScore) {
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
            currentScore += (int)(accum * mult);
            scoreText.text = ""+currentScore;
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
            currentScore = 0;
            currentTurns = startingTurns;
            scoreText.text = "" + currentScore;
            turnsText.text = "" + currentTurns;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}