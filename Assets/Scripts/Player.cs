using PlasticGui.WorkspaceWindow.Replication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerJump;
    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerThrowAttack;
    public event EventHandler OnPlayerDash;
    public event EventHandler OnPlayerInteract;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform slashPointTransform;
    [SerializeField] private Transform dashTrailTransform;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Transform wallCheckTransform;
    [SerializeField] private GameObject rangedAttackObject;
    [SerializeField] private Transform playerThrowPoint;
    [SerializeField] private Transform healthBar;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private Transform weapon;
    //[SerializeField] private Transform gameUIManagerTransform;

    private TrailRenderer slashPointTrailRenderer;
    private CapsuleCollider2D groundCollider;
    private CapsuleCollider2D playerCollider;
    private CapsuleCollider2D wallCollider;
    private Rigidbody2D playerRigidbody;
    private List<Collider2D> groundColliderList;
    private List<Collider2D> wallColliderList;
    private Collider2D weaponCollider;

    private Vector3 moveDir;
    private static Vector2 throwDir;

    private float health=100f;
    private float jumpHeight = 500;
    private float startingMoveSpeed = 0.06f;
    private float moveSpeed;
    private float runningAttackCD = 0f;
    private float dashCd=0f;
    private float maxDashCd = 1.5f;
    private float acceleration = 1f;
    private float maxMoveSpeed = 0.16f;
    private float lasty;
    private float dashTrailTimer = 0f;
    private float maxDashTrailTimer = 0.4f;

    private int healingPotionCount=0;
    private int throwingObjectsCount=5;

    private bool isFalling = false;
    private bool isAttacking;
    private bool isJumping;
    
    private void Awake()
    {
        Instance = this;
        groundCollider = groundCheckTransform.GetComponent<CapsuleCollider2D>();
        wallCollider = wallCheckTransform.GetComponent<CapsuleCollider2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        moveDir = new Vector3(0, 0, 0);
        throwDir = new Vector2(1f, 0f);
        groundColliderList = new List<Collider2D>();
        wallColliderList = new List<Collider2D>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        slashPointTrailRenderer = slashPointTransform.GetComponent<TrailRenderer>();
        DisableSlashTrailRenderer();
        healingPotionCount = 0;
        throwingObjectsCount = 5;
    }
    void Start()
    {
        GameUIManager.Instance.GetComponent<GameUIManager>().ChangeThrowingObjectCount(throwingObjectsCount);
        GameUIManager.Instance.GetComponent<GameUIManager>().ChangeHealingPotionCount(healingPotionCount);
        float lasty = transform.position.y;
        isJumping = false;
        isAttacking = false;
        gameInput.OnPlayerAttack += GameInput_OnPlayerAttack;
        gameInput.OnPlayerJump += GameInput_OnPlayerJump;
        gameInput.OnPlayerInteract += GameInput_OnPlayerInteract;
        gameInput.OnPlayerRangedAttack += GameInput_OnPlayerRangedAttack;
        gameInput.OnPlayerDash += GameInput_OnPlayerDash;
    }

    private void GameInput_OnPlayerDash(object sender, EventArgs e)
    {
        if (dashCd <= 0)
        {
            Vector2 dashDir = gameInput.GetMoveInputNormalized();
            if (dashDir.y > 0)
            {
                dashDir = new Vector2(dashDir.x, dashDir.y + 0.4f);
            }
            Debug.Log("dashDir= " + dashDir);
            playerRigidbody.AddForce(dashDir * 300f);
            OnPlayerDash?.Invoke(this, EventArgs.Empty);
            dashTrailTransform.gameObject.SetActive(true);
            dashTrailTimer = maxDashTrailTimer;
            dashCd = maxDashCd;
        }
    }

    private void GameInput_OnPlayerRangedAttack(object sender, EventArgs e)
    {
        if (!IsAttacking())
        {
            if (throwingObjectsCount > 0)
            {
                throwingObjectsCount--;
                OnPlayerThrowAttack?.Invoke(this, e);
                Instantiate(rangedAttackObject, playerThrowPoint.transform.position, transform.rotation);
                GameUIManager.Instance.GetComponent<GameUIManager>().ChangeThrowingObjectCount(throwingObjectsCount);
            }
        }
    }

    private void GameInput_OnPlayerInteract(object sender, EventArgs e)
    {
        OnPlayerInteract?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnPlayerJump(object sender, EventArgs e)
    {
        if (!IsJumping() && IsTouchingGround() && !IsFalling())
        {
            isJumping = true;
            playerRigidbody.AddForce(new Vector2(1, jumpHeight));
            OnPlayerJump?.Invoke(this, EventArgs.Empty);
            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + jumpHeight, transform.position.z), jumpSpeed*Time.deltaTime ); 
        }
    }

    private void GameInput_OnPlayerAttack(object sender, EventArgs e)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
        if (IsRunning() && runningAttackCD <= 0)
        {
            playerRigidbody.AddForce(new Vector2(-200f * moveDir.x, 0f));
        }
    }

    void Update()
    {
        HandleMovement();
        if (IsTouchingGround())
        {
            isJumping = false;
            isFalling = false;
        }
        else
        {
            if (lasty > transform.position.y)
            {
                isFalling = true;
            }
        }
        if (runningAttackCD > 0f)
        {
            runningAttackCD = runningAttackCD - 1 * Time.deltaTime;
        }
        if (dashCd > 0f)
        {
            dashCd = dashCd - 1 * Time.deltaTime;
        }
        if (dashTrailTimer > 0f)
        {
            
            dashTrailTimer = dashTrailTimer- 1 * Time.deltaTime;
        }
        if (dashTrailTimer <= 0f)
        {
            dashTrailTransform.gameObject.SetActive(false);
        }
        lasty = transform.position.y;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMoveInputNormalized();
        float moveX = 0;
        if (inputVector.x < 0)
        {
            moveX = -1;
        }
        else if(inputVector.x>0)
        {
            moveX = 1;
        }
        moveDir = new Vector3(moveX, 0, 0);
        if (!IsTouchingWall())
        {
            if (!IsJumping() && !IsFalling())
            {
                if (IsMoving() && !IsAttacking())
                {
                    moveSpeed = moveSpeed * acceleration;
                    transform.position = transform.position + moveDir * moveSpeed;
                    transform.right = new Vector3(moveDir.x, 0f, 0f);
                    throwDir = new Vector2(moveDir.x, moveDir.y);
                    if (moveSpeed >= maxMoveSpeed)
                    {
                        moveSpeed = maxMoveSpeed;
                        acceleration = 1f;
                    }
                    else
                    {
                        acceleration = acceleration + 0.02f * Time.deltaTime;
                    }
                }
                else if (!IsMoving() || IsAttacking())
                {

                    if (moveSpeed > startingMoveSpeed)
                    {
                        moveSpeed = moveSpeed - 0.11f * Time.deltaTime;
                    }
                    else
                    {
                        moveSpeed = startingMoveSpeed;
                    }
                }
            }
            else
            {
                transform.position = transform.position + moveDir * moveSpeed;
                if (moveDir != transform.right && moveDir.x != 0f)
                {
                    transform.right = new Vector3(moveDir.x, 0f, 0f);
                    throwDir = new Vector2(moveDir.x, moveDir.y);
                }
                else
                {
                    transform.right = new Vector3(transform.right.x, 0f, 0f);
                    throwDir = new Vector2(transform.right.x, moveDir.y);
                }
            }
        }
        else
        {
            transform.right = new Vector3(moveDir.x, 0f, 0f);
            throwDir =new  Vector2 (moveDir.x, moveDir.y);
            Debug.Log("is touching wall!");
        }
    }

    public bool IsTouchingWall()
    {
        int a = wallCollider.OverlapCollider(new ContactFilter2D(), wallColliderList);
        foreach (Collider2D collider2D in wallColliderList)
        {
            //int b = wallCollider.OverlapCollider(new ContactFilter2D(), wallColliderList);
            if (collider2D.gameObject.GetComponent<GroundColider>() != null)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsTouchingGround()
    {
        int a = groundCollider.OverlapCollider(new ContactFilter2D(), groundColliderList);
        foreach (Collider2D collider2D in groundColliderList)
        {
            if (collider2D.gameObject.GetComponent<GroundColider>()!=null)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsFalling()
    {
        return isFalling;
    }
    public bool IsJumping()
    {
        return isJumping;
    }

    public bool IsMoving()
    {
        if (moveDir.x != 0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsRunning()
    {
        if (moveSpeed >= maxMoveSpeed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void StopAttacking()
    {
        isAttacking = false;
    }
    public void SetAttacking(bool attackState)
    {
        isAttacking = attackState;
    }
    public bool IsAttacking()
    {
        return isAttacking;
    }
    public float GetRunningAttackCD()
    {
        return runningAttackCD;
    }
    public void SetRunningAttackCD(float runningAttackCD)
    {
        this.runningAttackCD = runningAttackCD;
    }
    public void EnableSlashTrailRenderer()
    {
        slashPointTrailRenderer.enabled = true;
    }
    public void DisableSlashTrailRenderer()
    {
        slashPointTrailRenderer.enabled = false;
    }
    public static Vector2 GetThrowDir()
    {
        if (throwDir.x != 0)
        {
            return throwDir;
        }
        else
        {
            return new Vector2(1f, 0f);
        }
    }

    public void AddHeallingPotions(int amount)
    {
        healingPotionCount = healingPotionCount+ amount;
        GameUIManager.Instance.ChangeHealingPotionCount(healingPotionCount);
        Debug.Log("New potion count= " + healingPotionCount);
    }

    public void Damaged(float damageValue)
    {
        if (health > 0)
        {
            health = health - damageValue;
            Image healthBarImg = healthBar.GetComponent<Image>();
            healthBarImg.fillAmount = health/100;
            GameObject damageNumberTransform=Instantiate(damageNumberPrefab,transform.position+new Vector3(0f,1f,0f),new Quaternion(0f,0f,0f,0f));
            damageNumberTransform.GetComponent<DamageNumber>().ChangeNumber(damageValue);
            Debug.Log(health);
        }
    }

}
