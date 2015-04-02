using UnityEngine;
using System.Collections;

public class PacMan : MonoBehaviour
{

    private Vector3 direction = Vector3.zero;
    private AudioSource waka;
    [SerializeField]
    float baseMoveSpeed = 3.5f;

    [SerializeField]
    float boostMoveSpeed = 7.0f;

    void Start()
    {
        waka = GetComponent<AudioSource>();
    }

    void Update ()
    {

        float speed = baseMoveSpeed;

        if(Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector3.forward;
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            direction = -Vector3.forward;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            direction = -Vector3.right;
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector3.right;
        }
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void EatPacDot()
    {
        // Award points

        // Play sound
        waka.Play ();
    }
}
