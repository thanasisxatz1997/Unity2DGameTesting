using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private float damageValue=10f;
    private float damageCd = 0f;
    private float maxDamageCd = 1f;
    private CapsuleCollider2D capsuleCollider2D;
    private List<Collider2D> colliderList; 
    private void Awake()
    {
        capsuleCollider2D=GetComponent<CapsuleCollider2D>();
        colliderList = new List<Collider2D>();
    }
    private void Update()
    {
        HandleDamage();
    }
    private void HandleDamage()
    {
        int a = capsuleCollider2D.OverlapCollider(new ContactFilter2D() ,colliderList);
        foreach (Collider2D collider in colliderList)
        {
            if (collider.gameObject.GetComponent<Player>())
            {
                if (damageCd <= 0)
                {
                    collider.gameObject.GetComponent<Player>().Damaged(damageValue);
                    damageCd = maxDamageCd;
                }
            }
        }
        if(damageCd>0)
        {
            damageCd = damageCd - 1f * Time.deltaTime;
        }
    }
}
