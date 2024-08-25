using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class MannageButtons : MonoBehaviour
{
    private int _index;
    private string[] _buttonList = { "Person","Computer","Settings","made by"};
    [SerializeField]
    private Color buttonColor;

    private bool isButton()
    {
        return _buttonList[_index] == gameObject.name;
    }

    [CanBeNull]
    private Button GetButton()
    {
        GameObject foundObject = GameObject.Find(gameObject.name);
        return foundObject != null
            ? foundObject.GetComponent<Button>()
            : null;
    }
    private void UpdateButtons()
    {
        try
        {
            var button = GetButton();
            if (button != null)
            {
                ColorBlock cb = button.colors;
                if (isButton())
                {
                    cb.normalColor = buttonColor;
                    button.transform.localScale = new Vector3(1.05f,1.05f,1.05f);
                    button.colors = cb;
                }
                else
                {
                    cb.normalColor = Color.white;
                    button.transform.localScale = new Vector3(1f, 1f, 1f);
                    button.colors = cb;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }
    void Start()
    {
        Debug.Log("GameObject 이름: " + gameObject.name);
        Debug.Log($"Button Color : {buttonColor}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_index < _buttonList.Length - 1) _index++;
            UpdateButtons();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_index > 0) _index--;
            UpdateButtons();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            var button = GetButton();
            if (button != null && isButton())
            {
                button.onClick.Invoke();
            }
            UpdateButtons();
        }
    }

    // Start is called before the first frame update
    public void OnClickPersonMenu()
    {
        Debug.Log("OnClickPersonMenu() 가 작동됨");
    }

    public void OnClickComputerMenu()
    {
        Debug.Log("OnClickComputerMenu() 가 작동됨");
    }

    public void OnClickSettingsMenu()
    {
        Debug.Log("OnClickSettingsMenu() 가 작동됨");
    }

    public void OnClickMadeByMenu()
    {
        Debug.Log("OnClickMadeBy() 가 작동됨");
    }

    public void OnClickQuitMenu()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
