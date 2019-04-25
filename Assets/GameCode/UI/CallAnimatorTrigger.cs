using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper.UI {
    public class CallAnimatorTrigger : MonoBehaviour {
        public Animator animator;
        public string triggerName;

        public void Trigger() {
            animator.SetTrigger(triggerName);
        }
    }
}
