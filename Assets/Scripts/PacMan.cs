using UnityEngine;
using System.Collections;

public class PacMan : MonoBehaviour
{

    private Vector3 direction = Vector3.zero;
    private AudioSource wakaSound;
    private AudioSource powerUpSound;
    private AudioSource deathSound;
    private Score score;
    private Transform model;
    private GameObject spawnPoint;

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
        model = transform.FindChild("PacManModel");
        spawnPoint = GameObject.Find ("SpawnPoint");

        AudioSource[] audio = GetComponents<AudioSource>();
        wakaSound = audio[0];
        powerUpSound = audio[1];
        deathSound = audio[2];

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

        if(direction != Vector3.zero)
        {
            ChangeOrientation(transform.position + direction);
            transform.Translate(direction * speed * Time.deltaTime);
        }
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

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "PacGhost")
        {
            deathSound.Play ();
            TeleportToSpawnPoint();
        }
    }

    void BoostSpeed()
    {
        timer = boostDuration;
    }

    void ChangeOrientation(Vector3 lookAtPoint)
    {
        model.transform.LookAt(lookAtPoint);
        networkView.RPC("ChangePacManOrientationNetwork", RPCMode.OthersBuffered, lookAtPoint);
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

    void TeleportToSpawnPoint()
    {
        transform.position = spawnPoint.transform.position;
        networkView.RPC("TeleportToSpawnPointNetwork", RPCMode.OthersBuffered);
    }

    [RPC]
    void TeleportToSpawnPointNetwork()
    {
        transform.position = spawnPoint.transform.position;
    }

    [RPC]
    void ChangePacManOrientationNetwork(Vector3 lookAtPoint)
    {
        model.transform.LookAt(lookAtPoint);
    }
}
