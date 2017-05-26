using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuItemSelectable : MonoBehaviour
{
    [SerializeField] private UnityEvent onSelectionEvent;

    private Vector3 originalScale;
    private Vector3 growScale;
    private Animator anim;

    private void Start()
    {
        originalScale = transform.localScale;
        anim = gameObject.GetComponent<Animator>();
    }

    public void RayOn()
    {
        anim.SetBool("Grow", true);
    }

    public void RayOff()
    {
        anim.SetBool("Grow", false);
    }

    public void ItemSelect()
    {
        onSelectionEvent.Invoke();
    }

    //Test
    public void printHit(int i)
    {
        Debug.Log(i);
    }
}
