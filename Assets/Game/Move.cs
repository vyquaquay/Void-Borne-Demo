using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    //animation and physic
    [Header("physic of player")]
    public Rigidbody2D rbd2;
    [SerializeField] public float speed = 2.0f;
    private float xAxis, yAxis;
    [SerializeField] private float jumpForce = 10;
    private int dbJumpCounter;
    [SerializeField] private int maxdbJump = 1;
    [Space(5)]

    [Header("Groundcheck setting")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float groundchecky = 0.2f;
    [SerializeField] private float groundcheckx = 0.5f;
    [SerializeField] private LayerMask isground;
    Animator animator;
    [HideInInspector] public PlayerStateList playerStateList;
    [Space(5)]


    [Header("Coyotetime setting")]
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimeCounter = 0;
    [Space(5)]

    [Header("Attacking setting")]
    [SerializeField] private float timeBetweenattck;
    [SerializeField] Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float playerDmg;
    private float timeSinceattk;
    bool attk = false;
    [Space(5)]

    [Header("Health Setting")]
    [SerializeField] public int health;
    [SerializeField] public int MaxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashspeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    [SerializeField] float timetoHeal;
    float healTimer;
    public int maxTotalHealth = 10;
    public int heartShards;
    [Space(5)]


    [Header("Dash setting")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCD;
    private bool canDash = true;
    private bool dashed;
    [Space(5)]

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepXrecoiled, stepYrecoiled;
    [Space(5)]

    [Header("Mana Setting")]
    [SerializeField] float mana;
    [SerializeField] float manaDrain;
    [SerializeField] float manaGain;
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [Space(5)]

    [Header("Spellcasting")]
    [SerializeField] float manaSpellcost = 0.3f;
    [SerializeField] float timeBetweencast = 0.5f;
     float timeSincecast;
    float castOrhealTimer;
    [SerializeField] float spellDmg; //dark rising and skyfall only
    [SerializeField] float skyfallForce; // force for skyfall
    [SerializeField] GameObject sideSpellDarkBall;
    [SerializeField] GameObject upSpellDarkRising;
    [SerializeField] GameObject downSpellSkyFall;
    [Space(5)]

    [Header("Walljump")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpPow;
    float wallJumpDirection;
    bool iswallSliding;
    bool iswallJumping;
    public bool unlockWalljump;
    public bool unlockDash;
    public bool unlockDbJump;
    [Space(5)]

    [Header("Audio")]
    [SerializeField] AudioClip landingSOund;
    [SerializeField] AudioClip jumpSOund;
    [SerializeField] AudioClip dashAndAttackSound;
    [SerializeField] AudioClip spellCastSound;
    [SerializeField] AudioClip hurtSound;
    private AudioSource audioSource;
    [Space(5)]
    bool restoreTime;
    float restoreTimeSpeed;
    private SpriteRenderer spriteRenderer;
    public static Move Instance;
    private float gravity;
    bool openMap;
    bool openInven;
    private bool landingSoundPlay;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    private void Start()
    {
        //player game object
        playerStateList = GetComponent<PlayerStateList>();
        rbd2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        gravity = rbd2.gravityScale;
        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = MaxHealth;
        SaveData.Instance.loadPlayerData();
        if(Health == 0)
        {
            playerStateList.alive = false;
            GameManager.Instance.respawnPlayer();
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

    void Update()
    {
        if (GameManager.Instance.gameIsPause) return;
        if (playerStateList.incutscene) return;

        if (playerStateList.alive)
        {
            getInput();
            OpenMap();
            OpenInventory();
        }

        UpdateJump();
        RestoreTimeScale();
        if (playerStateList.dashing) return;
        FlashWhenInvi();

        if (playerStateList.alive)
        {
            if (!iswallJumping && !playerStateList.healing)
            {
                Moving();
                Flip();
                Jump();
            }

            CastSpell(); // Always check CastSpell first
            Heal(); // Check healing second

        }
        if (unlockWalljump)
        {
            WallSlide();
            WallJump();
        }

        if (unlockDash)
        {
            StartDash();
        }

        Attack();
    }


    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<Enemy>() != null && playerStateList.castspell)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDmg, (_other.transform.position - _other.transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (playerStateList.incutscene) return;
        if (playerStateList.dashing) return;
        Recoil();
    }

    void getInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attk = Input.GetButtonDown("Attack");
        openMap = Input.GetButton("Map");
        openInven = Input.GetButton("Inventory");
        if (Input.GetButton("Cast/Heal"))
        {
            castOrhealTimer += Time.deltaTime;
        }
        else
        {
            castOrhealTimer = 0;
        }
    
    }
    //player Attack
    void Attack()
    {
        timeSinceattk += Time.deltaTime;
        if (attk && timeSinceattk >= timeBetweenattck)
        {
            timeSinceattk = 0;
            animator.SetTrigger("Attacking");
            audioSource.PlayOneShot(dashAndAttackSound);

            if (yAxis == 0 || yAxis < 0 && isonGround())
            {
                int _recoilLeftorRight = playerStateList.lookingRight ? 1 : -1;
                Hit(sideAttackTransform, sideAttackArea, ref playerStateList.recoilingX, recoilXSpeed, Vector2.right * _recoilLeftorRight);
            }
            else if (yAxis > 0)
            {
                Hit(upAttackTransform, upAttackArea, ref playerStateList.recoilingY, recoilYSpeed, Vector2.up);
            }
            else if (yAxis < 0 && !isonGround())
            {
                Hit(downAttackTransform, downAttackArea, ref playerStateList.recoilingY, recoilYSpeed, Vector2.down);
            }
        }
    }

    //Using for the character recoil
    void Recoil()
    {
        if (playerStateList.recoilingX)
        {
            if (playerStateList.lookingRight)
            {
                rbd2.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rbd2.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (playerStateList.recoilingY)
        {
            rbd2.gravityScale = 0;
            if (yAxis < 0)
            {

                rbd2.velocity = new Vector2(rbd2.velocity.x, recoilYSpeed);
            }
            else
            {
                rbd2.velocity = new Vector2(rbd2.velocity.x, -recoilYSpeed);
            }
            dbJumpCounter = 0;
        }
        else
        {
            rbd2.gravityScale = gravity;
        }
        //Stop recoil X
        if (playerStateList.recoilingX && stepXrecoiled < recoilXSteps)
        {
            stepXrecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        //Stop recoil Y
        if (playerStateList.recoilingY && stepYrecoiled < recoilYSteps)
        {
            stepYrecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        // if hit ground stop recoil Y
        if (isonGround())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepXrecoiled = 0;
        playerStateList.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepYrecoiled = 0;
        playerStateList.recoilingY = false;
    }
    //player hit check
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, float _recoilStrenght, Vector2 _recoilDir)
    {
        Collider2D[] objecttoHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        if (objecttoHit.Length > 0)
        {
            _recoilBool = true;
        }

        for (int i = 0; i < objecttoHit.Length; i++)
        {
            if (objecttoHit[i].GetComponent<Enemy>() != null)
            {
                objecttoHit[i].GetComponent<Enemy>().EnemyHit(playerDmg, _recoilDir, _recoilStrenght);
                if (objecttoHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }
    //change dir of the player
    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            playerStateList.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            playerStateList.lookingRight = true;
        }
    }
    // start the dash
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (isonGround())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {

        canDash = false;
        playerStateList.dashing = true;
        animator.SetTrigger("Dashing");
        audioSource.PlayOneShot(dashAndAttackSound);
        rbd2.gravityScale = 0;
        int _dir = playerStateList.lookingRight ? 1 : -1;
        rbd2.velocity = new Vector2(_dir * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rbd2.gravityScale = gravity;
        playerStateList.dashing = false;
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }
    //moving of player
    private void Moving()
    {
        //if(playerStateList.healing) rbd2.velocity = new Vector2(0,0);
        rbd2.velocity = new Vector2(speed * xAxis, rbd2.velocity.y);
        animator.SetBool("Walking", rbd2.velocity.x != 0 && isonGround());
    }
    //check if player is on the ground or not
    public bool isonGround()
    {
        if (Physics2D.Raycast(groundcheck.position, Vector2.down, groundchecky, isground)
            || Physics2D.Raycast(groundcheck.position + new Vector3(groundcheckx, 0, 0), Vector2.down, groundchecky, isground)
            || Physics2D.Raycast(groundcheck.position + new Vector3(-groundcheckx, 0, 0), Vector2.down, groundchecky, isground)
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //player jump
    void Jump()
    {

        if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0)
        {
            audioSource.PlayOneShot(jumpSOund);
            playerStateList.jumping = false;
            rbd2.velocity = new Vector3(rbd2.velocity.x, jumpForce);
            
        }
        else if (!isonGround() && dbJumpCounter < maxdbJump && Input.GetButtonDown("Jump") && unlockDbJump)
        {
            audioSource.PlayOneShot(jumpSOund);
            playerStateList.jumping = true ;
            dbJumpCounter++;
            rbd2.velocity = new Vector3(rbd2.velocity.x, jumpForce);
        }
        animator.SetBool("Jumping", !isonGround());
    }
    void UpdateJump()
    {
        if (isonGround())
        {
            if (!landingSoundPlay)
            {
                audioSource.PlayOneShot(landingSOund);
                landingSoundPlay = true;
            }
            playerStateList.jumping = false;
            coyoteTimeCounter = coyoteTime;
            dbJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlay = false;
        }
    }
    //stop taking dmg when player get hit
    IEnumerator StopTakingDmg()
    {
        playerStateList.Invi = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1f);
        animator.SetTrigger("TakeDmg");
        yield return new WaitForSeconds(0.8f);
        playerStateList.Invi = false;
    }
    //player health calculated
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, MaxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
                manaStorage.fillAmount = Mana;
            }
        }

    }
    //take dmg function
    public void takeDmg(float _Dmg)
    {
        if (playerStateList.alive)
        {
            audioSource.PlayOneShot(hurtSound);
            Health -= Mathf.RoundToInt(_Dmg);
            if(Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDmg());
            }
      
        } 
    }
    //time delay for player when get hit
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if (_delay > 0)
        {
            StopCoroutine(TimeStartAgain(_delay));
            StartCoroutine(TimeStartAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    IEnumerator TimeStartAgain(float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        restoreTime = true;

    }

    void FlashWhenInvi()
    {
        spriteRenderer.material.color = playerStateList.Invi ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashspeed, 0.5f)) : Color.white;
    }
    void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrhealTimer > 0.05f && Health < MaxHealth && Mana > 0 && !playerStateList.jumping && !playerStateList.dashing)
        {
            print("Healing: " + playerStateList.healing + ", Timer:" + healTimer + ", Mana:" + Mana);
            playerStateList.healing = true;
            animator.SetBool("Healing", true);
            healTimer += Time.deltaTime;
            if (healTimer >= timetoHeal)
            {
                Health++;
                healTimer = 0;
            }

            // Using mana to heal
            Mana -= Time.deltaTime * manaDrain;
        }
        else
        {
            playerStateList.healing = false;
            animator.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrhealTimer <= 0.05f && timeSincecast >= timeBetweencast && Mana >= manaSpellcost)
        {
            Debug.Log("Side Spell cast");
            playerStateList.castspell = true;
            timeSincecast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSincecast += Time.deltaTime;
        }

        if (!Input.GetButton("Cast/Heal"))
        {
            castOrhealTimer = 0; // Reset the timer when the button is released
        }

        if (isonGround())
        {
            // Cannot use Skyfall if on ground
            downSpellSkyFall.SetActive(false);
        }

        // Force the player to fall down if casting Skyfall
        if (downSpellSkyFall.activeInHierarchy)
        {
            rbd2.velocity += skyfallForce * Vector2.down;
        }
    }


    IEnumerator CastCoroutine()
    {

        //sidecast/darkball
        if(yAxis == 0 || (yAxis < 0 && isonGround()))
        {
            animator.SetBool("Casting", true);
            audioSource.PlayOneShot(spellCastSound);
            yield return new WaitForSeconds(0.15f);
            GameObject _darkball = Instantiate(sideSpellDarkBall, sideAttackTransform.position, Quaternion.identity);
            Debug.Log("Spell cast");
            //flip skill
            if (playerStateList.lookingRight)
            {
                _darkball.transform.eulerAngles = Vector3.zero; // ban spell nhu bth
            }
            else 
            {
                _darkball.transform.eulerAngles = new Vector2(_darkball.transform.eulerAngles.x, 100);
            }

            playerStateList.recoilingX = true;
        }

        //upcast/Dark Rising
        else if(yAxis > 0)
        {
            animator.SetBool("Casting", true);
            audioSource.PlayOneShot(spellCastSound);
            Debug.Log("Side Spell cast");
            yield return new WaitForSeconds(0.15f);
            Instantiate(upSpellDarkRising, transform);
            rbd2.velocity = Vector2.zero;
        }

        //downcasr/ Sky Fall
        else if (yAxis < 0 && !isonGround())
        {
            animator.SetBool("Casting", true);
            Debug.Log("Side Spell cast");
            audioSource.PlayOneShot(spellCastSound);
            yield return new WaitForSeconds(0.15f);
            downSpellSkyFall.SetActive(true);
        }

        Mana -= manaSpellcost;
        yield return new WaitForSeconds(0.35f);
        animator.SetBool("Casting", false);
        playerStateList.castspell = false;
    }

    public IEnumerator WalktonewScene(Vector2 _exitDir, float _delay)
    {
        playerStateList.Invi = true;
        if (_exitDir.y > 0)
        {
            rbd2.velocity = jumpForce * _exitDir;
        }
        if (_exitDir.x != 0) 
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Moving();
        }
        Flip();
        yield return new WaitForSeconds(_delay);
        playerStateList.Invi = false;
        playerStateList.incutscene = false;
    }

    IEnumerator Death()
    {
        playerStateList.alive = false;
        Time.timeScale = 1f;
        animator.SetTrigger("Death");
        rbd2.constraints = RigidbodyConstraints2D.FreezePosition;
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActiveDeathScreen());
    }

    public void Respawn()
    {
        if (!playerStateList.alive)
        {
            rbd2.constraints = RigidbodyConstraints2D.None;
            rbd2.constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<BoxCollider2D>().enabled = true;
            playerStateList.alive = true;
            Health = MaxHealth;
            Mana = 0;
            animator.Play("PlayerIdle");

        }
    }

    void OpenMap()
    {
        if (openMap)
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }
    void OpenInventory()
    {
        if (openInven)
        {
            UIManager.Instance.inventory.SetActive(true);
        }
        else
        {
            UIManager.Instance.inventory.SetActive(false);
        }
    }

    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);

    }

    void WallSlide()
    {
        if (Walled() && !isonGround() && xAxis != 0)
        {
            iswallSliding = true;
            rbd2.velocity = new Vector2(rbd2.velocity.x, Mathf.Clamp(rbd2.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            iswallSliding = false;
        }
    }
    void WallJump()
    {
        if (iswallSliding)
        {
            iswallJumping = false;
            wallJumpDirection = !playerStateList.lookingRight ? 1:-1;
            CancelInvoke(nameof(StopWallJumping));
        }

        if (Input.GetButtonDown("Jump") && iswallSliding)
        {
            iswallJumping = true;
            rbd2.velocity = new Vector2(wallJumpDirection * wallJumpPow.x, wallJumpPow.y);
            dashed = false;
            dbJumpCounter = 0;

            playerStateList.lookingRight =  !playerStateList.lookingRight;
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    void StopWallJumping()
    {
        iswallJumping = false;
        transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
    }
}
