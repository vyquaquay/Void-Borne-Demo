using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIreSkull : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    float stunTimer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Skull_Idle);
    }

    protected override void UpdateEnemyStates()
    {
        float _chasedist = Vector2.Distance(transform.position, Move.Instance.transform.position);
        switch (GetCurrentEnemyStates) 
        {
            case EnemyStates.Skull_Idle:
                rb.velocity = new Vector2(0, 0);
                if (_chasedist < chaseDistance)
                {
                    ChangeState(EnemyStates.Skull_Chase);
                }
                break;
            case EnemyStates.Skull_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, Move.Instance.transform.position, Time.deltaTime * speed));
                skull_flip();
                if (_chasedist > chaseDistance)
                {
                    ChangeState(EnemyStates.Skull_Idle);
                }
                break;
            case EnemyStates.Skull_Stunned:
                stunTimer += Time.deltaTime;
                if(stunTimer > stunDuration)
                {
                    ChangeState(EnemyStates.Skull_Idle);
                    stunTimer = 0;
                }
                break;
            case EnemyStates.Skull_Dead:
                Death(Random.Range(2, 5));
                break;
        }

    }

    // Update is called once per frame

    public override void EnemyHit(float _DmgDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_DmgDone, _hitDirection, _hitForce);
        if (health > 0) 
        {
            ChangeState(EnemyStates.Skull_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Skull_Dead);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyStates == EnemyStates.Skull_Idle);

        anim.SetBool("Chase", GetCurrentEnemyStates == EnemyStates.Skull_Chase);

        anim.SetBool("Stun", GetCurrentEnemyStates == EnemyStates.Skull_Stunned);

        if(GetCurrentEnemyStates == EnemyStates.Skull_Dead)
        {
            anim.SetTrigger("Death");
            int DeadCorpse = LayerMask.NameToLayer("DeadCorpse");
        }
    }
    void skull_flip()
    {
        Sr.flipX = Move.Instance.transform.position.x > transform.position.x;
    }
    protected override void Update()
    {
        base.Update();
        if (!Move.Instance.playerStateList.alive)
        {
            ChangeState(EnemyStates.Skull_Idle);
        }
    }
}
