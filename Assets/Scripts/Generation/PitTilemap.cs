using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class PitTilemap : MonoBehaviour
{
    private Tilemap tilemap;
    private bool _IsPlayerInPit = false;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        EventBroadcaster.PlayerFinishedDashing += OnPlayerFinishedDashing;
    }
    private void OnPlayerFinishedDashing(GameObject player)
    {
        
        // Find the child of name "PitCollider"
        GameObject pitColliderObj = player.transform.Find("PitCollider")?.gameObject;
        if (pitColliderObj == null) {return;}
        Collider2D pitCollider = pitColliderObj.GetComponent<Collider2D>();
        if (pitCollider == null) {return;}

        if (_IsPlayerInPit)
        {
            OnTriggerEnter2D(pitCollider);
        }
        
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if(other == null || other.transform.parent == null){return;}
        GameObject root = other.transform.parent.gameObject;
        if(root == null){return;}
        if (root.CompareTag("Player"))
        {
            DebugUtils.LogError("Player exited pit collider");
            _IsPlayerInPit = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If we dont hit specifically the pit collider, skip it
        if (!other.CompareTag("Pit_Collider"))
            return;
    
        // Get the parent object (the actual enemy or player) since we are a child collider
        GameObject root = other.transform.parent.gameObject;

        // Make sure it’s something that can fall into pits
        if (!root.CompareTag("Player") && !root.CompareTag("Enemy"))
            return;
        if (root.CompareTag("Player"))
        {
            DebugUtils.LogSuccess("Player entered pit collider");
            _IsPlayerInPit = true;
        }
        Vector3 hitPosition = other.transform.position;
        Vector3Int cellPos = tilemap.WorldToCell(hitPosition);
        Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPos);


        // Broadcast event with the correct pit center
        EventBroadcaster.Broadcast_ObjectFellInPit(root, tileCenter);
    }
    
}