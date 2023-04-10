using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float damageValue=40f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Weapon collided with" + collision.gameObject.name);
        if (collision.gameObject.GetComponent<Enemy>())
        {
            collision.gameObject.GetComponent<Enemy>().Damaged(damageValue);
        }
    }
}
