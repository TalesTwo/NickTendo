using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow")]
    public float followSpeed = 5f;
    private Transform player;

    [Header("Normal Camera")]
    public float normalSize = 5f;

    [Header("Boss Camera")]
    public float bossSize = 10f;
    public float bossTransitionTime = 1.5f;
    public float bossOffsetY = -5f;
    public float leanPercent = 0.15f;
    private Vector3 bossAnchor;

    private Camera cam;
    private CameraShake cameraShake;

    private Coroutine zoomRoutine;
    private bool inBossFight = false;
    private bool transitioning = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = GetComponent<Camera>();
        cameraShake = GetComponent<CameraShake>();

        EventBroadcaster.PlayerDamaged += OnPlayerDamaged;
        EventBroadcaster.StartBossFight += OnBossStart;
        EventBroadcaster.PlayerDeath += OnBossEnd;
    }

    private void OnDestroy()
    {
        EventBroadcaster.PlayerDamaged -= OnPlayerDamaged;
        EventBroadcaster.StartBossFight -= OnBossStart;
        EventBroadcaster.PlayerDeath -= OnBossEnd;
    }

    private void FixedUpdate()
    {
        if (!transitioning) 
        {
            if (!inBossFight)
            {
                FollowPlayer();
            }
            else
            {
                FollowBossAnchor();
            }
        }

        // Add camera shake offset
        if (cameraShake != null)
            transform.position += cameraShake.CurrentOffset;
    }

    private void FollowPlayer()
    {
        Vector3 target = new Vector3(player.position.x, player.position.y, -1f);
        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
    }

    private void FollowBossAnchor()
    {
        // Lean toward the player slightly
        Vector3 toPlayer = (player.position - bossAnchor).normalized * leanPercent;
        Vector3 target = bossAnchor + toPlayer;
        target.z = -1f;

        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
    }

    private void OnPlayerDamaged()
    {
        cameraShake?.ShakeOnce(0.15f, 0.3f);
    }

    // -------------------------
    //        BOSS EVENTS
    // -------------------------

    private void OnBossStart()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        bossAnchor = boss.transform.position + new Vector3(0, bossOffsetY, 0);

        StartZoom(bossSize, bossAnchor);
        inBossFight = true;
    }

    private void OnBossEnd()
    {
        StartZoom(normalSize, player.position);
        inBossFight = false;
    }

    // -------------------------
    //          ZOOM
    // -------------------------

    private void StartZoom(float targetSize, Vector3 targetPos)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomRoutine(targetSize, targetPos));
    }

    private IEnumerator ZoomRoutine(float targetSize, Vector3 targetPos)
    {
        transitioning = true;

        float startSize = cam.orthographicSize;
        Vector3 startPos = transform.position;

        float time = 0f;

        while (time < bossTransitionTime)
        {
            time += Time.deltaTime;
            float t = time / bossTransitionTime;

            // Smooth ease
            t = Mathf.SmoothStep(0f, 1f, t);

            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            // Only move toward the target anchor during transition
            Vector3 pos = Vector3.Lerp(startPos, new Vector3(targetPos.x, targetPos.y, -1f), t);
            transform.position = pos;

            yield return null;
        }

        cam.orthographicSize = targetSize;
        transitioning = false;
    }
}

