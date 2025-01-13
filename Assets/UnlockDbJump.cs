using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDbJump : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject canvasUI;
    bool used;
    void Start()
    {
        if (Move.Instance.unlockDbJump)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());
        }
    }

    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(0.5f);
        canvasUI.SetActive(true);
        Move.Instance.unlockDbJump = true;
        yield return new WaitForSeconds(3.5f);
        SaveData.Instance.savePlayerData();
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
