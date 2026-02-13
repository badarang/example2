using UnityEngine;
using TMPro;

namespace RhythmGame
{
    public class JudgmentEffectController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMesh;
        [SerializeField] private float _moveSpeed = 2.0f;
        [SerializeField] private float _lifeTime = 1.0f;

        private float _timer;
        private ObjectPooler _pooler;

        public void Initialize(string text, Color color, ObjectPooler pooler)
        {
            if (_textMesh != null)
            {
                _textMesh.text = text;
                _textMesh.color = color;
            }
            
            _pooler = pooler;
            _timer = 0f;
            
            // 초기화 시 투명도 등 리셋이 필요하다면 여기서 처리
        }

        private void Update()
        {
            // 위로 이동
            transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

            // 타이머 체크
            _timer += Time.deltaTime;
            if (_timer >= _lifeTime)
            {
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            if (_pooler != null)
            {
                _pooler.ReturnToPool("JudgmentEffect", gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}