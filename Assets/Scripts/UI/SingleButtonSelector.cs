using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SingleButtonSelector : MonoBehaviour
    {
        public Image[] buttonImages; // 버튼 이미지 연결
        public byte defaultTransparency = 20; // 기본 투명도 (0 ~ 255)
        public byte selectedTransparency = 200; // 선택 시 투명도 (0 ~ 255)
        private Dictionary<Button, bool> isButtonSelected = new Dictionary<Button, bool>();
        private Button currentlySelectedButton = null; // 현재 선택된 버튼

        private void Start()
        {
            foreach (Transform child in transform)
            {
                var button = child.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(OnButtonClick);
                    isButtonSelected.Add(button, false);
                }
            }

            if (buttonImages.Length > 0)
            {
                SetImage();
                foreach (Button button in isButtonSelected.Keys)
                {
                    SetTransparency(defaultTransparency, button);
                }
            }
        }


        private Button GetCurrentButton()
        {
            var selectedObject = EventSystem.current.currentSelectedGameObject;
            return selectedObject != null ? selectedObject.GetComponent<Button>() : null;
        }

        private void OnButtonClick()
        {
            var currentButton = GetCurrentButton();
            if (currentButton != null && isButtonSelected.ContainsKey(currentButton))
            {
                if (currentlySelectedButton != null && currentlySelectedButton != currentButton)
                {
                    // 다른 버튼이 선택되어 있는 경우, 기본 상태로 변경
                    SetTransparency(defaultTransparency, currentlySelectedButton);
                    isButtonSelected[currentlySelectedButton] = false;
                }

                if (!isButtonSelected[currentButton])
                {
                    // 버튼이 선택되지 않은 상태 -> 선택된 상태로 변경
                    SetTransparency(selectedTransparency, currentButton);
                    isButtonSelected[currentButton] = true;
                    currentlySelectedButton = currentButton;
                }
                else
                {
                    // 버튼이 이미 선택된 상태 -> 기본 상태로 변경
                    SetTransparency(defaultTransparency, currentButton);
                    isButtonSelected[currentButton] = false;
                    currentlySelectedButton = null;
                }
            }
        }



        private void SetImage()
        {
            int count = 0;
            foreach (Button button in isButtonSelected.Keys)
            {
                if (count < buttonImages.Length)
                {
                    button.image = buttonImages[count];
                    count++;
                }
            }
        }

        private void SetTransparency(byte transparency, Button button)
        {
            if (button != null)
            {
                var color = button.image.color;
                color.a = transparency / 255f;
                button.image.color = color;
            }
        }

        public Transform GetSelectedButton()
        {
            return currentlySelectedButton?.transform;
        }

        public string GetSelectedButtonText()
        {
            return currentlySelectedButton?.GetComponentInChildren<Text>().text;
        }
    }
}
