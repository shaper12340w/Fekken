using UnityEngine;

namespace UI
{
    public class AudioManager : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(transform.gameObject); // 다음 scene으로 넘어가도 오브젝트 사라짐 X
        }
    }
}