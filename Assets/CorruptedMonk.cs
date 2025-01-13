using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedMonk : Enemy
{
    public static CorruptedMonk Instance;

    [SerializeField] private AudioClip changestateSound;

    [Header("Attacking setting")]
    [SerializeField] public Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] public Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [Space(5)]

    public float attackRange;
    public float attackTimer;

    [Header("Groundcheck setting")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float groundchecky = 0.2f;
    [SerializeField] private float groundcheckx = 0.5f;
    [SerializeField] private LayerMask isground;

    int hitCounter;
    bool stunned, canStunned;
    bool alive;
    public bool damagedPlayer = false;

    [HideInInspector] public float runSpeed;
    [HideInInspector] public bool facingRight;
    [HideInInspector] public bool parrying;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.Monk_State1);
        alive = true;
    }

    public bool grounded;
    public bool isonGround()
    {
        if (Physics2D.Raycast(groundcheck.position, Vector2.down, groundchecky, isground)
            || Physics2D.Raycast(groundcheck.position + new Vector3(groundcheckx, 0, 0), Vector2.down, groundchecky, isground)
            || Physics2D.Raycast(groundcheck.position + new Vector3(-groundcheckx, 0, 0), Vector2.down, groundchecky, isground)
            )
        {
            grounded = true;
            return true;
        }
        else
        {
            grounded = false;
            return false;
        }
    }

    public bool touchWall()
    {
        if (Physics2D.Raycast(wallCheck.position, Vector2.down, groundchecky, isground)
            || Physics2D.Raycast(wallCheck.position + new Vector3(groundcheckx, 0, 0), Vector2.down, groundchecky, isground)
            || Physics2D.Raycast(wallCheck.position + new Vector3(-groundcheckx, 0, 0), Vector2.down, groundchecky, isground)
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (health <= 0 && alive)
        {
            Debug.Log("Update: Health is 0, calling Death()");
            Death(0);
        }

        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }
        if (stunned)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Flip()
    {
        if (Move.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            facingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            facingRight = true;
        }
    }


    protected override void UpdateEnemyStates()
    {
        if(Move.Instance != null)
        {
            switch (GetCurrentEnemyStates)
            {
                case EnemyStates.Monk_State1:
                    canStunned = true;
                    attackTimer = 4;
                    runSpeed = speed;
                    dmgMake = 2;
                    break;
                case EnemyStates.Monk_State2:
                    new WaitForSeconds(1f);
                    anim.SetBool("Changestate", false);
                    audioSource.PlayOneShot(changestateSound);
                    new WaitForSeconds(1f);
                    canStunned = true;
                    attackTimer = 2;
                    runSpeed = speed * 1.1f;
                    dmgMake = 3;
                    break;
            }
        } 
    }

    protected override void OnCollisionStay2D(Collision2D _other)
    {
       
    }

    public void ResetAllAttack()
    {
        attacking = false;
        StopCoroutine(TriplePunch());
        StopCoroutine(SweepKick());
        StopCoroutine(Parry());
        StopCoroutine(payback());

        flyingKichAttack = false;
    }

    #region attacking
    #region variables
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    [HideInInspector] public Vector2 moveToPosition;
    [HideInInspector] public bool flyingKichAttack;
    public GameObject divingCollider;
    public GameObject pillar;
    #endregion

    #region Control
    
    public void AttackHandler()
    {
        if(currentState == EnemyStates.Monk_State1)
        {
            if(Vector2.Distance(Move.Instance.transform.position,rb.position) <= attackRange)
            {
                StartCoroutine(TriplePunch());
            }
            else
            {
                StartCoroutine(SweepKick());
            }
        }
        if (currentState == EnemyStates.Monk_State2)
        {
            if (Vector2.Distance(Move.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TriplePunch());
            }
            else
            {
                int attackChoose = Random.Range(1,2);
                if(attackChoose == 1){
                    StartCoroutine(SweepKick());
                }
                if (attackChoose == 2)
                {
                    FlyingKickAttackJump();
                }
            }
        }
    }

    #endregion

    #region Stage1
    IEnumerator TriplePunch()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Punch");
        Debug.Log("First punch");
        yield return new WaitForSeconds(1f);
        anim.ResetTrigger("Punch");


        anim.SetTrigger("Punch");
        Debug.Log("Second punch");
        yield return new WaitForSeconds(0.7f);
        anim.ResetTrigger("Punch");


        anim.SetTrigger("Punch");
        Debug.Log("Third punch");
        yield return new WaitForSeconds(0.3f);
        anim.ResetTrigger("Punch");

        ResetAllAttack();
    }

    IEnumerator SweepKick()
    {
        Flip();
        attacking = true ;
        Debug.Log("Kich start");
        anim.SetBool("Kick",true);
        yield return new WaitForSeconds(1f);
        Debug.Log("Kich hit");
        anim.SetBool("Kick", false);
        damagedPlayer = false;
        ResetAllAttack() ;
    }

    IEnumerator Parry()
    {
        attacking=true ;
        rb.velocity = Vector2.zero;
        Debug.Log("start Parry");
        anim.SetBool("Parry", true);
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("Parry", false);

        parrying = false ;
        ResetAllAttack();
    }

    #endregion
    #endregion

    public override void EnemyHit(float _DmgDone, Vector2 _hitDirection, float _hitForce)
    {

        if (!stunned)
        {

            if (!parrying)
            {
                if (canStunned)
                {
                    hitCounter++;
                    if(hitCounter >= 6)
                    {
                        ResetAllAttack();
                        StartCoroutine(Stunned());
                    }
                }
                base.EnemyHit(_DmgDone, _hitDirection, _hitForce);
                StartCoroutine(Parry());
            }
            else
            {
                StopCoroutine(Parry());
                parrying=false;
                ResetAllAttack();
                StartCoroutine(payback());
            }
        }
        else
        {
            StopCoroutine(Stunned());
            anim.SetBool("Stun", false );
            stunned = false;
        }
        if(health > 30)
        {
            ChangeState(EnemyStates.Monk_State1);
        }
        if(health <= 15)
        {
            anim.SetBool("Changestate", true);
            ChangeState(EnemyStates.Monk_State2);
        }

        if (health <= 0 && alive)
        {
            Debug.Log("Health reached zero, calling Death()");
            Death(0); // Trigger death
        }
    }

    IEnumerator payback()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Punch");
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("Punch");

        ResetAllAttack();
    }

    #region Stage2

    void FlyingKickAttackJump()
    {
        attacking = true ;
        moveToPosition = new Vector2(Move.Instance.transform.position.x, rb.velocity.y +2);
        flyingKichAttack = true ;
        anim.SetBool("Jump", true);
    }

    public void Dive()
    {
        anim.SetBool("FlyingKick",true);
        anim.SetBool("Jump", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Move>() != null && flyingKichAttack)
        {
            collision.GetComponent<Move>().takeDmg(dmgMake * 2);
            Move.Instance.playerStateList.recoilingX = true;

            // Disable the attack to prevent further damage
            flyingKichAttack = false;
        }
    }


    public void DivingPillar()
    {
        Vector2 _impactPoint = groundcheck.position;
        float _spawnDistance = 5f;

        for (int i = 0; i < 5; i++)
        {
            Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
            Debug.Log($"Diving Pillar {i} has a spawn distance of {_spawnDistance}");
            Debug.Log($"Spawning right pillar at: {_pillarSpawnPointRight}");
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, 0));
            Debug.Log($"Spawning left pillar at: {_pillarSpawnPointLeft}");
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, 0));
            Debug.DrawLine(_impactPoint, _pillarSpawnPointRight, Color.red, 5f);
            Debug.DrawLine(_impactPoint, _pillarSpawnPointLeft, Color.blue, 5f);

            _spawnDistance += 2;
        }

        ResetAllAttack();
    }

    #endregion

    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        anim.SetBool("Stun", true);

        yield return new WaitForSeconds(4f);
        anim.SetBool("Stun", false);
        stunned = false;
    }

    protected override void Death(float _destroyTime)
    {
        if (!alive) return; // Prevent multiple death calls

        Debug.Log("Boss is dead!");
        alive = false;
        ResetAllAttack(); // Ensure attacks are stopped

        rb.velocity = Vector2.zero; // Stop movement
        anim.SetTrigger("Die"); // Play death animation

        // Optionally disable collider or make the boss "inactive"
        GetComponent<Collider2D>().enabled = false;
        GameManager.Instance.bossDeafeated = true;
        SaveData.Instance.saveBoss();

        // Destroy object after delay
        Destroy(gameObject, _destroyTime);
    }


    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}
