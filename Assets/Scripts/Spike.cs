using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if(_collision.CompareTag("Player"))
        {
            StartCoroutine(Respawnpoint());
        }
    }

    IEnumerator Respawnpoint()
    {
        Move.Instance.playerStateList.incutscene = true;
        Move.Instance.playerStateList.Invi = true;
        Move.Instance.rbd2.velocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneFading.Fade(SceneFading.fadeDirection.In));
        Move.Instance.takeDmg(1);
        yield return new WaitForSecondsRealtime(1);
        Move.Instance.transform.position = GameManager.Instance.platformRespawnpoint;
        StartCoroutine(UIManager.Instance.sceneFading.Fade(SceneFading.fadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFading.fadeTime);
        Move.Instance.playerStateList.incutscene = false;
        Move.Instance.playerStateList.Invi = false;
        Time.timeScale = 1;
    }
}
