using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size;
    }

    public Dictionary<string, Queue<GameObject>> PoolDictionary;
    public List<Pool> Pools;
    public static PoolManager Instance;
    
    
    public void Init()
    {
        Instance = this;
    }

    void Start()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in Pools)
        {
            Queue<GameObject> objectsPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab);
                obj.SetActive(false);
                objectsPool.Enqueue(obj);
            }

            PoolDictionary.Add(pool.Tag, objectsPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.Log("Pool with tag " + tag + "doesn't exist");
            return null;
        }
        
        GameObject objectToSpawn = PoolDictionary[tag].Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = Quaternion.identity;
        
        PoolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}