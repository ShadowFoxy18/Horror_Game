using System;
using System.Collections.Generic;
using UnityEngine;

public class WhiCatPool : MonoBehaviour
{
    public static WhiCatPool instance;

    [Serializable]
    public class InstanceObject
    {
        public GameObject[] prefabs;
        [Min(1)] public int minObjectInstance = 1, maxObjectInstance = 1;
    }
    [SerializeField] InstanceObject[] instances;

    Dictionary<GameObject, Queue<GameObject>> diccPool = new Dictionary<GameObject, Queue<GameObject>>();
    Dictionary<GameObject, List<GameObject>> diccListActive = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
    void PushCurrentObject(GameObject prefab, GameObject obj, Queue<GameObject> queue)
    {
        queue.Enqueue(obj);
        if (diccListActive.ContainsKey(obj))
        {
            diccListActive.Remove(obj);
        } else
        {
            diccListActive.Add(prefab, new List<GameObject>());
        }
    }

    public void InstanceObj(GameObject prefab)
    {
        Queue<GameObject> queue = diccPool.ContainsKey(prefab)? diccPool[prefab] : new Queue<GameObject>();
        GameObject currentObject = Instantiate
            (
                prefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        currentObject.SetActive(false);

        if (diccPool.ContainsKey(prefab))
        {
            diccPool.Add(prefab, queue);
        }

        PushCurrentObject(prefab, currentObject, queue);
    }

    public GameObject PopObj(GameObject prefab)
    {
        GameObject obj = null;

        if (diccPool.ContainsKey(prefab))
        {
            if (!diccPool[prefab].TryDequeue(out obj))
            {
                InstanceObject currrentInstance = null;

                try
                {
                    Array.Find<InstanceObject>(instances, x => Array.Find<GameObject>(x.prefabs, j => j == prefab));
                } catch
                {
                    Debug.Log("Don't find prefab to Pop");
                }
                
                if (currrentInstance != null && (currrentInstance.minObjectInstance < currrentInstance.maxObjectInstance))
                {
                    InstanceObj(prefab);
                    currrentInstance.minObjectInstance++;
                }
                obj = diccPool[prefab].Dequeue();
            }
        }

        return obj;
    }

    public void PushObj(GameObject prefab, GameObject obj)
    {
        if (diccListActive.ContainsKey(prefab))
        {
            List<GameObject> currentList = diccListActive[prefab];

            if (currentList.Contains(obj))
            {
                PushCurrentObject(prefab, obj, diccPool[prefab]);
            }

        } else
        {
            Debug.LogError("Cannot Pushed the Object" +  obj.ToString());
        }
    }
}
