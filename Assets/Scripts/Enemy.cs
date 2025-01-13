using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoilling = false;

    protected float recoinTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer Sr;
    protected Animator anim;
    [SerializeField] protected Move player;
    [SerializeField] public float speed;
    [SerializeField] public float dmgMake;
    [SerializeField] protected AudioClip hurtSound;
    protected AudioSource audioSource;


    // Start is called before the first frame update

    protected enum EnemyStates
    {
        //assault wolf
        Wolf_Idle,
        Wolf_Alert,
        Wolf_Charge,
        Wolf_Suprise,
        Wolf_Stun,
        Wolf_Dead,
        //faia skuru
        Skull_Idle,
        Skull_Chase,
        Skull_Stunned,
        Skull_Dead,

        //Crt Monk
        Monk_State1,
        Monk_State2, Monk_State3, Monk_State4,
    }

    protected virtual EnemyStates GetCurrentEnemyStates
    {
        get { return currentState; }
        set
        {
            if (currentState != value)
            {
                currentState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    protected EnemyStates currentState;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = Move.Instance;
        Sr = rb.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if(GameManager.Instance.gameIsPause) return;

        if (isRecoilling)
        {
            if (recoinTimer < recoilLength)
            {
                recoinTimer += Time.deltaTime;
            }
            else
            {
                isRecoilling = false ;
                recoinTimer = 0 ;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
        
    }
    public virtual void EnemyHit(float _DmgDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _DmgDone;
        if (!isRecoilling) 
        {
            audioSource.PlayOneShot(hurtSound);
            rb.velocity = _hitForce * recoilFactor * _hitDirection;
            isRecoilling=true ;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !Move.Instance.playerStateList.Invi && !Move.Instance.playerStateList.Invi && health > 0)
        {
            Attack();
            if (Move.Instance.playerStateList.alive)
            {
                Move.Instance.HitStopTime(0, 2, 0.5f);
            }

        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }
    protected virtual void UpdateEnemyStates()
    {

    }

    protected virtual void ChangeCurrentAnimation()
    {

    }

    protected void ChangeState(EnemyStates _newstate)
    {
        GetCurrentEnemyStates = _newstate;
    }
    protected virtual void Attack()
    {
        Move.Instance.takeDmg(dmgMake);
    }
}
