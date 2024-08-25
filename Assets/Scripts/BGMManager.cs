    using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance = null;

    void Awake()
    {
        // 이미 BGMManager가 존재하는지 확인
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복된 BGMManager가 있으면 파괴
        }
    }
}