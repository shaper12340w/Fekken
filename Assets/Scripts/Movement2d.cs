using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CompositeCollider2D))]

public class Movement2D : MonoBehaviour
{
    [SerializeField] private float      moveSpeed = 5; //이동 속도
    [SerializeField] private float      jumpForce = 10;
    [SerializeField] private float      lowGravity = 2;
    [SerializeField] private float      highGravity = 5;

    [SerializeField] private LayerMask  collisionLayer;
    public bool    isGrounded;
    private Vector3 footPosition;

    private Rigidbody2D         rigid2D; //속력 제어를 위한 Rigidbody2D
    private CompositeCollider2D compositeCollider2D;
    public bool IsLongJump { set; get; } = false;

    private void Awake()
    {
        rigid2D             = GetComponent<Rigidbody2D>();
        compositeCollider2D = GetComponent<CompositeCollider2D>();
        compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;

    }

    private void FixedUpdate()
    {
        Bounds bounds = compositeCollider2D.bounds;
        footPosition = new Vector2(bounds.center.x, bounds.min.y);
        isGrounded   = Physics2D.OverlapCircle(footPosition, 0.1f, collisionLayer);

        if (IsLongJump && rigid2D.velocity.y > 0)
        {
            rigid2D.gravityScale = lowGravity;
        }
        else
        {
            rigid2D.gravityScale = highGravity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(footPosition, 0.1f);
    }

    public void MoveTo(float x)
    {
        rigid2D.velocity = new Vector2(x * moveSpeed, rigid2D.velocity.y);
    }

    public bool JumpTo()
    {
        if (isGrounded)
        {
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
            return true;
        }
        else
        {
            return false;
        }
    }
}
