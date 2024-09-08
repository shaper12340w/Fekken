using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer; // 스프라이트 렌더러 참조
        [SerializeField] private Button yourButton; // UI 버튼 참조



        private void Start()
        {
            // 시작할 때 스프라이트를 비활성화
            spriteRenderer.enabled = false;

            // 버튼 클릭 시 AppearImage01 메서드 호출
            yourButton.onClick.AddListener(AppearImage01);
        }



        private void AppearImage01()
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // 버튼이 눌릴 때마다 스프라이트의 활성화 상태를 전환
        }
    }
}