using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 This Object Pool with be used to manage instantiations that happen at a high rate
 In this case it will be used for instantiating the HitEffect prefab when a player fires
 
    Because some of the weapons are automatic weapons, constantly instantiating and destroying the
    HitEffect prefab over and over again for every shot may become taxing, especially since there may be
    more than one player using a rapid fire weapon

    The idea is to fill an object pool with a set size of instantiations first, and then re-use instantiations in
    the pool later on, instead of constantly creating and destroying new ones
     
     
 */
public class ObjectPooler : MonoBehaviour {

    //Create a Pool class to hold a name, a prefab, and a set size
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    //void Start()
    //{

    //    //Start by creating a dictionary mapping the name of the pool to the queue used for it
    //    poolDictionary = new Dictionary<string, Queue<GameObject>>();

    //    foreach (Pool pool in pools)
    //    {
    //        Queue<GameObject> objectPool = new Queue<GameObject>();

    //        //Create a queue and fill with instantiations of the prefab set in the inspector
    //        for (int i = 0; i < pool.size; i++)
    //        {
    //            GameObject obj = Instantiate(pool.prefab);
    //            obj.SetActive(false);
    //            objectPool.Enqueue(obj);
    //        }

    //        //Add the pool (queue) to the dictionary
    //        poolDictionary.Add(pool.tag, objectPool);
    //    }
    //}

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        //Pull out the first element in the queue
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        //Recycle the instantiation
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
	
}
