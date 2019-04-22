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
        public IntReference queueLengthRef;

        private void OnTurn() {
            currentTurns.Value--;

            pairsFound = false;
            StopCoroutine(pairCoroutine);
            pairingInProgress = false;

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
            StartCoroutine(testing());
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
            if (queueLengthRef.Value > 0) {
                pairsFound = false;
                StopCoroutine(pairCoroutine);
                pairingInProgress = false;
            }
        }


        private HashSet<TileManager.TilePair> pairs = new HashSet<TileManager.TilePair>();
        IEnumerator pairCoroutine;
        private bool pairsFound;
        private bool pairingInProgress;
        private void PairCallback() {
            pairingInProgress = false;
            pairsFound = true;

            if (pairs.Count == 0) {
                //we are in trouble
            }
            else {
                foreach (var pair in pairs) {
                    pair.a.SetGleaming(true);
                    pair.b.SetGleaming(true);
                }
            }
        }

        IEnumerator testing() {
            //Testing code
            while (true) {
                if(!pairingInProgress && !pairsFound) {
                    foreach (var pair in pairs) {
                        pair.a.SetGleaming(false);
                        pair.b.SetGleaming(false);
                    }
                    pairs.Clear();
                    pairCoroutine = tileManager.GetFlippablePairs(pairs,PairCallback);
                    StartCoroutine(pairCoroutine);
                }
                else if (pairsFound){
                }
                yield return new WaitForSeconds(1);
            }
        }
        
    }
}