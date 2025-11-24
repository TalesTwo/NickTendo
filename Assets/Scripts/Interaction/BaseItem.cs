using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : SpawnableObject
{
    // Start is called before the first frame update

    public string Name = "Enter Name here";
    private bool hasFallenInPit = false;
    private bool isShrinking = false;
    private float shrinkTimer = 0f;

    [SerializeField] private float shrinkDuration = 0.5f;

    private Vector3 startScale;
    private Vector3 startPos;
    private Vector3 pitTarget;
    
    // Update is called once per frame
    private void Start()
    {

    }
    private void Update()
    {
        ///TODO: idk why this isnt running
        Debug.Log("Update BaseItem");
        if (!isShrinking)
            return;
        Debug.Log("Shrinking... " + shrinkTimer);

        shrinkTimer += Time.deltaTime;
        float time = Mathf.Clamp01(shrinkTimer / shrinkDuration);

        // Move and shrink simultaneously
        transform.position = Vector3.Lerp(startPos, pitTarget, time);
        transform.localScale = Vector3.Lerp(startScale, Vector3.zero, time);

        if (time >= 1f)
        {
            isShrinking = false;
            transform.localScale = Vector3.zero;
            transform.position = pitTarget;
            Destroy(gameObject);
        }
    }

    public void FellInPit(Vector3 pitCenter)
    {
        //Destroy(gameObject); ops im calling this twice
        if (hasFallenInPit)
            return;
        Debug.Log("Item " + Name + " has fallen in pit.");
        hasFallenInPit = true;
        isShrinking = true;
        shrinkTimer = 0f;
        startScale = transform.localScale;
        startPos = transform.position;
        pitTarget = pitCenter;
    }
    public void TemporarilyDisableCollision(float delay)
    {
        StartCoroutine(DisableTriggerForDelay(delay));
    }

    private void OnEnable()
    {
        // when this item gets enabled, we want to make sure its collider is enabled (incase we left and returned)
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var c in colliders)
        {
            if (c.isTrigger)
            {
                c.enabled = true;
                break;
            }
        }
    }

    private IEnumerator DisableTriggerForDelay(float delay)
    {
        // Find the trigger collider (since we will have two
        Collider2D[] colliders = GetComponents<Collider2D>();
        Collider2D triggerCol = null;

        foreach (var c in colliders)
        {
            if (c.isTrigger)
            {
                triggerCol = c;
                break;
            }
        }

        if (triggerCol == null) {yield break;}

        triggerCol.enabled = false;
        Debug.Log("Temporarily disabling item collider for " + delay + " seconds.");
        yield return new WaitForSeconds(delay);
        Debug.Log("Re-enabling item collider.");
        triggerCol.enabled = true;
    }



}
