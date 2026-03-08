using System.Collections.Generic;
using UnityEngine;


namespace HelperScripts
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        private Queue<GameObject> pool = new  Queue<GameObject>();

        public GameObject GetObject()
        {
            if (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                obj.SetActive(true);
                
                return obj;
            }

            return Instantiate(prefab, transform);
        }

        public void ReturnObject(GameObject _obj)
        {
            _obj.SetActive(false);
            pool.Enqueue(_obj);
        }
    }
}
