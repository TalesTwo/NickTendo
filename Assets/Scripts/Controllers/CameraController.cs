using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _playerTransform;
    public float cameraSpeed = 0.05f;
    public float normalCameraSize = 5f;
    
    [Header("Boss Camera")]
    public float BossCameraSize = 10f;
    public float BossCameraTransitionTime;
    public float BossCameraOffsetY;
    public float BossCameraLeanPercentage;
    private Vector3 _bossCameraAnchorPoint;
    private bool _inBossFight = false;
    
    
    private float _cameraVelocity;
    private Vector3 _cameraVelocity2;
    private CameraShake _cameraShake;
    
    private Camera _camera;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _cameraShake = GetComponent<CameraShake>();
        _camera = GetComponent<Camera>();
        EventBroadcaster.PlayerDamaged += OnPlayerDamaged;
        EventBroadcaster.StartBossFight += BossFightStarting;
        EventBroadcaster.PlayerDeath += BossFightEnding;
    }

    private void OnPlayerDamaged()
    {
        // Call the camera shake effect
        ShakeCamera(0.15f, 0.3f);
    }
    
    public void ShakeCamera(float duration, float magnitude)
    {
        _cameraShake?.ShakeOnce(duration, magnitude);
    }
    
    void OnDestroy()
    {
        EventBroadcaster.PlayerDamaged -= OnPlayerDamaged;
        EventBroadcaster.StartBossFight -= BossFightStarting;
        EventBroadcaster.PlayerDeath -= BossFightEnding;
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        if (!_inBossFight)
        {
            Vector2 movement = Vector2.Lerp(_playerTransform.position, gameObject.transform.position, cameraSpeed*Time.deltaTime);
            gameObject.transform.position = new Vector3(movement.x, movement.y, -1);
        }
        else
        {
            Vector3 anchorToPlayer = _playerTransform.position - _bossCameraAnchorPoint;
            Vector3 leanOffset = anchorToPlayer.normalized * BossCameraLeanPercentage;
            Vector3 targetPosition = _bossCameraAnchorPoint + leanOffset;
            targetPosition.z = gameObject.transform.position.z;
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed*Time.deltaTime);
            Debug.Log(transform.position);
        }
        

        if (_cameraShake != null) { transform.position += _cameraShake.CurrentOffset; }
    }

    private void BossFightStarting()
    {
        StartCoroutine(ToBossCamera());
        _inBossFight = true;
    }

    private void BossFightEnding()
    {
        StartCoroutine(ToNormalCamera());
        _inBossFight = false;
    }

    private IEnumerator ToBossCamera()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        _bossCameraAnchorPoint = new Vector2(boss.gameObject.transform.position.x, boss.gameObject.transform.position.y + BossCameraOffsetY);
        while (_camera.orthographicSize - BossCameraSize < 0f || Vector3.Distance(_bossCameraAnchorPoint, gameObject.transform.position) > 0.1f)
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, BossCameraSize + 0.2f, ref _cameraVelocity,
                BossCameraTransitionTime);
            Vector3 movement = Vector3.SmoothDamp(gameObject.transform.position, _bossCameraAnchorPoint,
                ref _cameraVelocity2, BossCameraTransitionTime);
            movement.z = gameObject.transform.position.z;
            gameObject.transform.position = movement;
            yield return null;
        }
    }

    private IEnumerator ToNormalCamera()
    {
        while (_camera.orthographicSize - normalCameraSize > 0f)
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, normalCameraSize - 0.2f, ref _cameraVelocity,
                BossCameraTransitionTime);
            yield return null;
        }
    }
}
