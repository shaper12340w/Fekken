using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class Player : MonoBehaviour
    {
        // KeyCodes
        [SerializeField] private KeyCode attackKey  = KeyCode.E;
        [SerializeField] private KeyCode skillKey   = KeyCode.Q;
        [SerializeField] private KeyCode backKey    = KeyCode.A;
        [SerializeField] private KeyCode frontKey   = KeyCode.D;
        [SerializeField] private KeyCode jumpKey    = KeyCode.W;
        [SerializeField] private KeyCode defenseKey = KeyCode.S;

        // Player Stats
        public float        threshold      = 0.1f   ;
        public float        hp             = 100    ;
        public float        mp             = 100    ;
        public float        meleeCooltime  = 3f     ;
        public float        attackCooltime = 0.5f   ;
        public float        skillCooltime  = 1f     ;
        public float        mpCooltime     = 0.1f   ;
        public bool         isFlip                  ;

        //Cooldown
        private IEnumerator _attackCoroutine    ;
        private IEnumerator _defendCoroutine    ;
        private IEnumerator _skillCoroutine     ;
        private IEnumerator _ultimateCoroutine  ;
        private bool        _isAttackUsable     = true;
        private bool        _isDefendUsable     = true;
        private bool        _isSkillUsable      = true;
        private bool        _isUltimateUsable   = true;
        private bool        _isAttack;
        private bool        _isDefend;

        //Timer
        private bool        _isCooldown;
        private float       _updateTime;
        private float       _timer;
        private int         _stack;

        //Movement
        private bool        _isMoving;
        private Vector3     _lastPosition;

        //Components
        private Animator    _animation;
        private Movement2D  _movement2D;
        private Rigidbody2D _rigid2D;

        //Skill
        private float       _skillStack; //maxValue = 2;
        private GameObject  _weapon;

        private void Awake()
        {
            _movement2D = GetComponent<Movement2D>  ();
            _animation  = GetComponent<Animator>    ();
            _rigid2D    = GetComponent<Rigidbody2D> ();
        }

        private void Update()
        {
            UpdateMove  ();
            UpdateJump  ();
            UpdateChange();
            UpdateAct   ();
        }

        private void FixedUpdate()
        {
            UpdateTimer();
            UpdateValue();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (
                IsPlayer(other.gameObject)                                                  &&
                other.gameObject == GetOpponent()                                           &&
                _isAttack
            )
            {
                Debug.Log($"Player {gameObject.name} is hit by {other.gameObject.name}");
                other.gameObject.GetComponent<Player>().hp -= 6;
                _isAttack = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"Weapon {gameObject.name} is hit by {other.gameObject.name}");
            if (other.gameObject.CompareTag("Weapon") && _weapon != other.gameObject)
            {
                var weaponDamage = other.gameObject.GetComponent<Weapon>().damage;
                RemoveTimer();
                if (_isDefend)
                {
                    weaponDamage *= weaponDamage * (9f / 10f);
                    Debug.Log($"Weapon {gameObject.name} is defended by {other.gameObject.name}");
                }

                _rigid2D.velocity = Vector2.zero;
                hp -= weaponDamage;
                if (hp <= 0) hp = 0;
                _weapon = null;
            }
        }

        private int GetDirection()
        {
            return isFlip ? -1 : 1;
        }

        private GameObject GetOpponent()
        {
            var eventManager = GameObject.Find("EventSystem");
            if (eventManager == null)
            {
                Debug.LogError("EventSystem not found.");
                return null;
            }

            var gameManager = eventManager.GetComponent<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager component not found on EventSystem.");
                return null;
            }

            if (gameManager.leftPlayer == null || gameManager.rightPlayer == null)
            {
                Debug.LogError("leftPlayer or rightPlayer is not assigned in GameManager.");
                return null;
            }

            return gameManager.leftPlayer.gameObject == gameObject
                ? gameManager.rightPlayer.gameObject
                : gameManager.leftPlayer.gameObject;
        }

        public static bool IsPlayer(GameObject other)
        {
            return other.CompareTag("Ninja") || other.CompareTag("Magician") || other.CompareTag("Boxer");
        }


        private void UpdateMove()
        {
            if (Input.GetKey(backKey) || Input.GetKey(frontKey))
            {
                isFlip      = Input.GetKey(backKey);
                _isMoving   = true;
                transform   .localScale = new Vector3(isFlip ? -2 : 2, 2, 1);
                _movement2D .MoveTo(1.2f * GetDirection());

            }
            else
            {
                _isMoving = false;
            }

            //  실제로 Run 애니메이션을 실행하는 부분
            if (_isMoving && _movement2D.isGrounded)
                _animation.SetBool(Animator.StringToHash("IsRun"), true);
            else
                _animation.SetBool(Animator.StringToHash("IsRun"), false);
        }

        private void UpdateChange()
        {
            var displacement = Vector3.Distance(_rigid2D.transform.position, _lastPosition);
            // 변위가 임계값을 넘는 경우에만 움직임으로 간주
            _isMoving = displacement > threshold;
            _lastPosition = _rigid2D.transform.position;
            if (transform.position.y < -200)
                hp = 0;
        }

        private void UpdateJump()
        {
            if (_rigid2D.position.y > 0 && !_movement2D.isGrounded && !_animation.GetCurrentAnimatorStateInfo(0).IsName("attack_third"))
            {
                Debug.Log($"Jump {gameObject.name} is not grounded. Position: {_rigid2D.position.y}");
                //  실제로 Jump 애니메이션을 실행하는 부분
                _animation.SetBool(Animator.StringToHash("IsJump"), true);
                _animation.SetBool(Animator.StringToHash("IsRun"), false);
            }
            else
            {
                _animation.SetBool(Animator.StringToHash("IsJump"), false);
            }

            if (Input.GetKeyDown(jumpKey))
                _movement2D.JumpTo();
            else if (Input.GetKeyUp(jumpKey))
                _movement2D.IsLongJump = false;
            else if (Input.GetKey(jumpKey))
                _movement2D.IsLongJump = true;

        }

        private GameObject InitializeWeapon<T>(GameObject weaponPrefab,Collider2D thisCollider2D) where T : Weapon
        {
            var weaponObject = Instantiate(weaponPrefab,
                new Vector3(transform.position.x + (isFlip ? -3 : 3), transform.position.y,
                    transform.position.z), Quaternion.identity);
            Physics2D.IgnoreCollision(weaponObject.GetComponent<Collider2D>(), thisCollider2D);
            var weapon = weaponObject.GetComponent<T>();
            if (weapon != null) weapon.SetDirection(GetDirection());
            switch (weapon)
            {
                case MeleeWeapon meleeWeapon:
                    meleeWeapon.Init(gameObject);
                    _isAttackUsable = false;
                    _attackCoroutine = CoolTimer(attackCooltime, 0);
                    StartCoroutine(_attackCoroutine);
                    break;
                case ProjectileWeapon projectileWeapon:
                    _isSkillUsable  = false;
                    _skillCoroutine = CoolTimer(skillCooltime, 1);
                    StartCoroutine(_skillCoroutine);
                    break;
                case SpawnWeapon spawnWeapon:
                    _isUltimateUsable = false;
                    _ultimateCoroutine = CoolTimer(3f, 3);
                    StartCoroutine(_ultimateCoroutine);
                    spawnWeapon.Init(GetOpponent());
                    break;
            }
            return weaponObject;
        }
        private void UpdateAct()
        {
            var thisCollider2D = gameObject.GetComponent<Movement2D>().GetComponent<Collider2D>();

            //ultimate
            if (
                (Input.GetKey(attackKey) && Input.GetKeyDown(skillKey)  ||
                 Input.GetKey(skillKey) && Input.GetKeyDown(attackKey)) &&
                _isUltimateUsable &&
                mp > 50
                )
            {
                switch (gameObject.tag)
                {
                    case "Ninja":
                        Debug.Log("Ultimate");
                        _weapon = InitializeWeapon<SpawnWeapon>(Resources.Load<GameObject>("Ninja_ultimate"), thisCollider2D);
                        mp -= 60;
                        break;
                    case "Magician":
                        Debug.Log("Ultimate");
                        _weapon = InitializeWeapon<SpawnWeapon>(Resources.Load<GameObject>("Magician_ultimate"), thisCollider2D);
                        mp -= 60;
                        break;
                    case "Boxer":
                        Debug.Log("Ultimate");
                        _weapon = InitializeWeapon<ProjectileWeapon>(Resources.Load<GameObject>("Boxer_ultimate"), thisCollider2D);
                        mp -= 60;
                        break;
                }
            }
            //skill
            else if (Input.GetKeyDown(skillKey) && !_isCooldown && _isSkillUsable && mp > 0)
            {
                switch (gameObject.tag)
                {
                    case "Ninja":
                        _weapon = InitializeWeapon<ProjectileWeapon>(Resources.Load<GameObject>("Ninja_stars"), thisCollider2D);
                        break;
                    case "Magician":
                        _weapon = InitializeWeapon<ProjectileWeapon>(Resources.Load<GameObject>("Magician_fireball"), thisCollider2D);
                        break;
                    case "Boxer":
                        _weapon = InitializeWeapon<ProjectileWeapon>(Resources.Load<GameObject>("Boxer_rasengen"), thisCollider2D);
                        break;
                }
                mp -= 10;
                _animation.SetTrigger(Animator.StringToHash("IsSkill"));
            }
            //attack
            else if (Input.GetKeyDown(attackKey) && (_isAttackUsable || _stack != 0))
            {
                _animation.SetInteger(Animator.StringToHash("Stage"), _stack);
                switch (_stack)
                {
                    case 0:
                        switch (gameObject.tag)
                        {
                            case "Ninja":
                                _weapon = InitializeWeapon<MeleeWeapon>(Resources.Load<GameObject>("Knife"), thisCollider2D);
                                break;
                            case "Magician":
                                _weapon = InitializeWeapon<FlooringWeapon>(Resources.Load<GameObject>("Magician_flooring"), thisCollider2D);
                                break;
                            case "Boxer":
                                Dash();
                                break;
                        }
                        Debug.Log("Attack 1");
                        _animation.SetTrigger(Animator.StringToHash("IsAttack"));
                        SetTimer();
                        _stack++;
                        break;
                    case 1:
                        Debug.Log("Attack 2");
                        switch (gameObject.tag)
                        {
                            case "Ninja":
                                if (_weapon != null)
                                    _weapon.GetComponent<MeleeWeapon>().UpdateStack();
                                break;
                            case "Magician":
                                if(_weapon != null)
                                    _weapon.GetComponent<FlooringWeapon>().UpdateStack();
                                break;
                            case "Boxer":
                                Dash();
                                break;
                        }
                        SetTimer();
                        _stack++;
                        break;
                    case 2:
                        Debug.Log("Attack 3");
                        switch (gameObject.tag)
                        {
                            case "Ninja":
                                Dash();
                                break;
                            case "Magician":
                                if (_weapon != null)
                                {
                                    _weapon.GetComponent<FlooringWeapon>().SetTargetPosition(GetOpponent().transform.position);
                                    _weapon.GetComponent<FlooringWeapon>().UpdateStack();
                                }
                                break;
                            case "Boxer":
                                Dash();
                                break;
                        }
                        RemoveTimer();
                        _animation.SetTrigger(Animator.StringToHash("IsVicinity"));
                        _animation.SetBool(Animator.StringToHash("IsAttack"), false);
                        break;
                }
            }

            //defense
            if (Input.GetKeyDown(defenseKey) && _isDefendUsable)
            {
                _isDefend           = true;
                _isDefendUsable     = false;
                _defendCoroutine    = CoolTimer(1f, 2);
                StartCoroutine(_defendCoroutine);
                _animation.SetBool(Animator.StringToHash("IsDefend"), true);
            }
            else if (Input.GetKeyUp(defenseKey))
            {
                _isDefend = false;
                _animation.SetBool(Animator.StringToHash("IsDefend"), false);
            }
        }

        private void Dash()
        {
            _isAttack = true;
            var dashDirection = isFlip ? -1 : 1; // -1이면 왼쪽, 1이면 오른쪽
            var dashForce = new Vector2(dashDirection, 0) * 20; // 더 큰 힘을 적용

            // 현재 속도를 초기화하고 대시 방향으로 다시 설정
            _rigid2D.velocity = Vector2.zero;
            _rigid2D.AddForce(dashForce, ForceMode2D.Impulse);
        }

        private void UpdateValue()
        {
            if(_updateTime == 0) _updateTime = Time.time;
            if (mp < 100 && Time.time - _updateTime > mpCooltime)
            {
                mp += 2;
                _updateTime = Time.time;
            }

            if (mp > 100) mp = 100;
        }

        private IEnumerator CoolTimer(float time, int type)
        {
            yield return new WaitForSeconds(time);
            switch (type)
            {
                case 0:
                    _isAttackUsable = true;
                    break;
                case 1:
                    _isSkillUsable  = true;
                    break;
                case 2:
                    _isDefendUsable = true;
                    _isDefend       = false;
                    _animation.SetBool(Animator.StringToHash("IsDefend"), false);
                    break;
                case 3:
                    _isUltimateUsable = true;
                    break;
            }
        }

        private void SetTimer()
        {
            Debug.Log("Set Timer");
            _isCooldown = true;
            _timer      = 0f;
        }

        public void RemoveTimer()
        {
            Debug.Log("Remove Timer");
            if (_weapon != null)
                Destroy(_weapon);
            _animation.SetInteger(Animator.StringToHash("Stage"), 2);
            _stack      = 0;
            _isCooldown = false;
            _timer      = 0f;
        }

        private void UpdateTimer()
        {
            if (_isCooldown)
            {
                // 현재 남은 시간 계산
                _timer += Time.fixedDeltaTime;
                if (_timer >= meleeCooltime)
                {
                    Debug.Log("Timer End");
                    RemoveTimer(); // 타이머가 끝나면 RemoveTimer 호출
                }
            }
        }

        public void Initialize(Vector3 position, bool flip, [CanBeNull] Dictionary<string, KeyCode> keyCodes)
        {
            if (keyCodes != null)
            {
                attackKey   = keyCodes["attack"];
                skillKey    = keyCodes["skill"];
                backKey     = keyCodes["back"];
                frontKey    = keyCodes["front"];
                jumpKey     = keyCodes["jump"];
            }

            _rigid2D.transform.position = position;
            transform.localScale        = new Vector3(flip ? -2 : 2, 2, 1);
            isFlip                      = flip;
        }
    }
}