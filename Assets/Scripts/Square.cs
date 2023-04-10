using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private float health = 100f;
    [SerializeField] Transform damageNumberPrefab;
    private void Awake()
    {
        
    }
    public void Damaged(float damageValue)
    {
        Debug.Log("square BEING DAMAGED!");
        if (health > 0f)
        {
            health = health - damageValue;
            Transform damageNumberTransform = Instantiate(damageNumberPrefab, transform.position + new Vector3(0f, 1f, 0f), new Quaternion(0f, 0f, 0f, 0f));
            damageNumberTransform.GetComponent<DamageNumber>().ChangeNumber(damageValue);
            Debug.Log(health);
        }
    }
}
