//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Elevation : MonoBehaviour
//{
//    public Collider2D[] mountainColliders;
//    public Collider2D[] boundaryColliders;

//    private void OnTriggerStay2D(Collider2D collision)
//    {
//        Debug.Log("P Enter");
//        if (collision.gameObject.tag == "Player")
//        {
//            foreach (var mountain in mountainColliders)
//            {
//                mountain.enabled = false;
//            }
//            foreach (var boundary in boundaryColliders)
//            {
//                boundary.enabled = true;
//            }

//            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 15;
//            collision.gameObject.layer = LayerMask.NameToLayer("Player High");
//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        Debug.Log("P Exist");
//        if (collision.gameObject.tag == "Player")
//        {
//            foreach (var mountain in mountainColliders)
//            {
//                mountain.enabled = true;
//            }
//            foreach (var boundary in boundaryColliders)
//            {
//                boundary.enabled = false;
//            }

//            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
//            collision.gameObject.layer = LayerMask.NameToLayer("Player Low");
//        }

//    }

//}
