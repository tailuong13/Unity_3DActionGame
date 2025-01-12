using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float Speed = 2f;
    public int Damage = 10;
    private Rigidbody _rb;
    public ParticleSystem HitVFX;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + transform.forward * Speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character cc = other.GetComponent<Character>();
        if (cc != null && cc.isPlayer)
        {
            cc.ApplyDamage(Damage, transform.position);
        }

        Instantiate(HitVFX, transform.position, quaternion.identity);
        Destroy(gameObject);
    }
}
