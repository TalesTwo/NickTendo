using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedFollowerEnemy : AnimatedEntity
{
    // Start is called before the first frame update

    private int currentindex = 0;
    private bool hasstepped = false;
    public List<int> footstepFrames;
    void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();

        if (index != currentindex)
        {
            currentindex = index;
            hasstepped = false;
        }
        if (footstepFrames.Contains(index) && !hasstepped)
        {
            Managers.AudioManager.Instance.PlayFollowMovementSound(1, 0.1f);
            hasstepped = true;
        }
    }
}
