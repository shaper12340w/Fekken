using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
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
}