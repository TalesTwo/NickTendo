using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteractBase : MonoBehaviour, IInteractable
{
    public GameObject Player { get; set; }
    public bool CanInteract { get; set; }

    public virtual void Interact() { }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (!CanInteract) { return; }
        // if we press our Interact key, we will interact with the object
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            DebugUtils.Log("Player in range to interact with " + gameObject.name);
            CanInteract = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            DebugUtils.Log("Player out of range to interact with " + gameObject.name);
            CanInteract = false;
        }
    }
    
}
