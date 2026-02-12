using System;
using UnityEngine;

namespace RhythmGame
{
    [Serializable]
    public class NoteData
    {
        [SerializeField] private float _time;
        [SerializeField] private int _lane;

        public float Time => _time;
        public int Lane => _lane;

        public NoteData(float time, int lane)
        {
            _time = time;
            _lane = lane;
        }
    }
}