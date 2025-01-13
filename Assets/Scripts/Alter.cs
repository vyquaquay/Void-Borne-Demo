using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Alter : MonoBehaviour
{

    public bool Interacted;
    public bool inRange;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetButtonDown("Interact"))
        {

            Interacted = true;
            SaveData.Instance.altersceneName = SceneManager.GetActiveScene().name;
            SaveData.Instance.alterPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            SaveData.Instance.saveAlter();
            SaveData.Instance.savePlayerData();
            Debug.Log("Saved");
        }
    }
    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player") )
        {
            inRange = false;
            Interacted = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }


}

