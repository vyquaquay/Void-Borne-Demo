using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
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
        TargetPlayerPosition(animator);
        if (CorruptedMonk.Instance.attackCountdown <= 0)
        {
            CorruptedMonk.Instance.AttackHandler();
            CorruptedMonk.Instance.attackCountdown = Random.Range(CorruptedMonk.Instance.attackTimer - 1, CorruptedMonk.Instance.attackTimer + 1);
        }
    }

    void TargetPlayerPosition(Animator animator)
    {
        if (CorruptedMonk.Instance.isonGround())
        {
            CorruptedMonk.Instance.Flip();
            Vector2 _target = new Vector2(Move.Instance.transform.position.x, rb.position.y);
            Vector2 _newPos = Vector2.MoveTowards(rb.position, _target, CorruptedMonk.Instance.runSpeed * Time.fixedDeltaTime);

            rb.MovePosition(_newPos);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }
        if(Vector2.Distance(Move.Instance.transform.position, rb.position) <= CorruptedMonk.Instance.attackRange)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            return;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Run", false);
    }

}
