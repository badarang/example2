using UnityEngine;

namespace RhythmGame
{
    public class RhythmManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private double _dspStartTime;
        private bool _isPlaying = false;

        /// <summary>
        /// 현재 재생 중인 트랙의 진행 시간 (DSP Time 기준)
        /// </summary>
        public double CurrentTrackTime => AudioSettings.dspTime - _dspStartTime;

        public bool IsPlaying => _isPlaying;

        private void Awake()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        private void Start()
        {
            // 테스트를 위해 시작 시 바로 재생
            if (_audioSource != null && _audioSource.clip != null)
            {
                PlayMusic();
            }
            else
            {
                Debug.LogWarning("[RhythmManager] AudioSource or AudioClip is missing.");
            }
        }

        private void Update()
        {
            if (_isPlaying)
            {
                // 예상 상황: 콘솔에 정확한 소수점 단위의 dspTime 출력
                Debug.Log($"Current Track Time (DSP): {CurrentTrackTime:F6}");
            }
        }

        public void PlayMusic()
        {
            _dspStartTime = AudioSettings.dspTime;
            _audioSource.Play();
            _isPlaying = true;
        }
    }
}