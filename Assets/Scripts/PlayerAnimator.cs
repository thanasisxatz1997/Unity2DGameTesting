using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;
    private Animator animator;
    private const string IS_MOVING = "isMoving";
    private const string IS_ATTACKING = "isAttacking";
    private const string ATTACK_TRIGGER = "attackTrigger";
    private const string RUNNINGATTACK_TRIGGER = "runningAttackTrigger";
    private const string JUMP_TRIGGER = "jumpTrigger";
    private const string RUNNING_ATTACK_CD = "runningAttackCD";
    private const string IS_FALLING = "isFalling";
    private const string IS_RUNNING = "isRunning";
    private const string THROW_ATTACK_TRIGGER = "throwAttackTrigger";
    private const string DASH_TRIGGER = "dashTrigger";
    private const string IS_GROUNDED = "isGrounded";

    

    private void Start()
    {
        animator = GetComponent<Animator>();
        player.OnPlayerJump += Player_OnPlayerJump;
        player.OnPlayerAttack += Player_OnPlayerAttack;
        player.OnPlayerThrowAttack += Player_OnPlayerThrowAttack;
        player.OnPlayerDash += Player_OnPlayerDash;
    }

    private void Player_OnPlayerDash(object sender, System.EventArgs e)
    {
        animator.SetTrigger(DASH_TRIGGER);
    }

    private void Player_OnPlayerThrowAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger(THROW_ATTACK_TRIGGER);
    }

    private void Player_OnPlayerAttack(object sender, System.EventArgs e)
    {
        if (player.IsMoving())
        {
            if (player.IsRunning() && player.GetRunningAttackCD() <= 0)
            {
                animator.SetTrigger(RUNNINGATTACK_TRIGGER);
                player.SetRunningAttackCD(1.5f);
            }
            else
            {
                animator.SetTrigger(ATTACK_TRIGGER);
            }
        }
        else
        {
            animator.SetTrigger(ATTACK_TRIGGER);
        }
        
    }

    private void Player_OnPlayerJump(object sender, System.EventArgs e)
    {
        animator.SetTrigger(JUMP_TRIGGER);
    }

    private void Update()
    {
        animator.SetBool(IS_MOVING, player.IsMoving());
        animator.SetBool(IS_RUNNING, player.IsRunning());
        if (player.IsTouchingGround())
        {
            animator.SetBool(IS_GROUNDED, true);
        }
        else
        {
            animator.SetBool(IS_GROUNDED, false);
        }
        animator.SetBool(IS_FALLING, player.IsFalling());
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("RunningAttack2H") || animator.GetCurrentAnimatorStateInfo(0).IsName("2HAttack"))
        {
            player.EnableSlashTrailRenderer();
            player.SetAttacking(true);
        }
        else
        {
            player.DisableSlashTrailRenderer();
            player.SetAttacking(false);
        }
    }
}
