using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class CollectableSO : ScriptableObject
{
    public string collectableName;
    public GameObject prefab;
    public Sprite image;
    
}
