using UnityEngine;

namespace RhythmGame
{
    public class NoteController : MonoBehaviour
    {
        private NoteData _noteData;
        private RhythmManager _rhythmManager;
        private ObjectPooler _pooler;
        private float _scrollSpeed;
        private JudgeManager _judgeManager;

        // 판정선 위치 (Y축 0을 기준으로 함)
        private const float JUDGMENT_LINE_Y = 0f;
        // 화면 밖으로 나갔을 때 제거할 기준 시간 (Miss 판정 이후)
        private const float MISS_OFFSET = 0.5f;

        public NoteData NoteData => _noteData;

        public void Initialize(NoteData noteData, RhythmManager rhythmManager, ObjectPooler pooler, float scrollSpeed, JudgeManager judgeManager)
        {
            _noteData = noteData;
            _rhythmManager = rhythmManager;
            _pooler = pooler;
            _scrollSpeed = scrollSpeed;
            _judgeManager = judgeManager;

            // 판정 매니저에 등록
            if (_judgeManager != null)
            {
                _judgeManager.RegisterNote(_noteData.Lane, this);
            }
        }

        private void Update()
        {
            if (_rhythmManager == null) return;

            // 현재 트랙 시간 가져오기
            double currentDspTime = _rhythmManager.CurrentTrackTime;

            // 위치 계산 공식: (노트 시간 - 현재 시간) * 속도
            // 시간이 딱 맞으면(0) Y=0(판정선)에 위치
            float timeDiff = _noteData.Time - (float)currentDspTime;
            float yPos = JUDGMENT_LINE_Y + (timeDiff * _scrollSpeed);

            // 위치 갱신 (X축은 Spawner에서 설정한 값 유지, Y축만 변경)
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

            // 화면 아래로 지나갔으면(Miss) 풀로 반환
            // timeDiff가 음수가 되면 이미 지나간 것.
            if (timeDiff < -MISS_OFFSET)
            {
                Deactivate();
            }
        }

        public void OnHit()
        {
            // 판정 성공 시 호출됨 -> 즉시 제거
            Deactivate();
        }

        private void Deactivate()
        {
            if (_pooler != null)
            {
                _pooler.ReturnToPool("Note", gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}