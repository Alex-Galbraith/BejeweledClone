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

        
        private TemplatedPool<TileFacade, Tile> facadePool;
        [Header("Effect settings")]
        public TileFacade facadePrefab;
        public Material facadeMaterial;
        public ParticleSystem absorbSystem;
        public Transform starTransform;
        public AnimationCurve starSize;
        public AnimationCurve wormholeRadius;

        #region effect tweens
        IEnumerator AnimateStar() {
            float cTime = 0;
            while (cTime < 1) {
                cTime += Time.deltaTime * 2f;
                starTransform.localScale = Vector3.one * starSize.Evaluate(cTime);
                yield return null;
            }
        }

        IEnumerator AnimateMaterial(List<TileFacade> facades) {
            facadeMaterial.SetFloat("_WormholeRadius", 0);
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            facades[0].spriteRenderer.GetPropertyBlock(mpb);
            foreach (var f in facades) {
                f.spriteRenderer.SetPropertyBlock(mpb);
            }
            float cTime = 0;
            while (cTime < 1) {
                cTime += Time.deltaTime * 2f;
                mpb.SetFloat("_WormholeRadius", wormholeRadius.Evaluate(cTime));
                foreach (var f in facades) {
                    f.spriteRenderer.SetPropertyBlock(mpb);
                }
                yield return null;
            }
            mpb.SetFloat("_WormholeRadius", 100);
            foreach (var f in facades) {
                facadePool.ReturnObject(f);
            }
            facades.Clear();
        }
        #endregion

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

        private void OnTilesBroken(IEnumerator<Tile> tiles) {
            int accum = 0;
            float mult = 1;
            int count = 0;
            Vector3 center = Vector3.zero;
            tiles.Reset();
            //This creates an anourmous amount of work for the garbage collector and I shouldnt be doing it.
            List<TileFacade> facades = new List<TileFacade>();
            while(tiles.MoveNext()) {
                count++;
                Tile t = tiles.Current;
                if (t == null)
                    continue;
                accum += t.baseScoreValue;
                
                center += t.transform.position;
                mult *= t.scoreMultiplier;
                var tf = facadePool.GetObject(t);
                tf.gameObject.SetActive(true);
                tf.spriteRenderer.sharedMaterial = facadeMaterial;
                facades.Add(tf);
                StartCoroutine(AnimateMaterial(facades));
                StartCoroutine(AnimateStar());
                absorbSystem.Play();
            }
            center /= count;
            currentScore.Value += (int)(accum * mult);
        }

        // Start is called before the first frame update
        void Start()
        {
            tileManager.TilesDestroyed += OnTilesBroken;
            tileManager.SuccessfulMove += OnTurn;
            facadePool = new TemplatedPool<TileFacade, Tile>(facadePrefab, Tile.PopulateFacade, transform);
            Setup();
            StartCoroutine(FindPairs());
        }
        //unsub from events
        private void OnDestroy() {
            tileManager.TilesDestroyed -= OnTilesBroken;
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

        IEnumerator FindPairs() {
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
                yield return new WaitForSeconds(1);
            }
        }
        
    }
}