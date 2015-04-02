using UnityEngine;
using System.Collections;

public class WarpZone : MonoBehaviour
{
    [SerializeField]
    Transform WarpToLocation;

    void OnTriggerEnter (Collider collider)
    {
        if(collider.tag == "PacMan")
        {
            collider.gameObject.transform.position = WarpToLocation.position;
        }
    }
}
