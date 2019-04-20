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
            
        }


        private HashSet<TileManager.TilePair> pairs = new HashSet<TileManager.TilePair>();
        IEnumerator pairCoroutine;
        private bool pairsFound;
        private bool pairingInProgress;
        private void PairCallback() {
            pairingInProgress = false;
            pairsFound = true;
        }

        IEnumerator testing() {
            //Testing code
            while (true) {
                if(!pairingInProgress && !pairsFound) { 
                    pairs.Clear();
                    pairCoroutine = tileManager.GetFlippablePairs(pairs,PairCallback);
                    StartCoroutine(pairCoroutine);
                }
                else if (pairsFound){
                    StartCoroutine(Wobble(pairs, 0.5f, 3f, 3));
                }
                yield return new WaitForSeconds(1);
            }
        }

        private static IEnumerator Wobble(HashSet<TileManager.TilePair> t, float time, float strength, int oscilations) {
            float cTime = 0;

            while (cTime < time) {
                cTime += Time.deltaTime;
                float theta = (cTime / time) * 6.28318f * oscilations;
                var e = t.GetEnumerator();
                while (e.MoveNext()) {
                    e.Current.a.transform.Rotate(new Vector3(0, 0, strength * Mathf.Sin(theta) * Mathf.Sin(2*theta)));
                    e.Current.b.transform.Rotate(new Vector3(0, 0, strength * Mathf.Sin(theta) * Mathf.Sin(2 * theta)));
                }
                yield return null;
            }
        }
    }
}