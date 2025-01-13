using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
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
        rb.velocity = Vector2.zero;
        RunToPlayer(animator);

        if(CorruptedMonk.Instance.attackCountdown <= 0)
        {
            CorruptedMonk.Instance.AttackHandler();
            CorruptedMonk.Instance.attackCountdown = Random.Range(CorruptedMonk.Instance.attackTimer - 1, CorruptedMonk.Instance.attackTimer + 1);
        }

        if (!CorruptedMonk.Instance.isonGround())
        {
            rb.velocity = new  Vector2(rb.velocity.x,-25);
        }
    }


    void RunToPlayer(Animator animator)
    {
        if(Vector2.Distance(Move.Instance.transform.position, rb.transform.position) >= CorruptedMonk.Instance.attackRange)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
