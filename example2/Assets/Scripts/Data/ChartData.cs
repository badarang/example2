using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    [CreateAssetMenu(fileName = "NewChartData", menuName = "RhythmGame/Chart Data")]
    public class ChartData : ScriptableObject
    {
        [SerializeField] private string _songName;
        [SerializeField] private float _bpm;
        [SerializeField] private float _baseScrollSpeed;
        [SerializeField] private List<NoteData> _notes = new List<NoteData>();

        public string SongName => _songName;
        public float Bpm => _bpm;
        public float BaseScrollSpeed => _baseScrollSpeed;
        public List<NoteData> Notes => _notes;
    }
}