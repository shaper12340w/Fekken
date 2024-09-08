using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float time;
        [SerializeField] private float curTime;

        private int second;

        private void Awake()
        {
            time = 31;
            StartCoroutine(StartTimer());
        }


        private IEnumerator StartTimer()
        {
            curTime = time;
            while (curTime > 0)
            {
                SetSelectedImage();
                curTime -= Time.deltaTime;
                second = (int)curTime % 60;
                text.text = second.ToString("00");
                yield return null;

                if (curTime <= 0)
                {
                    Debug.Log("시간 종료");
                    curTime = 0;
                    SceneManager.LoadScene("BattleScene");
                    CheckPlayer();
                    yield break;
                }
            }
        }

        private void CheckPlayer()
        {
            // 플레이어가 선택한 캐릭터를 확인하는 함수
            var canvas = GameObject.Find("Canvas");
            var leftPlayer = canvas.transform.Find("P1_List");
            var rightPlayer = canvas.transform.Find("P2_List");


            var leftPlayerName = leftPlayer.GetComponent<SingleButtonSelector>().GetSelectedButton().GetChild(0)
                .GetComponent<Text>().text;
            var rightPlayerName = rightPlayer.GetComponent<SingleButtonSelector>().GetSelectedButton().GetChild(0)
                .GetComponent<Text>().text;

            Debug.Log("왼쪽 플레이어: " + leftPlayerName);
            Debug.Log("오른쪽 플레이어: " + rightPlayerName);

            PlayerPrefs.SetString("LeftPlayer", leftPlayerName);
            PlayerPrefs.SetString("RightPlayer", rightPlayerName);
        }

        private void SetSelectedImage()
        {

            var canvas = GameObject.Find("Canvas");

            var currentlySelectedButton = canvas.transform.Find("P1_List").GetComponent<SingleButtonSelector>()
                .GetSelectedButton();
            if (currentlySelectedButton != null)
                canvas.transform.Find("P1_Illustration").GetComponent<SpriteRenderer>().sprite =
                    currentlySelectedButton.GetComponent<Button>().image.sprite;

            var currentlySelectedButton2 = canvas.transform.Find("P2_List").GetComponent<SingleButtonSelector>()
                .GetSelectedButton();
            if (currentlySelectedButton2 != null)
                canvas.transform.Find("P2_Illustration").GetComponent<SpriteRenderer>().sprite =
                    currentlySelectedButton2.GetComponent<Button>().image.sprite;
        }
    }
}