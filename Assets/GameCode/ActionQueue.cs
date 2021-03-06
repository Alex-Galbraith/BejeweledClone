﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper { 
    /// <summary>
    /// Queues and exectutes actions.
    /// </summary>
    public class ActionQueue : MonoBehaviour
    {
        private LinkedList<Action> Actions = new LinkedList<Action>();
        public IntReference queueLengthRef;
        private bool ActionInProgress;
        private Action active;
        int ID = 0;

        private void Awake() {
            queueLengthRef.Value = 0;
        }
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
                queueLengthRef.Value--;
            }
            else {
                Debug.LogError("Incorrect action ID processed. found: " + id + " Expected: " + ID);
            }
        }
        /// <summary>
        /// Add an action ot the end of the queue.
        /// </summary>
        public void Enqueue(Action action) {
            Actions.AddLast(action);
            queueLengthRef.Value++;
        }

        /// <summary>
        /// Add an action to the start of the queue.
        /// </summary>
        /// <param name="action"></param>
        public void JumpQueue(Action action) {
            Actions.AddFirst(action);
            queueLengthRef.Value++;
        }
    }

    /// <summary>
    /// Delegate for queueable Actions. Delegates must call <see cref="ActionQueue.ActionComplete(int)"/> when finished, or they will halt the queue.
    /// </summary>
    /// <param name="ID">ID to call <see cref="ActionQueue.ActionComplete(int)"/>.</param>
    public delegate void Action(int ID);

}