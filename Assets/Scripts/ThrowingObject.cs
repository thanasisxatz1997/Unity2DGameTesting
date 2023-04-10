using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowingObject : MonoBehaviour
{
    private Rigidbody2D throwingObjectRigidBody;
    private CapsuleCollider2D capsuleCollider2D;
    private List<Collider2D> colliderList;
    private Vector2 throwDir;
    private float moveSpeed=0.3f;
    private float lifeTime=0.5f;
    private float damageValue = 20f;

    private void Awake()
    {
        throwingObjectRigidBody= GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        colliderList = new List<Collider2D>();
        throwDir = new Vector2(1f, 0f);
    }
    private void Start()
    {
        throwDir = Player.GetThrowDir();
    }
    private void Update()
    {
        HandleMovement();
        //CheckForCollisions();
        lifeTime = lifeTime - 1 * Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void HandleMovement()
    {
        Debug.Log(throwDir.x);
        transform.position = new Vector3(transform.position.x +(throwDir.x * moveSpeed), transform.position.y, transform.position.z);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Enemy>())
        {
            collision.transform.GetComponent<Enemy>().Damaged(damageValue);
            Destroy(gameObject);
        }
    }
    private void CheckForCollisions()
    {
        int a = capsuleCollider2D.OverlapCollider(new ContactFilter2D(), colliderList);
        foreach (Collider2D collider in colliderList)
        {
            Debug.Log(collider.gameObject.name);
            if (collider.gameObject.GetComponent<Enemy>())
            {
                collider.gameObject.GetComponent<Enemy>().Damaged(damageValue);
                Destroy(gameObject);
            }
        }
    }

}
