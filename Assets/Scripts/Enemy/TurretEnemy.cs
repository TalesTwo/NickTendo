using System;
using System.Collections;
using UnityEngine;

public class TurretEnemy : RangedEnemyController
{
    private Transform _spawnPoint;

    private bool _isFiring = false;
    private float fireDuration = 3f;
    private float waitDuration = 3f;
    
    private Vector3 _smoothedDirection;


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

    }

    protected override void FindPath() { }
    protected override IEnumerator Follow() { yield break; }

    protected override void Attack()
    {
        DebugUtils.Log("TurretEnemy: Attack called");
        // DO NOT use ranged enemy shooting system yet
        // For now only draw debug lines
        if (_spawnPoint == null) return;

        Vector3 start = _spawnPoint.position;
        Vector3 dir = _direction.normalized;

        if (_isFiring)
        {
            // red = firing
            Debug.DrawLine(start, start + dir * 10f, Color.red);
        }
        else
        {
            // green = idle
            Debug.DrawLine(start, start + dir * 10f, Color.green);
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