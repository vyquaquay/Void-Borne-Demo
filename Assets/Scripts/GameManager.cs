using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string transitionFromScene;
    public static GameManager Instance {  get; private set; }

    public Vector2 respawnpoint;
    [SerializeField] Alter alter;
    [SerializeField] private fadedUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPause;
    public Vector2 platformRespawnpoint;
    public bool bossDeafeated = false;
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
        alter = FindObjectOfType<Alter>();
        SaveData.Instance.loadBoss();

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.savePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPause)
        {
            pauseMenu.fadeUIIn(fadeTime);
            Debug.Log("Pausing game. GameManager active: " + this.gameObject.activeSelf);
            Time.timeScale = 0;
            gameIsPause = true;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1; 
        pauseMenu.fadeUIOut(fadeTime); 
        gameIsPause = false;
        Rigidbody2D rbd2 = Move.Instance.GetComponent<Rigidbody2D>();
        rbd2.constraints = RigidbodyConstraints2D.FreezeRotation;
        Debug.Log("Game unpaused, Time.timeScale reset to 1");
    }

        public void SaveGame()
    {
        SaveData.Instance.savePlayerData();
    }

    public void respawnPlayer()
    {
        SaveData.Instance.loadAlter();
        if(SaveData.Instance.altersceneName != null)
        {
            SceneManager.LoadScene(SaveData.Instance.altersceneName);
        }
        if(SaveData.Instance.alterPos != null)
        {

            respawnpoint = SaveData.Instance.alterPos;
            
        }
        else
        {
            respawnpoint = platformRespawnpoint;
        }
        Move.Instance.transform.position = respawnpoint;
        StartCoroutine(UIManager.Instance.DeactiveDeathScreen());
        Move.Instance.Respawn();
    }

    
}
