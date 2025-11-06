using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedRangedEnemy : AnimatedEntity
{
    // Start is called before the first frame update
    //private int currentindex = 0;
    //private bool hasstepped = false;
    //public List<int> footstepFrames;
    void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();

    }
}
