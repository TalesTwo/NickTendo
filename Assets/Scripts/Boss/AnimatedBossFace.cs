using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBossFace : AnimatedEntity
{
    public List<Sprite> idleFrames;
    public List<Sprite> exhaustedFrames;
    public List<Sprite> loadingMinionsFrames;
    public List<Sprite> hurtFrames;
    
    // Start is called before the first frame update
    void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }

    public void SetIdleAnimation()
    {
        DefaultAnimationCycle = idleFrames;
    }

    public void SetExhuastedAnimation()
    {
        DefaultAnimationCycle = exhaustedFrames;
    }

    public void SetLoadingMinionsAnimation()
    {
        Interrupt(loadingMinionsFrames);
    }

    public void SetHurtAnimation()
    {
        DefaultAnimationCycle = hurtFrames;
    }
}
