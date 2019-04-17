using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper { 
    /// <summary>
    /// Queues and exectutes actions.
    /// </summary>
    public class ActionQueue : MonoBehaviour
    {
        private LinkedList<Action> Actions = new LinkedList<Action>();
        private bool ActionInProgress;
        private Action active;
        int ID = 0;

        // Update is called once per frame
        void Update()
        {
            //Dequeue actions and execute them.
            if (!ActionInProgress) {
                if (Actions.Count > 0) {
                    active = Actions.First.Value;
                    Actions.RemoveFirst();
                    ActionInProgress = true;
                    active.Invoke(++ID);
                    
                }
            }
        }
        /// <summary>
        /// Callback for actions.
        /// </summary>
        /// <param name="id">ID given to the action when it was called.</param>
        public void ActionComplete(int id) {
            if (id == ID){ 
                ActionInProgress = false;
                active = null;
            }
            else {
                Debug.LogWarning("Incorrect action ID processed. found: " + id + " Expected: " + ID);
            }
        }
        /// <summary>
        /// Add an action ot the end of the queue.
        /// </summary>
        public void Enqueue(Action action) {
            Actions.AddLast(action);
        }

        /// <summary>
        /// Add am action to the start of the queue.
        /// </summary>
        /// <param name="action"></param>
        public void JumpQueue(Action action) {
            Actions.AddFirst(action);
        }
    }

    public delegate void Action(int ID);

}