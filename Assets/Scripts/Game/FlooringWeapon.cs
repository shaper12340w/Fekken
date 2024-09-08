using System;
using UnityEngine;

namespace Game
{
    public class FlooringWeapon : Weapon
    {
        public bool         isDetachable;       // 무기 분리 가능 여부
        public float        speed;              // 무기 이동 속도
        public float        offsetX;            // 무기 이동 범위
        public float        offsetY;            // 무기 이동 범위
        public float        delayTime;          // 무기 이동 딜레이 시간

        private int         _stack;             // 무기 이동 단계
        private float       _timer;             // 경과 시간 타이머
        private Animator    _animator;          // 애니메이터 참조
        private bool        _actionTriggered;   // 무기 이동 동작이 수행되었는지 여부
        private float       _generateTime;      // 무기 생성 시간
        private Vector3     _targetPosition;    // 무기 이동 목표 위치

        public override void Start()
        {
            _animator = GetComponent<Animator>();
        }

        protected override void Use()
        {
            bool isPlaying = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
            Func<string,bool> checkName = str => _animator.GetCurrentAnimatorStateInfo(0).IsName(str);
            _timer += Time.deltaTime;
            if (_timer >= delayTime && !_actionTriggered)
                _actionTriggered = true;
            if (_actionTriggered)
            {
                if (_stack == 0)
                {
                    _animator.SetInteger(Animator.StringToHash("State"), 2);
                }
                else if (_stack == 1)
                {
                    _animator.SetInteger(Animator.StringToHash("State"), 3);
                }
                else if (_stack == 2 && checkName("second"))
                {
                    _animator.SetInteger(Animator.StringToHash("State"), 4);
                    if(isDetachable)
                        transform.SetParent(null);
                    transform.position = _targetPosition;
                }
                else if (_stack == 2 && checkName("thrid"))
                {
                    _animator.SetInteger(Animator.StringToHash("State"), 1);
                    if(!isPlaying)
                        Destroy(gameObject);
                }
            }

        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public void UpdateStack()
        {
            _stack++;
        }

        public void Init(GameObject parent)
        {
            transform.position = parent.transform.position;
            transform.SetParent(parent.transform);
            Physics2D.IgnoreCollision(parent.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
}