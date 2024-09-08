using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ButtonSelector : MonoBehaviour
    {
        public Button startButton;

        private void Start()
        {
            // 게임 시작 시 startButton을 선택 상태로 설정

            EventSystem.current.SetSelectedGameObject(startButton.gameObject);

        }

        private void Update()
        {
            // 선택된 버튼이 없을 경우, startButton을 선택 상태로 설정
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }

    }
}