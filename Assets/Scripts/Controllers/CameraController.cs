using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _playerTransform;
    public float cameraSpeed = 0.05f;
    public float normalCameraSize = 5f;
    public float BossCameraSize = 10f;
    public float BossCameraTransitionTime;
    private bool _inBossFight = false;
    private float _cameraVelocity;
    private CameraShake _cameraShake;
    
    private Camera _camera;
    private float _originalCameraSize;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _cameraShake = GetComponent<CameraShake>();
        _camera = GetComponent<Camera>();
        _originalCameraSize = _camera.orthographicSize;
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
        Vector2 movement = Vector2.Lerp(_playerTransform.position, gameObject.transform.position, cameraSpeed*Time.deltaTime);
        gameObject.transform.position = new Vector3(movement.x, movement.y, -1);

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
        while (_camera.orthographicSize - BossCameraSize < 0f)
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, BossCameraSize + 0.2f, ref _cameraVelocity,
                BossCameraTransitionTime);
            yield return null;
        }
    }

    private IEnumerator ToNormalCamera()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        while (_camera.orthographicSize - normalCameraSize > 0f)
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, normalCameraSize - 0.2f, ref _cameraVelocity,
                BossCameraTransitionTime);
            yield return null;
        }
    }
}
