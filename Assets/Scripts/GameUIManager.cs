using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }
    private void Awake()
    {
        Instance= this;
    }
    [SerializeField] private Transform throwingObjectTransform;
    [SerializeField] private Transform healingPotionTransform;
    public void ChangeThrowingObjectCount(int throwingObjectCount)
    {
        throwingObjectTransform.GetComponent<TMP_Text>().text=throwingObjectCount.ToString();
    }
    public void ChangeHealingPotionCount(int healingPotionCount)
    {
        healingPotionTransform.gameObject.GetComponent<TMP_Text>().text = healingPotionCount.ToString();
    }
}
