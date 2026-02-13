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

            // [추가] Edit 모드가 아니면 기록하지 않음
            if (_rhythmManager.CurrentMode != RhythmManager.GameMode.Edit) return;

            // D, F, J, K 키 매핑 (0, 1, 2, 3번 라인)
            if (Input.GetKeyDown(KeyCode.D)) RecordNote(0);
            if (Input.GetKeyDown(KeyCode.F)) RecordNote(1);
            if (Input.GetKeyDown(KeyCode.J)) RecordNote(2);
            if (Input.GetKeyDown(KeyCode.K)) RecordNote(3);
        }

        private void RecordNote(int lane)
        {
            float time = (float)_rhythmManager.CurrentTrackTime;
            
            NoteData newNote = new NoteData(time, lane);
            
            _targetChartData.Notes.Add(newNote);
            
            Debug.Log($"Recorded Note - Time: {time:F3}, Lane: {lane}");

#if UNITY_EDITOR
            EditorUtility.SetDirty(_targetChartData);
#endif
        }
    }
}