using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private List<CollectableSO> containedItemSOList;
    [SerializeField] private Transform itemDisplayPoint;
    [SerializeField] private Transform crateVisualsTransform;

    private Animator animator;
    private CapsuleCollider2D capsuleCollider;
    private bool isCollidingWithPlayer=false;
    private bool opened = false;

    private void Awake()
    {
        animator = crateVisualsTransform.GetComponent<Animator>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerInteract += Instance_OnPlayerInteract;
        //Debug.Log("Position= " + itemDisplayPoint.transform.position);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Player>())
        {
            Debug.Log("Collided with player");
            isCollidingWithPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Player>())
        {
            isCollidingWithPlayer = false;
        }
    }
    private void Instance_OnPlayerInteract(object sender, System.EventArgs e)
    {
        if (isCollidingWithPlayer && opened==false)
        {
            Debug.Log("Interacting!");
            opened = true;
            //Vector3 startingDisplayPoint = itemDisplayPoint.position;
           // itemDisplayPoint.position = itemDisplayPoint.position - new Vector3((containedItemSOList.Count-1), 0f, 0f);
            foreach (CollectableSO collectable in containedItemSOList)
            {
                ChestOpenAnimation();
                Debug.Log("Position= " + itemDisplayPoint.transform.position);
                GameObject healthPotion=Instantiate(collectable.prefab,itemDisplayPoint);
                healthPotion.GetComponent<HealthPotion>().OnPlayerPickUp();
                healthPotion.GetComponent<HealthPotion>().DestroyAfterTime(3f);
                //itemDisplayPoint.position = itemDisplayPoint.position + new Vector3(1f, 0f, 0f);
            }
            //itemDisplayPoint.position = startingDisplayPoint;
        }
    }
    private void ChestOpenAnimation()
    {
        animator.SetTrigger("crateOpen");
    }
}
