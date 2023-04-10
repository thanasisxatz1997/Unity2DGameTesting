using HeroEditor.Common.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform weapon;
    [SerializeField] private GameObject damageNumberPrefab;

    private Collider2D enemyCollider;
    private CapsuleCollider2D weaponCollider;
    private List<Collider2D> objectsToDamageList;
    private Animator enemyAnimator;
    private Rigidbody2D enemyRigidbody;
    private List<RaycastHit2D> raycastHitList;
    private Transform playerTransform;

    private bool isWalking=false;
    private bool seesPlayer = false;
    private bool isAttacking = false;
    private bool isDashing = false;

    private float health=500f;
    private float enemyDamageValue = 30f;
    private float moveSpeed=2f;
    private float attackCd = 0f;
    private float maxAttackCd = 1.3f;
    private float attackTime = 0f;
    private float maxAttackTime = 1.3f;
    private float damageCd=0f;
    private float maxDamageCd=1.3f;
    private float dashAttackCd = 0f;
    private float maxDashAttackCd = 6f;
    private float dashTimer = 0f;
    private float maxDashTimer = 0.5f;


    private void Awake()
    {
        enemyCollider = GetComponent<Collider2D>();
        objectsToDamageList =new List<Collider2D>();
        weaponCollider =weapon.GetComponent<CapsuleCollider2D>();
        enemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        raycastHitList= new List<RaycastHit2D>();
    }
    private void Update()
    {
        LookForPlayer();
        if (seesPlayer)
        {
            FollowPlayer();
            HandleAttacks();
        }
        if (isAttacking || isDashing)
        {
            CheckForWeaponCollision();
        }
        Animate();
    }
    public void LookForPlayer()
    {
        Physics2D.CircleCast(transform.position, 10f, transform.position,new ContactFilter2D(),raycastHitList);
        seesPlayer = false;
        foreach (RaycastHit2D raycastHit in raycastHitList)
        {
            if (raycastHit.transform.GetComponent<Player>())
            {
                playerTransform= raycastHit.transform;
                seesPlayer = true;
                Debug.Log("Player Found!");
            }
        }
    }

    public void FollowPlayer()
    {
        if (!isAttacking)
        {
            if (!isDashing)
            {
                Move();
            }
            else
            {
                Dash();
            }
        }
        else
        {
            isWalking = false;
        }
    }
    private void HandleAttacks()
    {
        float distanceFromPlayer = Mathf.Sqrt(Mathf.Pow(transform.position.x - playerTransform.position.x, 2f)+ Mathf.Pow(transform.position.y - playerTransform.position.y, 2f));

        if (distanceFromPlayer <= 1f)
        {
            if (attackCd <= 0f)
            {
                Debug.Log("Attacking!");
                isAttacking = true;
                enemyAnimator.SetTrigger("attack");
                attackCd = maxAttackCd;
                attackTime = maxAttackTime;
            }
        }
        else if(distanceFromPlayer<=10f && distanceFromPlayer>3f)
        {
            if (dashAttackCd <= 0f)
            {
                Vector3 playerPosition = playerTransform.position;
                isDashing = true;
                isWalking = false;
                enemyAnimator.SetTrigger("dashAttack");
                dashTimer = maxDashTimer;
                dashAttackCd = maxDashAttackCd;
                attackTime = maxAttackTime;
            }
        }
        if (attackCd > 0)
        {
            attackCd = attackCd - 1 * Time.deltaTime;
        }
        if (dashAttackCd > 0)
        {
            dashAttackCd = dashAttackCd - 1 * Time.deltaTime;
        }
        attackTime = attackTime - 1 * Time.deltaTime;
        dashTimer = dashTimer - 1 * Time.deltaTime;
        damageCd = damageCd - 1 * Time.deltaTime;
        if (attackTime <= 0f)
        {
            isAttacking = false;
        }
        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }
    private void Move()
    {
        Vector3 playerPosition = playerTransform.position;
        Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, moveSpeed * Time.deltaTime);
        float lookDir = 1;
        if (playerPosition.x > transform.position.x)
        {
            lookDir = -1;
        }
        transform.right = new Vector3(lookDir, 0f, 0f);
        isWalking = true;
    }
    private void Dash()
    {
        isWalking = false;
        Debug.Log("Dashing movement!");
        Vector3 playerPosition = playerTransform.position;
        Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, moveSpeed * 6f * Time.deltaTime);
        float lookDir = 1;
        if (playerPosition.x > transform.position.x)
        {
            lookDir = -1;
        }
        transform.right = new Vector3(lookDir, 0f, 0f);
    }
    private void CheckForWeaponCollision()
    {
           int a = weaponCollider.OverlapCollider(new ContactFilter2D(), objectsToDamageList);
           foreach (Collider2D collider in objectsToDamageList)
           {
               if (collider == playerTransform.gameObject.GetComponent<Collider2D>())
               {
                   if (damageCd <= 0)
                   {
                        playerTransform.gameObject.GetComponent<Player>().Damaged(enemyDamageValue);
                        damageCd = maxDamageCd;
                        Debug.Log("Applying Damage!");
                   }
               }
           }
    }
    private void Animate()
    {
        enemyAnimator.SetBool("walk", isWalking);
    }
    public void Damaged(float damageValue)
    {
        Debug.Log("ENEMY BEING DAMAGED!");
        if (health > 0f)
        {
            health = health - damageValue;
            GameObject damageNumberTransform = Instantiate(damageNumberPrefab, transform.position + new Vector3(0f, 1f, 0f), new Quaternion(0f, 0f, 0f, 0f));
            damageNumberTransform.GetComponent<DamageNumber>().ChangeNumber(damageValue);
            Debug.Log(health);
        }
        if (health <= 0)
        {
            enemyAnimator.SetTrigger("damage");
            Destroy(gameObject);
        }
    }
}
