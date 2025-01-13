using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingPillar : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !Move.Instance.playerStateList.Invi)
        {
            Debug.Log("Player hit by pillar!");
            collision.GetComponent<Move>().takeDmg(CorruptedMonk.Instance.dmgMake * 2);
            Debug.Log($"Player health after hit: {collision.GetComponent<Move>().health}");
            if (Move.Instance.playerStateList.alive)
            {
                Move.Instance.HitStopTime(0, 5, 0.5f);
            }

        }
    }

}
