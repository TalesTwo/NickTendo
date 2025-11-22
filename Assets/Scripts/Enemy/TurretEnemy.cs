using System;
using System.Collections;
using UnityEngine;

public class TurretEnemy : RangedEnemyController
{
    private Transform _spawnPoint;

    private bool _isFiring = false;
    private float fireDuration = 7f;
    private float waitDuration = 3f;
    [SerializeField] private float maxBeamLength = 100f;

    
    private Vector3 _smoothedDirection;
    
    [SerializeField] GameObject _laserBeam;


    protected override void Start()
    {
        base.Start();

        // find SPAWN_LOCATION child
        _spawnPoint = transform.Find("Laser_Origin");
        if (_spawnPoint == null)
        {
            Debug.LogError("TurretEnemy: No SPAWN_LOCATION child found!");
        }

        // begin shooting loop
        StartCoroutine(FiringLoop());
        _smoothedDirection = Vector3.right; // arbitrary initial direction
        
        // laser effect is hidden initially
        if (_laserBeam != null)
        {
            _laserBeam.SetActive(false);
        } else
        {
            Debug.LogWarning("TurretEnemy: No laserEffectPrefab assigned!");
        }

    }

    protected override void FindPath() { }
    protected override IEnumerator Follow() { yield break; }

    protected override void Attack()
{
    if (_spawnPoint == null) return;

    Vector3 start = _spawnPoint.position;

    // always aim laser at the player
    Vector3 dir = (_playerTransform.position - _spawnPoint.position).normalized;

    if (_isFiring)
    {
        // run a raycast to see if we hit something
        int finalMask = doNotHit | LayerMask.GetMask("Pits") |
                        LayerMask.GetMask("Spawning") |
                        LayerMask.GetMask("Loot") |
                        LayerMask.GetMask("Ignore Raycast") |
                        LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.Raycast(start, dir, maxBeamLength, ~finalMask);
        
        // ---------- LASER VISUAL ----------- //
        if (_laserBeam != null)
        {
            // ---------- Fire the raycast of the laser ---------- //
            // if we hit the player, visualize the ray to the hit point
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(start, hit.point, Color.red);
                
            }
            else if (hit.collider != null && hit.collider.gameObject != null)
            {
                // visualize actual ray to hit point
                Debug.DrawLine(start, hit.point, Color.yellow);
            }
            else
            {
                // visualize full ray if nothing hit
                Debug.DrawRay(start, dir * maxBeamLength, Color.cyan);
            }
            
            // ---------- Enable the prefab of the laser ---------- //

            if (!_laserBeam.activeSelf)
                _laserBeam.SetActive(true);

            Transform beamT = _laserBeam.transform;

            // root the beam at origin
            beamT.position = _spawnPoint.position;

            // rotate to face direction
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            beamT.rotation = Quaternion.Euler(0, 0, angle);

            // determine length: real hit or max
            float currentBeamLength = hit.collider != null ? hit.distance : maxBeamLength;

            // apply scaling (sprite pivot MUST be left-aligned)
            beamT.localScale = new Vector3(currentBeamLength, beamT.localScale.y, 1);
            
        }
    }
    else
    {
        if (_laserBeam != null && _laserBeam.activeSelf){ _laserBeam.SetActive(false);}
    }
}


    private IEnumerator FiringLoop()
    {
        while (true)
        {
            // 🔥 FIRE for 3 seconds
            _isFiring = true;
            yield return new WaitForSeconds(fireDuration);

            // 💤 REST for 3 seconds
            _isFiring = false;
            yield return new WaitForSeconds(waitDuration);
        }
    }
}