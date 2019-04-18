using UnityEngine;
using System.Collections;
namespace TSwapper { 
    public abstract class TileLerpEffect {
        /// <summary>
        /// Converts an input time int domain [0,1] to an output time of range [0,1].
        /// </summary>
        /// <param name="t">Inpute time in domain [0,1]</param>
        /// <returns></returns>
        public delegate float LerpFunction(float t);

        /// <summary>
        /// Constant lerp function maps t -> t
        /// </summary>
        public static float ConstLerp(float t) {
            return t;
        }

        /// <summary>
        /// Mathf smoothstep lerp
        /// </summary>
        public static float EaseInOutLerp(float t) {
            return Mathf.SmoothStep(0, 1, t);
        }

        /// <summary>
        /// f -> f * f
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float SqrLerp(float f) {
            return f * f;
        }

        public static IEnumerator LerpPosition(int ID, ActionQueue queue, Tile[] t, float time, TileGrid tg, LerpFunction lf) {
            Vector3[] positionsFrom = new Vector3[t.Length];
            Vector3[] positionsTo = new Vector3[t.Length];
            float cTime = 0;
            for (int i = 0; i < t.Length; i++) {
                positionsFrom[i] = t[i].transform.position;
                positionsTo[i] = tg.GetWorldspaceTilePos(t[i].GridPos.x,t[i].GridPos.y).center;
            }

            while (cTime < time) {
                cTime += Time.deltaTime;
                for (int i = 0; i < t.Length; i++) {
                    t[i].transform.position = Vector3.Lerp(positionsFrom[i], positionsTo[i], lf(cTime/time));
                }
                yield return 0;
            }

            queue.ActionComplete(ID);

        }
    }
}