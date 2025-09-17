using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : Singleton<SceneSwapManager>
{
    
    private DoorTriggerInteraction.DoorToSpawnAt _doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
    
    private static bool _loadFromDoor;
    
    private GameObject _player;
    private Collider2D _playerCollider;
    private Collider2D _doorCollider;
    private Vector3 _playerSpawnPosition;
    
    
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerCollider = _player.GetComponent<Collider2D>();
    }
    
    public static void SwapSceneFromDoorUse(SceneField sceneToLoad, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None)
    {
        _loadFromDoor = true;
        Instance.StartCoroutine(Instance.FadeOutThenChangeScene(sceneToLoad, doorToSpawnAt));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    
    private IEnumerator FadeOutThenChangeScene(SceneField sceneToLoad, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None)
    {
        // Start fade out animation here (you'll need to implement this)
        SceneFadeManager.Instance.StartFadeOut();
        while(SceneFadeManager.Instance.IsFadingOut)
        {
            yield return null; // Wait until fade out is complete
        }

        
        DebugUtils.LogSuccess("Faded out, now changing scene to: " + sceneToLoad);
        // Load the new scene
        _doorToSpawnAt = doorToSpawnAt;
        SceneManager.LoadScene(sceneToLoad);
    }
    
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        DebugUtils.LogSuccess("Scene loaded: " + scene.name);
        // Start fade in animation here (you'll need to implement this)
        SceneFadeManager.Instance.StartFadeIn();
        if (_loadFromDoor)
        {
            // warp the player to the correct door position
            FindDoor(_doorToSpawnAt);
            _player.transform.position = _playerSpawnPosition;
            _loadFromDoor = false;
        }
    }
    
    private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt)
    {
        DoorTriggerInteraction[] doorsInScene = FindObjectsOfType<DoorTriggerInteraction>();
        foreach (var door in doorsInScene)
        {
            if (door.CurrentDoorPosition == doorToSpawnAt)
            {
                _doorCollider = door.GetComponent<Collider2D>();
                CalculatePlayerSpawnPosition();
                return;
            }
        }
        DebugUtils.LogError("No door found in scene with DoorToSpawnAt: " + doorToSpawnAt);
    }
    
    private void CalculatePlayerSpawnPosition()
    {
        if (_playerCollider == null || _doorCollider == null)
        {
            DebugUtils.LogError("Player or Door collider is null, cannot calculate spawn position.");
            return;
        }
        
        // the door actually has a child called Spawn_Location, that is where we want to spawn the player
        // if that is not present, we will default to the center of the door collider
        Transform spawnLocation = _doorCollider.transform.Find("Spawn_Location");
        if (spawnLocation != null)
        {
            _playerSpawnPosition = spawnLocation.position;
            return;
        }
        
        Vector3 doorPosition = _doorCollider.bounds.center;
        Vector3 playerOffset = new Vector3(0, 0, 0); // Slightly above the door
        _playerSpawnPosition = doorPosition + playerOffset;
    }
}
