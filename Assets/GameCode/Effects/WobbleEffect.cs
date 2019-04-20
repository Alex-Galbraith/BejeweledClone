using UnityEngine;
using System.Collections;

namespace TSwapper {
    public class WobbleEffect {
        public static IEnumerator Wobble(int ID, ActionQueue queue, Tile[] t, float time, float strength, int oscilations) {
            float cTime = 0;

            while (cTime < time) {
                cTime += Time.deltaTime;
                for (int i = 0; i < t.Length; i++) {
                    float theta = cTime / time * 6.28318f * oscilations;
                    t[i].transform.Rotate(new Vector3(0, 0, Mathf.Sin(theta) * strength));
                }
                yield return 0;
            }
            queue.ActionComplete(ID);
        }

        public static IEnumerator Wobble(Tile[] t, float time, float strength, int oscilations) {
            float cTime = 0;

            while (cTime < time) {
                cTime += Time.deltaTime;
                for (int i = 0; i < t.Length; i++) {
                    float theta = cTime / time * 6.28318f * oscilations;
                    t[i].transform.Rotate(new Vector3(0, 0, Mathf.Sin(theta) * strength));
                }
                yield return 0;
            }
        }
    }
 }