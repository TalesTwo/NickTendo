using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedHitParticle : AnimatedEntity
{
    private bool _selfDestruct = false;
    
    // Start is called before the first frame update
    void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
        if (index == DefaultAnimationCycle.Count - 1)
        {
            _selfDestruct = true;
        }

        if (_selfDestruct)
        {
            Destroy(gameObject);
        }
    }
}
