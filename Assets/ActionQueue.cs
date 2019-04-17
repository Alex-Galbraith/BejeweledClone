using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper { 
    public class ActionQueue : MonoBehaviour
    {
        private Queue<Action> Actions = new Queue<Action>();
        private bool ActionInProgress;
        private Action active;
        int ID = 0;
        // Update is called once per frame
        void Update()
        {
            if (!ActionInProgress) {
                if (Actions.Count > 0) {
                    active = Actions.Dequeue();
                    ActionInProgress = true;
                    active.Invoke(++ID);
                    
                }
            }
        }

        public void ActionComplete(int id) {
            if (id == ID){ 
                ActionInProgress = false;
                active = null;
            }
        }

        public void Enqueue(Action action) {
            Actions.Enqueue(action);
        }
    }

    public delegate void Action(int ID);

}