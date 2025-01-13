using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    // Start is called before the first frame update

    public static SpawnBoss Instance;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject boss;
    [SerializeField] Vector2 exitDir;
    bool callOnce;
    BoxCollider2D col;

    private void Awake()
    {
        if(CorruptedMonk.Instance != null)
        {
            Destroy(CorruptedMonk.Instance);
            callOnce = false;
            col.isTrigger = true;
        }

        if(GameManager.Instance.bossDeafeated)
        {
            callOnce = true;
        }

        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple SpawnBoss scripts found!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !callOnce && !GameManager.Instance.bossDeafeated)
        {
            if (!callOnce)
            {
                StartCoroutine(WalktoRoom());
                callOnce = true;
            }
        }
    }

    IEnumerator WalktoRoom()
    {
        StartCoroutine(Move.Instance.WalktonewScene(exitDir, -1));
        Move.Instance.playerStateList.incutscene = true;
        yield return new WaitForSeconds(1f);
        col.isTrigger = false;
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
    }

    public void isnotTrigger()
    {
        col.isTrigger = true;
    }
}
