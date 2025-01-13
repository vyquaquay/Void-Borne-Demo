using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Kick : StateMachineBehaviour
{
    Rigidbody2D rb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.gravityScale = 0;
        int _dir = CorruptedMonk.Instance.facingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * (CorruptedMonk.Instance.speed * 1.5f), 0f);

        if (Vector2.Distance(Move.Instance.transform.position, rb.position) <= CorruptedMonk.Instance.attackRange && !CorruptedMonk.Instance.damagedPlayer && !Move.Instance.playerStateList.Invi)
        {
            Move.Instance.takeDmg(CorruptedMonk.Instance.dmgMake);
            if(Move.Instance.playerStateList.alive)
            {
                Move.Instance.HitStopTime(0, 5, 0.5f);
            }
            CorruptedMonk.Instance.damagedPlayer = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
