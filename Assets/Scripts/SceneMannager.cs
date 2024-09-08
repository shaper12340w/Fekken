using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMannger : MonoBehaviour
{
    public void ChangeSettingMenu()
    {
        SceneManager.LoadScene("SettingMenu");
    }
    
    public void ChangeSelectMenu()
    {
        SceneManager.LoadScene("SelectMenu");

    }

    public void ChangeMadeBy()
    {
        SceneManager.LoadScene("MadebyScene");
    }
    
    public void ChangeGameMenu()
    {
        SceneManager.LoadScene("GameMenu");
    }
}
