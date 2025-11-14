using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGuard : MonoBehaviour
{

    // create an overlap function that teleports the player back to the "Spawn_location" if they try to go through the door
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("DoorGuard: Player tried to go through a guarded door. Teleporting back to spawn location.");
            // teleport the player back to the spawn location
            // Get the parent of this script
            Transform spawnLocation = transform.parent.Find("Spawn_Location");
            if (spawnLocation != null)
            {
                other.transform.position = spawnLocation.position;
                //other.transform.rotation = spawnLocation.rotation;
            }
        }
    }
}
