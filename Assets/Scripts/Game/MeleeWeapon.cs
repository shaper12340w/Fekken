using UnityEngine;

namespace Game
{
    public class MeleeWeapon : Weapon
    {
        public Transform target; // 기준이 되는 오브젝트
        public float speed;
        public float radius = 5.0f; // 회전 반지름
        public float acceleration = 2f; // 가속도
        public float maxAngle = 30.0f; // 최대 회전 각도
        public float offsetX = 0.3f; // 중점 좌표를 오른쪽으로 이동시킬 오프셋
        public float offsetY = 0.3f; // 중점 좌표를 위쪽으로 이동시킬 오프셋
        public float delayTime = 0.2f; // 지연 시간 (초)

        private bool _actionTriggered; // 동작이 수행되었는지 여부
        private float _angle;
        private int _stack; // 동작 단계
        private float _timer; // 경과 시간 타이머

        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (Player.IsPlayer(other.gameObject) && other.transform != target)
            {
                transform.parent.GetComponent<Player>().RemoveTimer();
            }
        }

        protected override void Use()
        {
            _timer += Time.deltaTime;

            if (_timer >= delayTime && !_actionTriggered)
            {
                _actionTriggered = true;
            }

            if (_actionTriggered)
            {
                PerformWeaponAction();
            }

            if (_stack == 0 && _angle >= maxAngle && _actionTriggered)
            {
                ResetWeaponAction();
            }
        }

        private void PerformWeaponAction()
        {
            if (Mathf.Abs(_angle) < Mathf.Abs(maxAngle))
            {
                transform.Translate(transform.up * 1.5f, Space.World);
            }

            if (_stack == 0 && _angle < maxAngle)
            {
                _angle += speed * Time.deltaTime;
                UpdatePosition();
            }
            else if (_stack == 1 && _angle > 0)
            {
                _angle -= speed * Time.deltaTime;
                UpdatePosition();

                if (_angle <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void ResetWeaponAction()
        {
            _timer = 0f;
            _actionTriggered = false;
            float nowDirection = GetDirectionMultiplier();
            Vector3 offsetPosition = GetOffsetPosition(nowDirection);

            transform.position = offsetPosition + GetRotationPosition(maxAngle, nowDirection);
        }

        private void UpdatePosition()
        {
            speed += acceleration;
            float nowDirection = GetDirectionMultiplier();
            Vector3 offsetPosition = GetOffsetPosition(nowDirection);

            transform.position = offsetPosition + GetRotationPosition(_angle, nowDirection);
        }

        private float GetDirectionMultiplier()
        {
            return transform.parent.GetComponent<Player>().isFlip ? -1 : 1;
        }

        private Vector3 GetOffsetPosition(float directionMultiplier)
        {
            return transform.parent.position + new Vector3(offsetX * directionMultiplier, offsetY);
        }

        private Vector3 GetRotationPosition(float angle, float directionMultiplier)
        {
            return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * directionMultiplier,
                               Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
        }

        public void UpdateStack()
        {
            _stack++;
        }

        public void Init(GameObject parent)
        {
            target = parent.transform;
            transform.SetParent(parent.transform);
            Physics2D.IgnoreCollision(parent.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
}
