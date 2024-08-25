using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{

    private float _generateTime;

    protected override void Use()
    {
        if (_generateTime == 0)
        {
            _generateTime = Time.time;
        }
        if (Time.time - _generateTime > removeTime)
        {
            Destroy(gameObject);
        }

        if (Rigid2D != null) // Rigidbody2D가 null이 아닐 경우만 처리
        {
            Rigid2D.AddForce(Vector2.right * speed * Direction, ForceMode2D.Force);

        }
    }
}
