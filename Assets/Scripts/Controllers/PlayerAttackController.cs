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
