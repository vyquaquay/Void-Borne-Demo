using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpririt : Enemy
{
    [SerializeField] private float flipWaittime;
    [SerializeField] private float ledgecheckX;
    [SerializeField] private float ledgecheckY;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private float stunDuration;
    [SerializeField] private LayerMask whatIsGround;
    float timer;
    float stunTimer;
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12.0f;
        ChangeState(EnemyStates.Wolf_Idle);
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    // Update is called once per frame
    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }
        Vector3 _ledgeCheckStartPoint = transform.localScale.x > 0 ? new Vector3(ledgecheckX, 0) : new Vector3(-ledgecheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        switch (GetCurrentEnemyStates) 
        {
            case EnemyStates.Wolf_Idle:
                
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, Vector2.down, ledgecheckY, whatIsGround) || Physics2D.Raycast(transform.position,
                    _wallCheckDir, ledgecheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Wolf_Alert);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, _wallCheckDir, ledgecheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.Wolf_Suprise);
                }

                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }

                break;
            case EnemyStates.Wolf_Alert:
                timer += Time.deltaTime;
                if (timer > flipWaittime) 
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1,transform.localScale.y);
                    ChangeState(EnemyStates.Wolf_Idle);
                }

                break;
            case EnemyStates.Wolf_Suprise:
                rb.velocity = new Vector2(0, jumpForce);
                ChangeState (EnemyStates.Wolf_Charge);
                break;
            case EnemyStates .Wolf_Charge:
                timer += Time.deltaTime;
                if (timer < chargeDuration)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgecheckY, whatIsGround))
                    {
                        if (transform.localScale.x > 0)
                        {
                            rb.velocity = new Vector2(speed * chargeSpeed, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-speed * chargeSpeed, rb.velocity.y);
                        }

                    }
                    else
                    {
                        rb.velocity = new Vector2(0,rb.velocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.Wolf_Idle);
                }
                break;
            case EnemyStates.Wolf_Stun:
                stunTimer += Time.deltaTime;
                if (stunTimer > stunDuration)
                {
                    ChangeState(EnemyStates.Wolf_Idle);
                    stunTimer = 0;
                    timer = 0;
                }
                break;
            case EnemyStates.Wolf_Dead:
                Death(Random.Range(1, 3));
                break;
        }

    }

    public override void EnemyHit(float _DmgDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_DmgDone, _hitDirection, _hitForce);
        if (health > 0)
        {
            ChangeState(EnemyStates.Wolf_Stun);
        }
        else
        {
            ChangeState(EnemyStates.Wolf_Dead);
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Patrol", GetCurrentEnemyStates == EnemyStates.Wolf_Idle);
        anim.SetBool("Found", GetCurrentEnemyStates == EnemyStates.Wolf_Charge);
        anim.SetBool("Stunned", GetCurrentEnemyStates == EnemyStates.Wolf_Stun);
        if (GetCurrentEnemyStates == EnemyStates.Wolf_Idle)
        {
            anim.speed = 1;
        }

        if (GetCurrentEnemyStates == EnemyStates.Wolf_Charge)
        {
            anim.speed = chargeSpeed;
        }
        if (GetCurrentEnemyStates == EnemyStates.Wolf_Dead)
        {
            anim.SetTrigger("Death");
            int DeadCorpse = LayerMask.NameToLayer("DeadCorpse");
        }
    }


    protected override void Update()
    {
        base.Update();
        if (!Move.Instance.playerStateList.alive)
        {
            ChangeState(EnemyStates.Wolf_Idle);
        }
    }
}
