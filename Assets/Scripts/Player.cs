using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode attackKey  = KeyCode.E;
    [SerializeField] private KeyCode skillKey   = KeyCode.Q;
    [SerializeField] private KeyCode backKey    = KeyCode.A;
    [SerializeField] private KeyCode frontKey   = KeyCode.D;
    [SerializeField] private KeyCode jumpKey    = KeyCode.W;
    [SerializeField] private KeyCode defenseKey = KeyCode.S;

    public  float       threshold   = 0.1f;
    public  float       hp          = 100;
    public  float       mp          = 100;
    private bool        _isGuard;
    private bool        _isFlip;
    private bool        _isMoving;
    private Vector3     _lastPosition;
    private Animator    _animation;
    private Movement2D  _movement2D;
    private Rigidbody2D _rigid2D;
    private Dictionary
        <string,GameObject> _collisonList = new Dictionary<string, GameObject>();

    private void Awake()
    {
        _movement2D = GetComponent<Movement2D>();
        _animation = GetComponent<Animator>();
        _rigid2D = GetComponent<Rigidbody2D>();
        SetCollider();
    }
    private void Update()
    {
        UpdateMove();
        UpdateJump();
        UpdateChange();
        UpdateAct();
    }



    private void SetCollider()
    {
        string[] colliderList = {"body","right_leg","left_leg","arms","head"};
        foreach (var colliders in colliderList)
        {
            _collisonList.Add(colliders,GameObject.Find(colliders));
        }
    }
    private void UpdateMove()
    {

        if (Input.GetKey(backKey) || Input.GetKey(frontKey))
        {
            _isFlip = Input.GetKey(backKey);
            transform.localScale = new Vector3(_isFlip ? -2 : 2, 2, 1);
            _movement2D.MoveTo(1.2f * (_isFlip ? -1 : 1));
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }

        //  실제로 Run 애니메이션을 실행하는 부분
        if (_isMoving && _movement2D.isGrounded)
        {
            _animation.SetBool("IsRun", true);
        }
        else
        {
            _animation.SetBool("IsRun", false);
        }
    }
    private void UpdateChange()
    {
        float displacement = Vector3.Distance(_rigid2D.transform.position, _lastPosition);

        // 변위가 임계값을 넘는 경우에만 움직임으로 간주
        _isMoving = displacement > threshold;

        _lastPosition = _rigid2D.transform.position;
    }
    private void UpdateJump()
    {
        if(_rigid2D.position.y > 0 && !_movement2D.isGrounded)
        {
            //  실제로 Jump 애니메이션을 실행하는 부분
            _animation.SetBool("IsJump", true);
            _animation.SetBool("IsRun", false);
        }
        else
        {
            _animation.SetBool("IsJump", false);
        }
        if (Input.GetKeyDown(jumpKey))
        {
            bool isJump = _movement2D.JumpTo();
        }
        else if (Input.GetKeyUp(jumpKey))
        {
            _movement2D.IsLongJump = false;
        }
        else if (Input.GetKey(jumpKey))
        {
            _movement2D.IsLongJump = true;
        }

    }

    private void UpdateAct()
    {
        //skill
        if (Input.GetKeyDown(skillKey))
        {
            switch (gameObject.tag)
            {
                case "Ninja":

                    GameObject ninjaStars = Instantiate(Resources.Load<GameObject>("Ninja_stars"),
                        new Vector3(transform.position.x + (_isFlip ? -3 : 3), transform.position.y,
                            transform.position.z), Quaternion.identity);
                    float parentDirection = _isFlip ? -1 : 1;
                    Weapon weapon = ninjaStars.GetComponent<Weapon>();
                    if (weapon != null)
                    {
                        weapon.SetDirection(parentDirection);
                    }
                    break;
            }
            _animation.SetTrigger("IsSkill");
        }

        //defense

    }

    public void Initialize(Vector3 position, bool flip, [CanBeNull] Dictionary<string, KeyCode> keyCodes)
    {
        if(keyCodes != null)
        {
            attackKey = keyCodes["attack"];
            skillKey = keyCodes["skill"];
            backKey = keyCodes["back"];
            frontKey = keyCodes["front"];
            jumpKey = keyCodes["jump"];
        }
        _rigid2D.transform.position = position;
        transform.localScale = new Vector3(flip ? -2 : 2, 2, 1);
        _isFlip = flip;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            _rigid2D.velocity = Vector2.zero;
            hp -= other.gameObject.GetComponent<Weapon>().damage;
            if (hp <= 0) hp = 0;
        }
    }



}
