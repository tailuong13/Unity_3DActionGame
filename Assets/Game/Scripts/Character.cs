using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float MoveSpeed = 5.0f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;
    private float _verticalVelocity;
    public float Gravity = -9.8f;
    private Animator _animator;

    //enemy
    public bool isPlayer = true;
    private NavMeshAgent _navMeshAgent;
    private Transform _targetPlayer;

    //state machines
    public enum CharacterState
    {
        Normal,
        Attacking,
        Dead,
        BeingHit
    }
    public CharacterState CurrentState;
    
    //heath
    private Health _health;
    
    //coin
    public int coin;
    
    //plauer slides
    private float _attackStartTime;
    public float attackSlideDuration = 0.1f;
    public float attackSlideSpeed = 0.5f;
    
    //DamageCaster
    private DamageCaster _damageCaster;
    
    //Material Animations
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    
    //Item to Drop
    public GameObject itemToDrop;

    //Impact on Player
    private Vector3 _imnpactOnCharacter;
    
    //Invinsible
    public bool isInvinsible;
    public float invinsibleDuration = 2f;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
        
        if (!isPlayer)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _targetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }

    private void CalculatedPlayerMovement()
    {
        if(_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        
        _movementVelocity.Set(_playerInput.HorizontalInput, 0, _playerInput.VerticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;

        _animator.SetFloat("Speed", _movementVelocity.magnitude);

        _movementVelocity *= MoveSpeed * Time.deltaTime;

        if (_movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }

    private void CalculateEnemyMovement()
    {
        if (Vector3.Distance(_targetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(_targetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);
            
            SwitchStateTo(CharacterState.Attacking);
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                {
                    CalculatedPlayerMovement();
                }
                else
                {
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if (isPlayer)
                {

                    if (Time.time < _attackStartTime + attackSlideDuration)
                    {
                        float timePassed = Time.time - _attackStartTime;
                        float lerpTime = timePassed / attackSlideDuration;
                        _movementVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                if(_imnpactOnCharacter.magnitude > 0.2f)
                {
                    _movementVelocity= _imnpactOnCharacter * Time.deltaTime;
                }
                _imnpactOnCharacter = Vector3.Lerp(_imnpactOnCharacter, Vector3.zero, 5f * Time.deltaTime);
                break;
        }
        
        if (isPlayer)
        {
            if (_cc.isGrounded == false)
            {
                _verticalVelocity = Gravity;
            }
            else
            {
                _verticalVelocity = Gravity * 0.3f;
            }

            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;
            
            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }
    }

    public void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            _playerInput.MouseButtonDown = false;
        }
        
        //Exiting case
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (_damageCaster)
                {
                    DisableDamageCaster();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
        }
        
        
        //Entering case
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (!isPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(_targetPlayer.position - transform.position);
                    transform.rotation = newRotation;                                                           
                }
                
                _animator.SetTrigger("Attack");

                if (isPlayer)
                {
                    _attackStartTime = Time.time;
                }
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");

                if (isPlayer)
                {
                    isInvinsible = true;
                    StartCoroutine(DelayedInvisible());
                }
                
                break;
        }
        
        CurrentState = newState;
        
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void ApplyDamage(int damage, Vector3 attackPos = new Vector3())
    {
        if (isInvinsible)
        {
            return;
        }
        
        if (_health)
        {
            _health.ApplyDamage(damage);
        }

        if (!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackPos);
        }

        StartCoroutine(MaterialBlink());

        if (isPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackPos, 10f);
        }
    }

    private void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        _imnpactOnCharacter = impactDir * force;
    }
    
    public void EnableDamageCaster()
    {
        if (_damageCaster)
        {
            _damageCaster.EnableDamageCaster();
        }
    }
    
    public void DisableDamageCaster()
    {
        if (_damageCaster)
        {
            _damageCaster.DisableDamageCaster();
        }
    }

    IEnumerator DelayedInvisible()
    {
        yield return new WaitForSeconds(invinsibleDuration);
        isInvinsible = false;
    }

    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        
        yield return new WaitForSeconds(0.2f);
        
        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2f);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHeight_start = 20f;
        float dissolveHeight_end = -10f;
        float dissolveHeight;
        
        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        
        while(currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_end, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            
            yield return null;
        }
        
        DropItem();
    }
    
    private void DropItem()
    {
        if (itemToDrop)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.pickUpType)
        {
            case PickUp.PickUpType.Health:
                AddHealth(item.value);
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(item.value);
                break;
        }
    }
    
    private void AddHealth(int health)
    {
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().PlayHealthVFX();
    }
    
    private void AddCoin(int value)
    {
        coin += value;
    }
}
