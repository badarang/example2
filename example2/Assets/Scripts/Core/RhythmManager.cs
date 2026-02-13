using UnityEngine;

namespace RhythmGame
{
    public class RhythmManager : MonoBehaviour
    {
        public enum GameMode
        {
            Play, // 일반 플레이 모드 (노트 스폰 및 판정)
            Edit  // 채보 제작 모드 (노트 기록)
        }

        [Header("Game Settings")]
        [SerializeField] private GameMode _gameMode = GameMode.Play;
        [SerializeField] private AudioSource _audioSource;

        private double _dspStartTime;
        private bool _isPlaying = false;

        /// <summary>
        /// 현재 재생 중인 트랙의 진행 시간 (DSP Time 기준)
        /// </summary>
        public double CurrentTrackTime => AudioSettings.dspTime - _dspStartTime;

        public bool IsPlaying => _isPlaying;
        public GameMode CurrentMode => _gameMode;

        private void Awake()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            if (_audioSource != null)
            {
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 0f; 
                _audioSource.mute = false;
            }
            
            if (FindObjectOfType<AudioListener>() == null)
            {
                Debug.LogError("[RhythmManager] 씬에 AudioListener가 없습니다! Main Camera에 AudioListener 컴포넌트가 있는지 확인하세요.");
            }
        }

        private void Start()
        {
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
                // 디버그 로그는 개발 중에만 유용하므로 필요 시 주석 처리
                // Debug.Log($"Current Track Time (DSP): {CurrentTrackTime:F6}");
            }
        }

        public void PlayMusic()
        {
            _dspStartTime = AudioSettings.dspTime + 0.1;
            _audioSource.PlayScheduled(_dspStartTime);
            _isPlaying = true;
        }

        private void OnDrawGizmos()
        {
            // 판정선 시각화 (Y=0)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(-10, 0, 0), new Vector3(10, 0, 0));
        }
    }
}