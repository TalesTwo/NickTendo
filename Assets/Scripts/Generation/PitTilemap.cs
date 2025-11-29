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
            _IsPlayerInPit = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        //Debug.Log("PitTilemap detected trigger enter with: " + other.name);
        // null checks
        // If its not a pit collider, and not an item, ignore
        if (!other.CompareTag("Pit_Collider") && !other.GetComponent<BaseItem>()) { return;}
        // get access to the current room
        Room currentRoom = DungeonController.Instance.GetCurrentRoom();
        if (currentRoom == null) { return; }
        // get the RoomGridController
        RoomGridManager gridManager = currentRoom.GetComponentInChildren<RoomGridManager>();
        if (gridManager == null) { return; }
        Vector3 tileCenter;
        
        BaseItem item = other.GetComponent<BaseItem>();
        if (item != null)
        {
            // we want the item to "fall" in the same way that enemies and players do
            tileCenter = gridManager.GetNearestPitToLocation(item.transform.position);
            item.FellInPit(tileCenter);
            return;
        }
        
        // Get the parent object (the actual enemy or player) since we are a child collider
        GameObject root = other.transform.parent.gameObject;

        // Make sure it’s something that can fall into pits
        if (!root.CompareTag("Player") && !root.CompareTag("Enemy"))
            return;
        if (root.CompareTag("Player"))
        {
            _IsPlayerInPit = true;
        }
        

        // find the nearest pit location to the object that fell in
        tileCenter = gridManager.GetNearestPitToLocation(root.transform.position);

        // Broadcast event with the correct pit center
        EventBroadcaster.Broadcast_ObjectFellInPit(root, tileCenter);
    }
    
}