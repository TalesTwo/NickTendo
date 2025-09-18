using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private PlayerController playerController;
    private AnimatedPlayerAttack animation;
    public GameObject attackAnimation;
    
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animation = attackAnimation.GetComponent<AnimatedPlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = PlayerController.directionAngle;
        
        //attackAnimation.transform.RotateAround(transform.position, new Vector3(0, 0, 1), angle);
        /*
            Vector2 pivotPoint = transform.position;
            Vector2 direction = (Vector2)attackAnimation.transform.position - pivotPoint;

            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 rotatedDirection = rotation * direction;

            attackAnimation.transform.position = pivotPoint + rotatedDirection;
            attackAnimation.transform.rotation = rotation;
        */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        attackAnimation.SetActive(true);
    }
}
