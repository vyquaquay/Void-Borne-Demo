using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SaveData
{
    //player
    public static SaveData Instance;
   public HashSet<string> sceneNames;
    public string altersceneName;
    public Vector2 alterPos;
    public int playerHealth;
    public int playerHeartShards;
    public float playerMana;
    public Vector2 playerPosition;
    public string lastScene;
    public int playerMaxHealth;


    public bool playerUnlockWallJump;
    public bool playerUnlockDash;
    public bool playerUnlockDbJump;

    //boss
    public bool BossDeafeated;
    public void Initialize()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.alter.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.alter.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }
        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void saveAlter()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.alter.data")))
        {
            writer.Write(altersceneName);
            writer.Write(alterPos.x);
            writer.Write(alterPos.y);
            Debug.Log("saved Max Health: " + playerMaxHealth);
            Debug.Log("saved heart shards: " + playerHeartShards);
        }
    }
    public void loadAlter()
    {
        string savePath = Application.persistentDataPath + "/save.alter.data";
        if (File.Exists(savePath) && new FileInfo(savePath).Length > 0)
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.alter.data")))
            {
                altersceneName = reader.ReadString();
                alterPos.x = reader.ReadSingle();
                alterPos.y = reader.ReadSingle();
                Debug.Log("saved Max Health: " + playerMaxHealth);
                Debug.Log("saved heart shards: " + playerHeartShards);
            }
        }
    }
    public void savePlayerData()
    {
        string filePath = Application.persistentDataPath + "/save.player.data";
        using (BinaryWriter writer = new BinaryWriter(File.Create(filePath)))
        {
            // Save player health and max health
            playerHealth = Move.Instance.Health;
            writer.Write(playerHealth);

            playerMaxHealth = Move.Instance.MaxHealth;
            writer.Write(playerMaxHealth);

            // Save player mana
            playerMana = Move.Instance.Mana;
            writer.Write(playerMana);

            // Save player unlock abilities
            playerUnlockWallJump = Move.Instance.unlockWalljump;
            writer.Write(playerUnlockWallJump);

            playerUnlockDash = Move.Instance.unlockDash;
            writer.Write(playerUnlockDash);

            playerUnlockDbJump = Move.Instance.unlockDbJump;
            writer.Write(playerUnlockDbJump);

            // Save player position
            playerPosition = Move.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            // Save player heart shards
            playerHeartShards = Move.Instance.heartShards;
            writer.Write(playerHeartShards);

            // Save the last scene name
            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);

            Debug.Log("Player data saved successfully.");
            Debug.Log("Before saving - Health: " + playerHealth + ", Max Health: " + playerMaxHealth + ", Heart Shards: " + playerHeartShards);
        }
    }

    public void loadPlayerData()
    {
        string filePath = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(filePath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                // Load all necessary data
                playerHealth = reader.ReadInt32();
                playerMaxHealth = reader.ReadInt32();
                playerMana = reader.ReadSingle();
                playerUnlockWallJump = reader.ReadBoolean();
                playerUnlockDash = reader.ReadBoolean();
                playerUnlockDbJump = reader.ReadBoolean();
                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                playerHeartShards = reader.ReadInt32();
                lastScene = reader.ReadString();

                Debug.Log($"Loaded Health: {playerHealth}, Max Health: {playerMaxHealth}");

                // Load the scene and register a callback to set player data after the scene loads
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(lastScene);
            }
        }
        else
        {
            Debug.Log("Save file not found. Initializing default values.");
            Move.Instance.Health = Move.Instance.MaxHealth;
            Move.Instance.heartShards = 0;
            Move.Instance.Mana = 0.5f;
            Move.Instance.unlockWalljump = false;
            Move.Instance.unlockDash = false;
            Move.Instance.unlockDbJump = false;
        }
    }

    // Callback function to set player data after the scene has loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unregister the event to avoid it being called multiple times
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Set the player's data once the scene has fully loaded
        Move.Instance.transform.position = playerPosition;
        Move.Instance.MaxHealth = playerMaxHealth;
        Move.Instance.Health = playerHealth;
        Move.Instance.Mana = playerMana;
        Move.Instance.unlockWalljump = playerUnlockWallJump;
        Move.Instance.unlockDash = playerUnlockDash;
        Move.Instance.unlockDbJump = playerUnlockDbJump;
        Move.Instance.heartShards = playerHeartShards;

        Debug.Log($"After loading - Health: {Move.Instance.Health}, Max Health: {Move.Instance.MaxHealth}, Position: {playerPosition}");
    }

    public void saveBoss()
    {
        string savePath = Application.persistentDataPath + "/save.boss.data";

        // Ensure the save file is created and written properly
        using (BinaryWriter writer = new BinaryWriter(File.Create(savePath)))
        {
            BossDeafeated = GameManager.Instance.bossDeafeated; // Get the defeat state
            writer.Write(BossDeafeated); // Write the state to the file
        }

        Debug.Log("Boss save file created and state saved: " + BossDeafeated);
    }

    public void loadBoss()
    {
        string savePath = Application.persistentDataPath + "/save.boss.data";
        if (File.Exists(savePath) && new FileInfo(savePath).Length > 0)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.boss.data")))
            {
                BossDeafeated = reader.ReadBoolean();
                GameManager.Instance.bossDeafeated = BossDeafeated;
            }
        }
    }
}
