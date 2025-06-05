using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevation_Exit : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (var mountain in mountainColliders)
            {
                mountain.enabled = true;
            }
            foreach (var boundary in boundaryColliders)
            {
                boundary.enabled = false;
            }
//thoat ra se sort lai order
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
            collision.gameObject.layer = LayerMask.NameToLayer("Player Low");
        }

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
    }
}
