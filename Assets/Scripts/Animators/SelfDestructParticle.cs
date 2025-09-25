using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructParticle : MonoBehaviour
{
    public float lifetime = 0.4f;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(SelfDestruct), lifetime);
    }

    // Update is called once per frame
    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
