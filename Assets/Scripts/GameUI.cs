using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [SerializeField] private Slider leftSlider;
    [SerializeField] private Slider rightSlider;
    //UI
    [SerializeField] private TMP_Text text;

    [SerializeField] private float time;
    [SerializeField] private float curTime;

    public Coroutine TimerCoroutine;
    private GameManager _gameManager;
    void Start()
    {
        time = 60;
        TimerCoroutine = StartCoroutine(StartTimer());
        _gameManager = gameObject.GetComponent<GameManager>();
    }

    IEnumerator StartTimer()
    {
        curTime = time;
        while(curTime > 0)
        {
            curTime -= Time.deltaTime;
            text.text = ((int)curTime % 60).ToString("00");
            yield return null;

            if(curTime <= 0)
            {
                Debug.Log("시간 종료");
                _gameManager.EndGame(EndState.TimeOver);
                curTime = 0;
                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameManager.gameState == GameState.Playing)
        {
            leftSlider.value = _gameManager.leftPlayer.hp;
            rightSlider.value = _gameManager.rightPlayer.hp;
        }
    }
}
