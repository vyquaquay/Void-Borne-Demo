using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corrupted_Monk_Event : MonoBehaviour
{
    void PunchDamagePlayer()
    {
        if (Move.Instance.transform.position.x > transform.position.x || Move.Instance.transform.position.x < transform.position.x)
        {
            Hit(CorruptedMonk.Instance.sideAttackTransform, CorruptedMonk.Instance.sideAttackArea);
        }
        else if (Move.Instance.transform.position.y > transform.position.y)
        {
            Hit(CorruptedMonk.Instance.upAttackTransform, CorruptedMonk.Instance.upAttackArea);
        }
        else if (Move.Instance.transform.position.y < transform.position.y)
        {
            Hit(CorruptedMonk.Instance.downAttackTransform, CorruptedMonk.Instance.downAttackArea);
        }
    }
    void Hit(Transform _attackTrasnform, Vector2 _attackArea)
    {
        Collider2D _objectToHit = Physics2D.OverlapBox(_attackTrasnform.position, _attackArea, 0);
        if( _objectToHit.GetComponent<Move>() != null && !Move.Instance.playerStateList.Invi )
        {
            _objectToHit.GetComponent<Move>().takeDmg(CorruptedMonk.Instance.dmgMake);
            if (Move.Instance.playerStateList.alive)
            {
                Move.Instance.HitStopTime(0, 5, 0.5f);
            }
            CorruptedMonk.Instance.damagedPlayer = true;
        }
    }

    void Parrying()
    {
        CorruptedMonk.Instance.parrying = true;
    } 

    void DestroyAfterDeath()
    {
        SpawnBoss.Instance.isnotTrigger();
        CorruptedMonk.Instance.DestroyAfterDeath();
        GameManager.Instance.bossDeafeated = true;
        SaveData.Instance.saveBoss();
        Debug.Log("Save boss data");
        SaveData.Instance.savePlayerData();
    }
}
