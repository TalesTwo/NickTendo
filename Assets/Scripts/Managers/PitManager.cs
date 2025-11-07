using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PitManager : Singleton<PitManager>
{
    private readonly List<PitTilemap> activePits = new List<PitTilemap>();
    private LayerMask pitsLayer;

    private void Awake()
    {
        pitsLayer = LayerMask.NameToLayer("Pits");
    }

    private void Start()
    {
        EventBroadcaster.GameRestart += ClearAllPits;
    }

    private void OnDisable()
    {
        EventBroadcaster.GameRestart -= ClearAllPits;
    }

    public void RegisterPitsInRoom(Room room)
    {
        var tilemaps = room.GetComponentsInChildren<TilemapCollider2D>(true);

        foreach (var t in tilemaps)
        {
            if (t.gameObject.layer == pitsLayer)
            {
                var pit = t.GetComponent<PitTilemap>();
                if (pit == null)
                    pit = t.gameObject.AddComponent<PitTilemap>();

                activePits.Add(pit);
            }
        }
    }

    private void ClearAllPits()
    {
        foreach (var pit in activePits)
        {
            if (pit != null)
                Destroy(pit);
        }

        activePits.Clear();
    }
}

