using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInteractButton : MonoBehaviour
{
    public GameObject InteractSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            InteractSprite.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            InteractSprite.SetActive(false);
        }
    }
}
