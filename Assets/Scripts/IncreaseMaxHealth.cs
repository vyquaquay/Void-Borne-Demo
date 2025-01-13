using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxHealth : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject canvasUI;
    bool used;
    [SerializeField] HeartShards heartShards;
    void Start()
    {
        if (Move.Instance.MaxHealth >= Move.Instance.maxTotalHealth)
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
        heartShards.initalFillAmount = Move.Instance.heartShards * 0.25f;
        Move.Instance.heartShards++;
        heartShards.targetFillAmount = Move.Instance.heartShards * 0.25f;
        Move.Instance.unlockWalljump = true;
        StartCoroutine(heartShards.LerpFill());

        yield return new WaitForSeconds(2.5f);
        SaveData.Instance.savePlayerData();
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
