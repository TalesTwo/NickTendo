using System;
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
    public List<Sprite> toIdleAnimation;
    
    private bool _isRunning = false;
    private bool _isAttacking = false;

    protected override void ResetBools()
    {
        if (_isAttacking && !_isRunning)
        {
            SetToIdle();
        }
        _isAttacking = false;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        AnimationSetup();
        EventBroadcaster.PlayerDeath += SetDead;
    }

    // Update is called once per frame
    private void Update()
    {
        AnimationUpdate();
    }

    public void SetAttacking()
    {
        _isAttacking = true;
        Interrupt(attackAnimation);
    }

    public void SetHurting()
    {
        Interrupt(hurtAnimation);
    }

    public void SetRunning()
    {
        _isRunning = true;
        DefaultAnimationCycle = runAnimation;
    }

    public void SetStill()
    {
        if (_isRunning && !_isAttacking)
        {
            SetToIdle();
            _isRunning = false;
        }
        DefaultAnimationCycle = idleAnimation;
    }

    public void SetDashing()
    {
        //Interrupt(dashAnimation);
        DefaultAnimationCycle = dashAnimation;
    }

    public void SetDashAngle(Quaternion rotation)
    {
        Vector3 angles = rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(angles.x, angles.y, angles.z-90f);
    }

    public void ResetDashAngle()
    {
        transform.rotation = Quaternion.identity;
    }

    private void SetToIdle()
    {
        Interrupt(toIdleAnimation);
    }

    private void SetDead()
    {
        DefaultAnimationCycle = hurtAnimation;
    }
}
