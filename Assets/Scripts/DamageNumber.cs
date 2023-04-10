using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    private float lifeTime=1f;
    private Rigidbody2D damageNumberRigidBody;

    private void Awake()
    {
        damageNumberRigidBody=GetComponent<Rigidbody2D>();
        damageNumberRigidBody.AddForce(RandomForce());
    }
    private void Update()
    {
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            lifeTime= lifeTime-Time.deltaTime;
        }
    }

    private Vector2 RandomForce()
    {
        return new Vector2(Random.Range(50f, 200f), Random.Range(50f, 200f));
    }

    public void ChangeNumber(float num)
    {
        GetComponent<TextMeshPro>().text = num.ToString();
    }
}
