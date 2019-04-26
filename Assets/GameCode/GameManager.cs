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
        public SetPaused pauseRef;

        
        private TemplatedPool<TileFacade, Tile> facadePool;
        [Header("Effect settings")]
        public EffectManager effectManager;
        public TileFacade facadePrefab;
        public Material facadeMaterial;
        public ParticleSystem absorbSystem;
        public RectTransform starTransform;
        public AnimationCurve starSize;
        public AnimationCurve wormholeRadius;
        [Header("UI settings")]
        public GameObject EnableOnComplete;
        public GameObject EnableOnWin;
        public GameObject EnableOnLose;
        [Header("SFX")]
        public AudioPool pool;
        public AudioSource DestroyedEffect;
        private float basePitch;
        public AudioSource ScoreCompleteEffect;

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
            facadeMaterial.SetVector("_WormholePos", Camera.main.WorldToViewportPoint(starTransform.position)*2-Vector3.one);
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

        private int breaksThisTurn = 0;

        private void OnTurn() {
            currentTurns.Value--;
            breaksThisTurn = 1;
            pairsFound = false;
            StopCoroutine(pairCoroutine);
            pairingInProgress = false;

            
        }

        private void OnTilesBroken(IEnumerator<Tile> tiles) {
            int accum = 0;
            float mult = 1;
            int count = 0;
            Vector3 center = Vector3.zero;
            tiles.Reset();
            breaksThisTurn++;
            DestroyedEffect.pitch = basePitch * Mathf.Pow(1.1f, breaksThisTurn-1);
            DestroyedEffect.Play();
            
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

                if (t.isComplex) {
                    //effect
                    if (t.onDeathParticle != VFXType.None) {
                        var eff = effectManager.GetEffect(t.onDeathParticle);
                        eff.transform.position = t.transform.position;
                        eff.Play();
                    }

                    //effect
                    if (t.onDeathSound != AudioPool.SFXType.None) {
                        pool[t.onDeathSound].Play();
                    }
                }
            }
            center /= count;
            //play sound effect when goal reached
            if (currentScore.Value + (int)(accum * mult) >= goalScore.Value && currentScore.Value < goalScore.Value) {
                ScoreCompleteEffect.Play();
            }
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
            basePitch = DestroyedEffect.pitch;
        }
        //unsub from events
        private void OnDestroy() {
            tileManager.TilesDestroyed -= OnTilesBroken;
            tileManager.SuccessfulMove -= OnTurn;
        }

        private void Setup() {
            currentScore.Value = 0;
            currentTurns.Value = startingTurns;
            pauseRef.UnPause();
        }

        // Update is called once per frame
        void Update()
        {
            if (queueLengthRef.Value > 0) {
                pairsFound = false;
                StopCoroutine(pairCoroutine);
                pairingInProgress = false;
            }
            else {
                if (currentTurns.Value <= 0) {
                    //prevent actions
                    queueLengthRef.Value += 1;
                    Invoke("LateEndGame",1f);
                }
            }
        }

        private void LateEndGame() {
            queueLengthRef.Value -= 1;
            pauseRef.Pause();
            EnableOnComplete.SetActive(true);
            if (currentScore.Value >= goalScore.Value) {
                EnableOnWin.SetActive(true);
            }
            else {
                EnableOnLose.SetActive(true);
            }
            //trigger update
            currentScore.Value = currentScore.Value;
            
        }

        private HashSet<TileManager.TilePair> pairs = new HashSet<TileManager.TilePair>();
        IEnumerator pairCoroutine;
        private bool pairsFound;
        private bool pairingInProgress;
        private void PairCallback() {
            if (pairingInProgress == false)
                return;
            pairingInProgress = false;
            pairsFound = true;

            if (pairs.Count == 0) {
                //We didnt find any pairs, shuffle
                StartCoroutine(tileManager.ShuffleTiles());
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
                if (queueLengthRef.Value > 0)
                    yield return null;
                if(!pairingInProgress && !pairsFound) {
                    pairingInProgress = true;
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