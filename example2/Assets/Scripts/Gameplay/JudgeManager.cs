using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    public class JudgeManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RhythmManager _rhythmManager;
        [SerializeField] private ObjectPooler _objectPooler;
        [SerializeField] private Transform _judgmentEffectSpawnPoint; // 판정 이펙트 생성 위치 (Canvas 내)

        [Header("Judgment Settings (Seconds)")]
        [SerializeField] private float _perfectThreshold = 0.05f;
        [SerializeField] private float _greatThreshold = 0.1f;
        [SerializeField] private float _goodThreshold = 0.2f;
        [SerializeField] private float _missThreshold = 0.3f; // 이 범위 밖이면 무시하거나 Miss

        // 라인별로 판정 대기 중인 노트들을 관리하는 큐 (또는 리스트)
        private List<Queue<NoteController>> _activeNotes;

        private void Awake()
        {
            // 4키 기준 초기화
            _activeNotes = new List<Queue<NoteController>>();
            for (int i = 0; i < 4; i++)
            {
                _activeNotes.Add(new Queue<NoteController>());
            }
        }

        private void Update()
        {
            if (_rhythmManager == null || _rhythmManager.CurrentMode != RhythmManager.GameMode.Play) return;

            // 키 입력 감지
            if (Input.GetKeyDown(KeyCode.D)) ProcessInput(0);
            if (Input.GetKeyDown(KeyCode.F)) ProcessInput(1);
            if (Input.GetKeyDown(KeyCode.J)) ProcessInput(2);
            if (Input.GetKeyDown(KeyCode.K)) ProcessInput(3);

            // Miss 처리 (입력 없이 지나간 노트)
            CheckMissedNotes();
        }

        public void RegisterNote(int lane, NoteController note)
        {
            if (lane >= 0 && lane < _activeNotes.Count)
            {
                _activeNotes[lane].Enqueue(note);
            }
        }

        private void ProcessInput(int lane)
        {
            if (_activeNotes[lane].Count == 0) return;

            // 가장 먼저 생성된(가장 아래에 있는) 노트 확인
            NoteController targetNote = _activeNotes[lane].Peek();
            
            // 노트의 목표 시간과 현재 시간의 차이 계산
            double currentDspTime = _rhythmManager.CurrentTrackTime;
            float noteTime = targetNote.NoteData.Time;
            float timeDiff = Mathf.Abs(noteTime - (float)currentDspTime);

            if (timeDiff <= _missThreshold)
            {
                // 판정 범위 내에 들어옴 -> 판정 수행
                HitNote(lane, timeDiff);
            }
        }

        private void HitNote(int lane, float timeDiff)
        {
            // 큐에서 제거 및 오브젝트 비활성화
            NoteController note = _activeNotes[lane].Dequeue();
            
            // 노트 오브젝트에게 "맞았다"고 알림 (풀 반환 등)
            note.OnHit();

            // 판정 결과 결정
            string judgmentText = "";
            Color judgmentColor = Color.white;

            if (timeDiff <= _perfectThreshold)
            {
                judgmentText = "PERFECT";
                judgmentColor = new Color(0f, 1f, 1f); // Cyan
                Debug.Log("PERFECT!");
            }
            else if (timeDiff <= _greatThreshold)
            {
                judgmentText = "GREAT";
                judgmentColor = new Color(1f, 0.5f, 0f); // Orange
                Debug.Log("GREAT");
            }
            else if (timeDiff <= _goodThreshold)
            {
                judgmentText = "GOOD";
                judgmentColor = Color.green;
                Debug.Log("GOOD");
            }
            else
            {
                judgmentText = "MISS";
                judgmentColor = Color.gray;
                Debug.Log("MISS (Timing)");
            }

            // 이펙트 생성
            SpawnJudgmentEffect(judgmentText, judgmentColor);
        }

        private void CheckMissedNotes()
        {
            double currentDspTime = _rhythmManager.CurrentTrackTime;

            for (int i = 0; i < _activeNotes.Count; i++)
            {
                if (_activeNotes[i].Count > 0)
                {
                    NoteController note = _activeNotes[i].Peek();
                    float noteTime = note.NoteData.Time;

                    // 현재 시간이 노트 시간 + Miss 범위를 지났다면
                    if ((float)currentDspTime > noteTime + _missThreshold)
                    {
                        // Miss 처리
                        _activeNotes[i].Dequeue();
                        
                        Debug.Log("MISS (Passed)");
                        SpawnJudgmentEffect("MISS", Color.red);
                    }
                }
            }
        }

        private void SpawnJudgmentEffect(string text, Color color)
        {
            if (_objectPooler == null) return;

            // UI 캔버스 위 적절한 위치에 생성
            Vector3 spawnPos = _judgmentEffectSpawnPoint != null ? _judgmentEffectSpawnPoint.position : Vector3.zero;
            
            GameObject effectObj = _objectPooler.SpawnFromPool("JudgmentEffect", spawnPos, Quaternion.identity);
            if (effectObj != null)
            {
                // [수정] UI 요소이므로 Canvas 하위(SpawnPoint)로 부모를 변경해야 보임
                if (_judgmentEffectSpawnPoint != null)
                {
                    effectObj.transform.SetParent(_judgmentEffectSpawnPoint, true); // World Position 유지
                    effectObj.transform.localScale = Vector3.one; // 스케일이 꼬일 수 있으므로 1로 초기화
                    effectObj.transform.localRotation = Quaternion.identity; // 회전 초기화
                }

                JudgmentEffectController controller = effectObj.GetComponent<JudgmentEffectController>();
                if (controller != null)
                {
                    controller.Initialize(text, color, _objectPooler);
                }
            }
        }
    }
}