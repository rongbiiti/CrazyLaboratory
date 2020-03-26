using UnityEngine;
using System;

public class RsdAcdPool : MonoBehaviour {

    private ObjectPool pool;
    [SerializeField, CustomLabel("残留酸のプレハブ")] private GameObject _rsdAcdPrefab;


    protected static readonly string[] findTags =
    {
        "RsdAcdPool",
    };

    private static RsdAcdPool instance;

    public static RsdAcdPool Instance
    {
        get
        {
            if (instance == null) {

                Type type = typeof(RsdAcdPool);

                foreach (var tag in findTags) {
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

                    for (int j = 0; j < objs.Length; j++) {
                        instance = (RsdAcdPool)objs[j].GetComponent(type);
                        if (instance != null)
                            return instance;
                    }
                }

                Debug.LogWarning(string.Format("{0} is not found", type.Name));
            }

            return instance;
        }
    }

    void Awake()
    {
        CheckInstance();
        pool = gameObject.AddComponent<ObjectPool>();
        RsdAcdPool.Instance.CreateRsdAcdPool(_rsdAcdPrefab, 5);
    }

    private bool CheckInstance()
    {
        if (instance == null) {
            instance = (RsdAcdPool)this;
            DontDestroyOnLoad(gameObject);
            return true;
        } else if (Instance == this) {
            return true;
        }

        Destroy(this);
        return false;
    }

    public void CreateRsdAcdPool(GameObject obj, int maxCount)
    {
        pool.CreatePool(obj, maxCount);
    }

    public GameObject GetObject()
    {
        return pool.GetObject();
    }

    
}
