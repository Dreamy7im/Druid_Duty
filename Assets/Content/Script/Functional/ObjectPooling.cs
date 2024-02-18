using System.Collections.Generic;
using UnityEngine;


public class ObjectPooling : MonoBehaviour
{

    #region Singleton
    public static ObjectPooling Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Another instance of ObjectPooling already exists. Destroying this one.");
            Destroy(this);
        }
    }
    #endregion

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
        public string tag { get { return prefab.name; } } // Automatyczne przypisanie nazwy prefabu jako tagu
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    [SerializeField] private List<GameObject> ParentList = new List<GameObject>();

    [SerializeField] private List<ItemInfo> ItemInfoDatabase = new List<ItemInfo>();
    private int RandomItem;

    void Start()
    {
        foreach (Pool pool in pools)
        {
            GameObject Parent = new GameObject("");
            Parent.name = pool.tag;
            Parent.transform.parent = transform;
            ParentList.Add(Parent);

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                obj.transform.parent = Parent.transform;
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

   public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // SprawdŸ, czy istnieje wolny obiekt w puli
        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                return obj;
            }
        }

        // Jeœli brak wolnych obiektów, dodaj nowy do puli
        AddNewObject(tag);

        // Pobierz i zwróæ nowy obiekt z puli
        GameObject objToSpawn = poolDictionary[tag].Dequeue();
        objToSpawn.SetActive(true);
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public void AddNewObject(string tag)
    {
        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                // Wyszukaj w³aœciwego rodzica w liœcie ParentList
                GameObject parent = ParentList.Find(p => p.name == tag);
                if (parent != null)
                {
                    GameObject newObj = Instantiate(pool.prefab, parent.transform);
                    newObj.SetActive(false);
                    poolDictionary[tag].Enqueue(newObj);
                    break;
                }
                else
                {
                    Debug.LogWarning("Parent with tag " + tag + " not found.");
                }
            }
        }
    }


    public ItemInfo ReturnItemFromDatabase()
    {
        RandomItem = Random.Range(0, ItemInfoDatabase.Count);
        return ItemInfoDatabase[RandomItem];
    }
}