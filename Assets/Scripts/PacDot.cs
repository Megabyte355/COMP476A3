using UnityEngine;
using System.Collections;

public class PacDot : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "PacMan")
        {
            collider.GetComponent<PacMan>().EatPacDot();
            Destroy (gameObject);
        }
    }
}
