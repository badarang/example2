using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RhythmGame
{
    public class ChartRecorder : MonoBehaviour
    {
        [SerializeField] private RhythmManager _rhythmManager;
        [SerializeField] private ChartData _targetChartData;

        private void Update()
        {
            if (_rhythmManager == null || _targetChartData == null) return;

            // D, F, J, K 키 매핑 (0, 1, 2, 3번 라인)
            if (Input.GetKeyDown(KeyCode.D)) RecordNote(0);
            if (Input.GetKeyDown(KeyCode.F)) RecordNote(1);
            if (Input.GetKeyDown(KeyCode.J)) RecordNote(2);
            if (Input.GetKeyDown(KeyCode.K)) RecordNote(3);
        }

        private void RecordNote(int lane)
        {
            // RhythmManager의 DSP Time을 가져옴 (float로 캐스팅)
            float time = (float)_rhythmManager.CurrentTrackTime;
            
            // 새 노트 데이터 생성
            NoteData newNote = new NoteData(time, lane);
            
            // ChartData에 추가
            _targetChartData.Notes.Add(newNote);
            
            Debug.Log($"Recorded Note - Time: {time:F3}, Lane: {lane}");

#if UNITY_EDITOR
            // 에디터에서 변경 사항을 저장하도록 설정
            EditorUtility.SetDirty(_targetChartData);
#endif
        }
    }
}