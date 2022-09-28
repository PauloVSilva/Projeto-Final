using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour{

    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject prefab;
        public int size;

        public Pool(GameObject _prefab, int _size){
            prefab = _prefab;
            size = _size;
            tag = _prefab.name;
        }
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
    public Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    void Start(){
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        foreach(Pool pool in pools){
            GeneratePool(pool);
        }
    }

    public void AddPool(GameObject _prefab, int _size){
        if(poolDictionary.ContainsKey(_prefab)){
            Debug.Log("Pool with GameObject " + _prefab + " already exist. Extending it by " + _size);
            foreach(Pool pool in pools){
                if(pool.prefab == _prefab){
                    ExtendPool(pool, _size);
                    return;
                }
            }
        }
        else{
            Pool pool = new Pool(_prefab, _size);
            pools.Add(pool);
            GeneratePool(pool);
        }
    }

    private void GeneratePool(Pool pool){
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

    private void ExtendPool(Pool pool, int size){
        for(int i = 0; i < size; i++){
            GameObject objectToAdd = Instantiate(pool.prefab);
            objectToAdd.transform.parent = this.transform;
            objectToAdd.SetActive(false);
            poolDictionary[pool.prefab].Enqueue(objectToAdd);
            pool.size++;
        }
    }

    public void RemovePool(GameObject _prefab){
        if(!poolDictionary.ContainsKey(_prefab)){
            Debug.Log("Pool with GameObject " + _prefab + " doesn't exist. Can't remove it.");
        }
        else{
            foreach(Pool pool in pools){
                if(pool.prefab == _prefab){
                    pools.Remove(pool);
                    DestroyPool(pool);
                    return;
                }
            }
        }
    }

    private void DestroyPool(Pool pool){
        for (int i = 0; i < pool.size; i++){
            GameObject objectToDestroy = poolDictionary[pool.prefab].Dequeue();
            Destroy(objectToDestroy);
        }
        poolDictionary.Remove(pool.prefab);
    }

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