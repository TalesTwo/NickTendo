using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : BaseItem
{
    private GameObject Player;
    public AudioSource PickupSFX;
    public bool CanAutoPickup = true;
    private bool CanInteract;
    public PlayerStatsEnum BuffType;
    public float BuffValue = 1;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PickupSFX.playOnAwake = false;
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
        PlayerStats.Instance.ApplyItemBuffs(BuffType, BuffValue);
        //PlayerStats.Instance.DisplayAllStats();
        PickupSFX.Play();
        //sprite is destoryed first because delete the entire object skips the playing of the sfx
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<Collider2D>());
        //delays destruction according to the lenght of the sfx
        Destroy(gameObject, PickupSFX.clip.length);
    }
}
