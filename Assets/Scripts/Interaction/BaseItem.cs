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
        Destroy(gameObject);
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
        ///TODO: temp fix for items not being able to be picked up
        return;
        int lootLayer = gameObject.layer;
        int defaultLayer = LayerMask.NameToLayer("Default");

        Physics2D.IgnoreLayerCollision(lootLayer, defaultLayer, true);
        StartCoroutine(Reenable(lootLayer, defaultLayer, delay));
    }

    private IEnumerator Reenable(int layer1, int layer2, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreLayerCollision(layer1, layer2, false);
    }

}
