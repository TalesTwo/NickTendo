using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    // Enum to hold the current state of this door
    public enum DoorState
    {
        Open,
        Closed,
        Locked
    }
    
    // Gameobjects to hold the sprites for the door states
    [SerializeField] private Sprite openDoor;
    [SerializeField] private Sprite closedDoor;
    [SerializeField] private Sprite lockedDoor;
    private DoorState _currentState = DoorState.Closed;
    
    // Color to hold the "opened" color of the door
    // the first time we interact with a door, we will change it to be opened
    [SerializeField] private Color openedColor = Color.white;
    [SerializeField] private Color closedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;
    
    // get access to our door trigger interaction script
    [SerializeField] private DoorTriggerInteraction _doorTriggerInteraction;
    
    
    void Start()
    {
        // Default state is closed
        SetDoorState(DoorState.Closed);
    }
    
    public void SetDoorState(DoorState newState)
    {
        _currentState = newState;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null){ return;}
        switch (_currentState)
        {
            case DoorState.Open:
                spriteRenderer.sprite = openDoor;
                spriteRenderer.color = openedColor;
                _doorTriggerInteraction.SetInteractAllowedToInteract(true);
                break;
            case DoorState.Closed:
                spriteRenderer.sprite = closedDoor;
                spriteRenderer.color = closedColor;
                _doorTriggerInteraction.SetInteractAllowedToInteract(true);
                break;
            case DoorState.Locked:
                spriteRenderer.sprite = lockedDoor;
                spriteRenderer.color = lockedColor;
                _doorTriggerInteraction.SetInteractAllowedToInteract(false);
                break;
            default:
                Debug.LogError("Invalid door state");
                break;
        }
    }
    public DoorState GetCurrentState()
    {
        return _currentState;
    }
    
}
