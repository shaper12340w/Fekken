using UnityEngine;

public enum WeaponType
{
    Projectile = 0, // 투사체
    Flooring = 1, // 장판
    Melee = 2, // 근접
}

public class Weapon : MonoBehaviour
{
    public float damage;
    public float speed;
    public float removeTime;
    public WeaponType weaponType;

    protected float Direction = 1f;
    protected Rigidbody2D Rigid2D;

    public virtual void Start()
    {
        Rigid2D = GetComponent<Rigidbody2D>(); // Rigidbody2D 초기화

    }

    public virtual void Update()
    {
        Use();
    }

    public virtual void SetDirection(float direction)
    {
        Direction = direction;
        transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        Rigid2D.velocity = Vector2.zero;
        bool isPlayer = other.gameObject.CompareTag("Ninja");
        if (isPlayer)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Use(){} // 무기 사용
}