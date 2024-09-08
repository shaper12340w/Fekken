using UnityEngine;
using System.Collections;

public partial class AudioManager : MonoBehaviour {

    void Start () {

        DontDestroyOnLoad(transform.gameObject); // 다음 scene으로 넘어가도 오브젝트 사라짐 X

    }
}
