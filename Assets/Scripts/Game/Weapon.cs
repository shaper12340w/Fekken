using UnityEngine;

public enum WeaponType
{
    Projectile = 0, // 투사체
    Flooring = 1, // 장판
    Melee = 2, // 근접
    Spawn = 3 // 소환
}

namespace Game
{
    public class Weapon : MonoBehaviour
    {
        public      float       damage;
        public      float       removeTime;
        public      WeaponType  weaponType;

        protected   float       Direction = 1f;
        protected   Transform   Parent;
        protected   Rigidbody2D Rigid2D;

        public virtual void Start()
        {
            Rigid2D = GetComponent<Rigidbody2D>(); // Rigidbody2D 초기화
            if (transform.parent != null)
                Parent = transform.parent;
        }

        public virtual void Update()
        {
            Use();
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (Rigid2D != null)
                Rigid2D.velocity = Vector2.zero;

            if (Player.IsPlayer(other.gameObject) && other.transform != Parent && typeof(SpawnWeapon) != GetType())
                Destroy(gameObject);
        }

        public void SetDirection(float direction)
        {
            Direction = direction;
            transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y,
                transform.localScale.z);
        }

        protected virtual void Use()
        {
        } // 무기 사용


    }
}