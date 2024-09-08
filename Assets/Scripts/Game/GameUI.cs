using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Slider     leftHealthSlider;
        [SerializeField] private Slider     rightHealthSlider;
        [SerializeField] private Slider     leftMagicSlider;
        [SerializeField] private Slider     rightMagicSlider;
        //UI
        [SerializeField] private TMP_Text   text;
        [SerializeField] private float      time;
        [SerializeField] private float      curTime;

        private GameManager     _gameManager;
        public  Coroutine       TimerCoroutine;


        private void Start()
        {
            time            = 60;
            TimerCoroutine  = StartCoroutine(StartTimer());
            _gameManager    = gameObject.GetComponent<GameManager>();
            SetTheme();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_gameManager.gameState == GameState.Playing)
            {
                leftHealthSlider.value    = _gameManager.leftPlayer .hp;
                rightHealthSlider.value   = _gameManager.rightPlayer.hp;
                leftMagicSlider.value     = _gameManager.leftPlayer .mp;
                rightMagicSlider.value    = _gameManager.rightPlayer.mp;
            }
        }

        private IEnumerator StartTimer()
        {
            curTime = time;
            while (curTime > 0)
            {
                curTime     -= Time.deltaTime;
                text.text   = ((int)curTime % 60).ToString("00");
                yield return null;

                if (curTime <= 0)
                {
                    Debug.Log("시간 종료");
                    _gameManager.EndGame(EndState.TimeOver);

                    curTime = 0;
                    yield break;
                }
            }
        }

        private void SetTheme()
        {
            var theme = PlayerPrefs.GetString("Theme");
            GameObject.Find("BackgroundCanvas").transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Theme/{theme}");
        }
    }
}