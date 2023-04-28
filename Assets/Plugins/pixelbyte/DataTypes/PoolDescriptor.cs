using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Pixelbyte
{
    /// <summary>
    /// Describes each pool contained by the Pool class
    /// 2022 Pixelbyte Studios
    /// </summary>
    public sealed class PoolDescriptor
    {
        ObjectPool<GameObject> pool;

        //The parent where these gameObjects are pooled
        Transform poolParent;

        /// <summary>
        /// The prefab for which this pool exists
        /// </summary>
        GameObject prefab;

        public ObjectPool<GameObject> Pool => pool;

        /// <summary>
        /// True if any of the components attached to the prefab implement IPoolable
        /// </summary>
        bool implementsIPoolable;

        public bool ImplementsIPoolable => implementsIPoolable;

        public PoolDescriptor(Transform parent, GameObject prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxCapacity = 10000)
        {
            this.prefab = prefab;
            CreatePoolParent(parent);
            pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnPooled, OnDestroy, collectionCheck, defaultCapacity, maxCapacity);

            //Check if any of the objects implement IPoolable
            //If not, then we dont bother to check each time objects are pooled or borrowed
            var poolInterface = prefab.GetComponentsInChildren<IPoolable>(true);
            implementsIPoolable = (poolInterface.Length > 0);
        }

        public GameObject OnCreate()
        {
            var gob = Object.Instantiate(prefab);
            return gob;
        }

        void CreatePoolParent(Transform parent)
        {
            if (poolParent != null) return;
            poolParent = new GameObject(prefab.name).transform;
            poolParent.SetParent(parent);
            poolParent.position = Vector3.zero;
        }

        private void OnDestroy(GameObject obj) => Object.Destroy(obj);
        private void OnPooled(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(poolParent, false);

            if (implementsIPoolable)
                SendPooledMessage(obj);
        }

        private void OnGet(GameObject obj)
        {
            obj.transform.SetParent(null, false);
        }

        public override int GetHashCode() => prefab.GetInstanceID();

        //This is used to avoid a GC alloc when we look for components that implement IPoolable
        private static List<IPoolable> iPoolableCache = new List<IPoolable>(32);

        private static void SendPooledMessage(GameObject obj)
        {
            //Get all components on this object that have an IPoolable interface
            iPoolableCache.Clear();
            obj.GetComponentsInChildren(true, iPoolableCache);
            if (iPoolableCache.Count > 0)
                for (int i = 0; i < iPoolableCache.Count; i++)
                    iPoolableCache[i].OnPooled();
        }

        public static void SendRespawnedMessage(GameObject obj)
        {
            //Get all components on this object and its children that have an IPoolable interface
            iPoolableCache.Clear();
            obj.GetComponentsInChildren(true, iPoolableCache);
            if (iPoolableCache.Count > 0)
                for (int i = 0; i < iPoolableCache.Count; i++)
                    iPoolableCache[i].OnReSpawned();
        }
    }
}
