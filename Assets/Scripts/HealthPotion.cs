using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    private float timeAlive=0f;
    private float destroyTime;
    private bool autoDestroy=false;

    private void Awake()
    {
        timeAlive = 0f;
    }

    private void Update()
    {
        timeAlive = timeAlive + Time.deltaTime;
        if (autoDestroy)
        {
            if (timeAlive >= destroyTime)
            {
                DestroySelf();
            }
        }
    }
    public void OnPlayerPickUp()
    {
        Player.Instance.AddHeallingPotions(1);
    }
    public void DestroyAfterTime(float destroyTime)
    {
        autoDestroy = true;
        this.destroyTime = destroyTime;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
