using UnityEngine;
using System.Collections;

public class PacMan : MonoBehaviour
{

    private Vector3 direction = Vector3.zero;
    private AudioSource wakaSound;
    private AudioSource powerUpSound;
    private Score score;

    [SerializeField]
    float baseMoveSpeed = 3.5f;
    [SerializeField]
    float boostMoveSpeed = 7.0f;
    [SerializeField]
    float boostDuration = 10.0f;
    float timer;

    void Awake()
    {
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<Score>();

        AudioSource[] audio = GetComponents<AudioSource>();
        wakaSound = audio[0];
        powerUpSound = audio[1];

        if(!networkView.isMine)
        {
            GetComponent<AudioListener>().enabled = false;
        }
    }

    void Update ()
    {
        if(!networkView.isMine)
        {
            return;
        }

        float speed;
        if(timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            speed = boostMoveSpeed;
        }
        else
        {
            speed = baseMoveSpeed;
        }

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

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 networkPosition = Vector3.zero;

        if(stream.isWriting)
        {
            networkPosition = transform.position;
            stream.Serialize(ref networkPosition);
        }
        if(stream.isReading)
        {
            stream.Serialize(ref networkPosition);
            transform.position = networkPosition;
        }
    }

    void BoostSpeed()
    {
        timer = boostDuration;
    }

    public void EatPacDot()
    {
        // Award points
        if(networkView.isMine)
        {
            score.IncrementScore();
        }
        else
        {
            score.IncrementOpponentScore();
        }

        // Play sound
        wakaSound.Play ();
    }

    public void EatSuperDot()
    {
        // Award points
        if(networkView.isMine)
        {
            score.IncrementScore();
            BoostSpeed();
        }
        else
        {
            score.IncrementOpponentScore();
        }
        
        // Play sound
        powerUpSound.Play();
    }
}
