using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    public class NoteSpawner : MonoBehaviour
    {
        [SerializeField] private RhythmManager _rhythmManager;
        [SerializeField] private ObjectPooler _objectPooler;
        [SerializeField] private ChartData _chartData;
        [SerializeField] private JudgeManager _judgeManager; // 추가
        
        [Header("Settings")]
        [SerializeField] private float _spawnAdvanceTime = 2.0f; // 노트가 판정선에 닿기 몇 초 전에 생성할지
        [SerializeField] private float _laneWidth = 1.5f; // 라인 간격
        [SerializeField] private float _scrollSpeed = 5.0f; // 노트 이동 속도

        private List<NoteData> _sortedNotes;
        private int _nextNoteIndex = 0;

        private void Start()
        {
            if (_chartData != null)
            {
                // 시간 순 정렬 (혹시 모를 데이터 꼬임 방지)
                _sortedNotes = new List<NoteData>(_chartData.Notes);
                _sortedNotes.Sort((a, b) => a.Time.CompareTo(b.Time));
            }
        }

        private void Update()
        {
            if (_rhythmManager == null || !_rhythmManager.IsPlaying || _sortedNotes == null) return;

            // [추가] Play 모드가 아니면 스폰하지 않음
            if (_rhythmManager.CurrentMode != RhythmManager.GameMode.Play) return;

            double currentDspTime = _rhythmManager.CurrentTrackTime;

            // 윈도우 기반 스폰: 현재 시간 + 미리보기 시간(SpawnAdvanceTime)보다 노트 시간이 작으면 스폰
            while (_nextNoteIndex < _sortedNotes.Count)
            {
                NoteData noteData = _sortedNotes[_nextNoteIndex];

                if (noteData.Time <= currentDspTime + _spawnAdvanceTime)
                {
                    SpawnNote(noteData);
                    _nextNoteIndex++;
                }
                else
                {
                    // 시간 순 정렬되어 있으므로, 조건을 만족하지 않으면 이후 노트도 만족하지 않음
                    break;
                }
            }
        }

        private void SpawnNote(NoteData noteData)
        {
            // 라인에 따른 X 위치 계산 (예: 4키 기준 중앙 정렬)
            // 0 -> -2.25, 1 -> -0.75, 2 -> 0.75, 3 -> 2.25 (LaneWidth 1.5 기준)
            float xPos = (noteData.Lane - 1.5f) * _laneWidth;
            
            // 생성 위치: 위쪽(Y축) 또는 깊이(Z축) 등 게임 방식에 따라 다름. 여기서는 2D 세로 스크롤 가정 (Y축 위에서 생성)
            // 실제 노트의 이동 로직은 NoteController에서 처리하겠지만, 초기 위치는 여기서 잡아줌.
            // 스폰 위치는 판정선 위치 + (속도 * 미리보기 시간) 등으로 계산 가능하나, 
            // 일단은 고정된 상단 위치나, NoteController가 초기화될 때 처리하도록 함.
            Vector3 spawnPos = new Vector3(xPos, 6.0f, 0); // 임시 Y값 6.0f

            GameObject noteObj = _objectPooler.SpawnFromPool("Note", spawnPos, Quaternion.identity);
            
            if (noteObj != null)
            {
                // 노트 객체에 데이터 전달 (이후 NoteController 구현 시 필요)
                NoteController controller = noteObj.GetComponent<NoteController>();
                if (controller != null)
                {
                    controller.Initialize(noteData, _rhythmManager, _objectPooler, _scrollSpeed, _judgeManager);
                }
            }
        }
    }
}