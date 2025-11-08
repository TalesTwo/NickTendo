using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class PitManager : Singleton<PitManager>
    {
        private readonly List<PitTilemap> _activePits = new List<PitTilemap>();
        private LayerMask _pitsLayer;

        protected override void Awake()
        {
            base.Awake();
            _pitsLayer = LayerMask.NameToLayer("Pits");
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
                if (t.gameObject.layer == _pitsLayer)
                {
                    var pit = t.GetComponent<PitTilemap>();
                    if (pit == null)
                        pit = t.gameObject.AddComponent<PitTilemap>();

                    _activePits.Add(pit);
                }
            }
        }

        private void ClearAllPits()
        {
            foreach (var pit in _activePits)
            {
                if (pit != null)
                    Destroy(pit);
            }

            _activePits.Clear();
        }
    }
}

