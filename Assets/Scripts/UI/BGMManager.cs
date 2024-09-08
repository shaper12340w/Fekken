using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class BGMManager : MonoBehaviour
    {
        private static BGMManager instance;
        private AudioSource bgmAudioSource;
        private bool shouldPlayBGMOnReturn = false; // GameMenu로 돌아올 때 BGM을 재생할지 여부를 결정하는 변수

        private void Awake()
        {
            // 이미 BGMManager가 존재하는지 확인
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
            }
            else if (instance != this)
            {
                Destroy(gameObject); // 중복된 BGMManager가 있으면 파괴
                return;
            }

            // AudioSource 가져오기
            bgmAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            // 씬 전환 이벤트 리스너 등록
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene previousScene, Scene newScene)
        {
            if (newScene.name == "BattleScene") // BattleScene으로 전환될 때
            {
                StopBGM();
                shouldPlayBGMOnReturn = false; // BattleScene에서는 BGM 재생하지 않음
            }
            else if (newScene.name == "SelectMenu") // SelectMenu으로 전환될 때
            {
                StopBGM();
                shouldPlayBGMOnReturn = true; // GameMenu로 돌아올 때 BGM을 재생하도록 설정
            }
            else if (newScene.name == "GameMenu") // GameMenu로 전환될 때
            {
                if (shouldPlayBGMOnReturn)
                {
                    PlayBGM();
                }
                // GameMenu로 돌아올 때만 BGM을 재생하도록 설정하고, 다시는 BGM을 재생하지 않도록 초기화
                shouldPlayBGMOnReturn = false;
            }
        }

        private void StopBGM()
        {
            if (bgmAudioSource != null && bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Pause(); // BGM을 일시 정지
            }
        }

        private void PlayBGM()
        {
            if (bgmAudioSource != null)
            {
                bgmAudioSource.Play(); // BGM을 재생
            }
        }

        private void OnDestroy()
        {
            // 씬 전환 이벤트 리스너 제거
            if (instance == this)
            {
                SceneManager.activeSceneChanged -= OnSceneChanged;
            }
        }
    }
}