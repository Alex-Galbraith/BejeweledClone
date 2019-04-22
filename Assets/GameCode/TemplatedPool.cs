#define SAFE_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TemplatedPool<T, U> where T : MonoBehaviour
{
    /// <summary>
    /// Delegate for populating a pooled object.
    /// </summary>
    public delegate void Populate(T template, U recipient);

    private List<T> pooledItems;
    private T template;
    private Populate populateFunc;
    private Transform parentTrans;

    /// <summary>
    /// Create a new TemplatePool.
    /// </summary>
    /// <param name="template">Template object to spawn new instances from.</param>
    /// <param name="pfunc">Function for populating an object.</param>
    /// <param name="parent">Transform to spawn objects under.</param>
    public TemplatedPool(T template, Populate pfunc, Transform parent) {
        pooledItems = new List<T>();
        this.template = template;
        populateFunc = pfunc;
        parentTrans = parent;
    }

    /// <summary>
    /// Get an object from the pool.
    /// </summary>
    /// <param name="populateLike">Object to populate the returned object like, using the function
    /// this object was constructed with.</param>
    public T GetObject(U populateLike) {
        T t;
        if (pooledItems.Count > 0) {
            t = pooledItems[pooledItems.Count - 1];
            pooledItems.RemoveAt(pooledItems.Count - 1);
        }
        else {
            t= GameObject.Instantiate<T>(template, parentTrans);
        }

        populateFunc(t, populateLike);
        return t;
    }

    /// <summary>
    /// Return an object to the pool.
    /// </summary>
    /// <param name="t">Object to return.</param>
    public void ReturnObject(T t) {
        t.gameObject.SetActive(false);

#if SAFE_MODE
            //Check we arent adding duplicates
            if (pooledItems.Contains(t)) {
                throw new System.InvalidOperationException("Object already in pool.");
            }
#endif

        pooledItems.Add(t);
    }
}
