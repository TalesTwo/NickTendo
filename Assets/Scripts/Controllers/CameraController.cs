using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _playerTransform;
    public float cameraSpeed = 0.05f;
    public float BossCameraSize = 10f;
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
        EventBroadcaster.PlayerEnteredBossRoom += BossCameraToggle;
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
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            BossCameraToggle(true);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            BossCameraToggle(false);
        }
        
        Vector2 movement = Vector2.Lerp(_playerTransform.position, gameObject.transform.position, cameraSpeed*Time.deltaTime);
        gameObject.transform.position = new Vector3(movement.x, movement.y, -1);

        if (_cameraShake != null) { transform.position += _cameraShake.CurrentOffset; }

    }

    public void BossCameraToggle(bool inBossRoom)
    {
        if (inBossRoom)
        {
            _camera.orthographicSize = BossCameraSize;
        }
        else
        {
            _camera.orthographicSize = _originalCameraSize;
        }
    }
}
