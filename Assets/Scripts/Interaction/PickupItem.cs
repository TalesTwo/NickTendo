using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : BaseItem
{
    [Header("Item Stats")]
    private GameObject Player;
    public bool CanAutoPickup = true;
    private bool CanInteract;
    public PlayerStatsEnum BuffType;
    public float BuffValue = 1;

    [Header("Animation")]
    public Sprite[] Animation;
    public float FrameRate = 12;

    private SpriteRenderer SpriteRenderer;
    private float AnimationTimer;
    private float AnimationTimerMax;
    public int Index; 
    
    private Collider2D _triggerCollider;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        CanInteract = false;
        AnimationTimer = 0;
        AnimationTimerMax = 1/FrameRate;
        Index = 0;
        
        // Find the trigger collider
        foreach (var c in GetComponents<Collider2D>())
        {
            if (c.isTrigger)
            {
                _triggerCollider = c;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ItemAnimation();
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
        if (!_triggerCollider.enabled){ return; }
        
        //DebugUtils.Log("Picked up " + Name);
        if (!Player.GetComponent<PlayerController>().GetIsDead())
        {
            PlayerStats.Instance.ApplyItemBuffs(BuffType, BuffValue);
        }

        //sprite is destoryed first because delete the entire object skips the playing of the sfx
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject);
    }

    private void ItemAnimation()
    {
        AnimationTimer += Time.deltaTime;
        if (AnimationTimer > AnimationTimerMax)
        {
            AnimationTimer = 0;
            Index++;
            if (Index >= Animation.Length)
            {
                Index = 0;
            }
            SpriteRenderer.sprite = Animation[Index];
        }
    }

    private void DeleteItem()
    {
        Destroy(gameObject);
    }
}
