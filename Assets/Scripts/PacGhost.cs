using UnityEngine;
using System.Collections;

public class PacGhost : MonoBehaviour
{
    private Vector3 direction = Vector3.zero;

    [SerializeField]
    Transform model;

    void Start()
    {
        model.renderer.material.color = Color.red;
    }

    void Update()
    {

    }
}
