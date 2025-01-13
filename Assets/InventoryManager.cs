using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] Image heartShard;
    [SerializeField] GameObject dash, dbJump, walljump;


    private void OnEnable()
    {
        heartShard.fillAmount = Move.Instance.heartShards * 0.25f;

        if (Move.Instance.unlockDash)
        {
            dash.SetActive(true);
        }
        else
        {
            dash.SetActive(false);
        }
        if (Move.Instance.unlockDbJump)
        {
            dbJump.SetActive(true);
        }
        else
        {
            dbJump.SetActive(false);
        }
        if (Move.Instance.unlockWalljump)
        {
            walljump.SetActive(true);
        }
        else
        {
            walljump.SetActive(false);
        }
    }
}
