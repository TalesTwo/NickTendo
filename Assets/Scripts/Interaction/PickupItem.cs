using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : BaseItem
{
    private GameObject Player;
    public bool CanAutoPickup = true;
    private bool CanInteract;
    public PlayerStatsEnum BuffType;
    public float BuffValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        CanInteract = false;
    }

    // Update is called once per frame
    void Update()
    {    
        if (!CanInteract) { return; }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (collision.gameObject == Player)
        {   
            if (CanAutoPickup)
            {
                Pickup();
            }
            else
            {
                CanInteract = true;
            }
        }
    }
    private void Pickup()
    {
        DebugUtils.Log("Picked up " + Name);
        if (!Player.GetComponent<PlayerController>().GetIsDead())
        {
            PlayerStats.Instance.ApplyItemBuffs(BuffType, BuffValue);
        }

        //sprite is destoryed first because delete the entire object skips the playing of the sfx
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject);
    }

    private void DeleteItem()
    {
        Destroy(gameObject);
    }
}
