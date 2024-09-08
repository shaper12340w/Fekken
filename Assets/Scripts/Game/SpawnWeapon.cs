using UnityEngine;

namespace Game
{
    public class SpawnWeapon : Weapon
    {
        private Animator    _anim;
        private float       _generateTime;
        public override void Start()
        {
            _anim = GetComponent<Animator>();
        }

        protected override void Use()
        {
            if (_generateTime == 0)                                                 _generateTime = Time.time;
            if (Time.time - _generateTime > removeTime)                             Destroy(gameObject);
            if(_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) Destroy(gameObject);
        }

        public void Init(GameObject prefab)
        {
            transform.position = new Vector3(
                prefab.transform.position.x,
                prefab.transform.position.y,
                prefab.transform.position.z);
        }
    }
}