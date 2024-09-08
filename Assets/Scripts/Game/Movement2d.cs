using UnityEngine;



namespace Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Movement2D : MonoBehaviour
    {
        [SerializeField] private float      moveSpeed    = 5; //이동 속도
        [SerializeField] private float      jumpForce    = 10;
        [SerializeField] private float      lowGravity   = 2;
        [SerializeField] private float      highGravity  = 5;
        [SerializeField] private LayerMask  collisionLayer;


        public  bool        IsLongJump { set; get; }
        public  bool        isGrounded;

        private Collider2D  _collider2D;
        private Vector3     _footPosition;
        private Rigidbody2D _rigid2D; //속력 제어를 위한 Rigidbody2D

        private void Awake()
        {
            _rigid2D    = GetComponent<Rigidbody2D>();
            _collider2D  = GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            var bounds      = _collider2D.bounds;
            _footPosition = new Vector2(bounds.center.x, bounds.min.y);
            isGrounded    = Physics2D.OverlapCircle(_footPosition, 0.1f, collisionLayer);

            if (IsLongJump && _rigid2D.velocity.y > 0)
                _rigid2D.gravityScale = lowGravity;
            else
                _rigid2D.gravityScale = highGravity;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_footPosition, 0.1f);
        }

        public void MoveTo(float x)
        {
            _rigid2D.velocity = new Vector2(x * moveSpeed, _rigid2D.velocity.y);
        }

        public bool JumpTo()
        {
            if (isGrounded)
            {
                _rigid2D.velocity = new Vector2(_rigid2D.velocity.x, jumpForce);
                return true;
            }

            return false;
        }
    }
}