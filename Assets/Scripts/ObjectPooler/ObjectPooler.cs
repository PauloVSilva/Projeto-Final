using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour{

    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler instance;

    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    public List<Pool> pools;
    //public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    void Start(){
        /*poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools){
            Queue<GameObject> objectPool = new Queue<GameObject>();

            if(pool.size < 1) pool.size = 1;
            
            for (int i = 0; i < pool.size; i++){
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = this.transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }*/
        
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach(Pool pool in pools){
            Queue<GameObject> objectPool = new Queue<GameObject>();

            if(pool.size < 1) pool.size = 1;
            
            for (int i = 0; i < pool.size; i++){
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = this.transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    /*public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, GameObject parent){
        if(!poolDictionary.ContainsKey(tag)){
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.parent = parent.transform;

        IPooledObjects pooledObj = objectToSpawn.GetComponent<IPooledObjects>();
        if(pooledObj != null){
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }*/

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation, GameObject parent){
        if(!poolDictionary.ContainsKey(prefab)){
            Debug.LogWarning("Pool with GameObject " + prefab + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[prefab].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.parent = parent.transform;

        IPooledObjects pooledObj = objectToSpawn.GetComponent<IPooledObjects>();
        if(pooledObj != null){
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[prefab].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}