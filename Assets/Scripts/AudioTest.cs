using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class AudioTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Playing Noise");
            AudioManager.Instance.PlayWalkingSound(1);
        }
    }
}
