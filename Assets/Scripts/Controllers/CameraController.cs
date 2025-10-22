using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _playerTransform;
    public float cameraSpeed = 0.05f;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        /*
        float distance = Vector3.Distance(_playerTransform.position, gameObject.transform.position);
        if (distance > 50f)
        {
            gameObject.transform.position = new Vector3(_playerTransform.position.x, _playerTransform.position.y, -1);
        }
        else if (distance > 1.5f)
        {
        
            Vector2 movement = Vector2.MoveTowards(gameObject.transform.position, _playerTransform.position, Time.deltaTime * cameraSpeed * distance);
          */  
        
            Vector2 movement = Vector2.Lerp(_playerTransform.position, gameObject.transform.position, cameraSpeed*Time.deltaTime);
            gameObject.transform.position = new Vector3(movement.x, movement.y, -1);
            /*
        }
        */

    }
}
