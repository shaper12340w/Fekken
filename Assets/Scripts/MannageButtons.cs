using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannageButtons : MonoBehaviour
{
    void Start()
    {
        Debug.Log("test");
    }

    void Update()
    {
        
    }
    // Start is called before the first frame update
    public void OnClickPersonMenu()
    {
        Debug.Log("와 세팅!");
    }

    public void OnClickComputerMenu()
    {
        Debug.Log("와 세팅!");
    }

    public void OnClickSettingsMenu()
    {
        Debug.Log("와 세팅!");
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
