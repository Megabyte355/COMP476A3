using UnityEngine;
using System.Collections;

public class SuperPacDot : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "PacMan")
        {
            collider.GetComponent<PacMan>().EatSuperDot();
            Destroy (gameObject);
        }
    }
}
