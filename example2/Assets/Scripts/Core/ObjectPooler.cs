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
            public Transform parentTransform; // [추가] 특정 부모 아래 생성하고 싶을 때 지정
        }

        [SerializeField] private List<Pool> _pools;
        private Dictionary<string, Queue<GameObject>> _poolDictionary;
        private Dictionary<string, Transform> _poolParents; // 태그별 부모 저장

        private void Awake()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
            _poolParents = new Dictionary<string, Transform>();

            foreach (Pool pool in _pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                
                // 부모 설정: 지정된 부모가 있으면 사용, 없으면 ObjectPooler 자신을 사용
                Transform parent = pool.parentTransform != null ? pool.parentTransform : transform;
                _poolParents[pool.tag] = parent;

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, parent);
                    obj.SetActive(false);
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

            // 부모가 바뀌었을 수도 있으므로(다른 스크립트에서 SetParent 했을 경우), 
            // 다시 원래 풀의 부모로 돌려놓는 것이 안전할 수 있으나, 
            // UI의 경우 동적으로 부모를 바꾸는 경우가 많아 여기서는 강제하지 않음.
            // 필요하다면 아래 주석 해제:
            // if (_poolParents.ContainsKey(tag) && _poolParents[tag] != null)
            //     objectToSpawn.transform.SetParent(_poolParents[tag]);

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Pool with tag {tag} doesn't exist.");
                // 풀에 없으면 그냥 파괴하거나 비활성화
                obj.SetActive(false);
                return;
            }

            obj.SetActive(false);
            
            // 반환 시 원래 지정된 부모 밑으로 정리 (계층 구조 깔끔하게 유지)
            if (_poolParents.ContainsKey(tag) && _poolParents[tag] != null)
            {
                obj.transform.SetParent(_poolParents[tag]);
            }
            else
            {
                obj.transform.SetParent(transform);
            }

            _poolDictionary[tag].Enqueue(obj);
        }
    }
}