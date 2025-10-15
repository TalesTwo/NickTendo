using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPlayer : AnimatedEntity
{
    [Header("Animation Cycles")]
    public List<Sprite> idleAnimation;
    public List<Sprite> attackAnimation;
    public List<Sprite> dashAnimation;
    public List<Sprite> walkAnimation;
    public List<Sprite> runAnimation;
    public List<Sprite> hurtAnimation;
    
    // Start is called before the first frame update
    private void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    private void Update()
    {
        AnimationUpdate();
    }

    public void SetAttacking()
    {
        Interrupt(attackAnimation);
    }

    public void SetHurting()
    {
        Interrupt(hurtAnimation);
    }

    public void SetRunning()
    {
        DefaultAnimationCycle = runAnimation;
    }

    public void SetStill()
    {
        DefaultAnimationCycle = idleAnimation;
    }

    public void SetDashing()
    {
        Interrupt(dashAnimation);
    }
}
