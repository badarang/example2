using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<Pool> _pools;
        private Dictionary<string, Queue<GameObject>> _poolDictionary;

        private void Awake()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in _pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform); // 계층 구조 정리
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Pool with tag {tag} doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = _poolDictionary[tag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // 사용 후 다시 큐에 넣어서 재사용 준비 (활성화 상태로 큐에 들어감 -> 비활성화 시점은 개별 객체가 관리하거나 여기서 관리)
            // 일반적인 큐 방식은 Dequeue -> 사용 -> Enqueue(Return) 이지만, 
            // 여기서는 간단한 순환 큐 방식으로 구현하여 별도의 Return 호출 없이도 동작하게 할 수 있음.
            // 하지만 명시적인 Return이 더 안전하므로, 여기서는 Dequeue만 하고 
            // 객체가 비활성화될 때 ReturnToPool을 호출하는 구조가 이상적임.
            // 그러나 편의상 순환 구조(Dequeue 후 바로 Enqueue)를 많이 사용하기도 함.
            // 가이드라인의 "오브젝트 풀링 패턴 적용"을 위해 명시적 Return 기능을 추가함.
            
            // _poolDictionary[tag].Enqueue(objectToSpawn); // 순환 방식일 경우 주석 해제
            
            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform); // 다시 풀러 자식으로
            _poolDictionary[tag].Enqueue(obj);
        }
    }
}