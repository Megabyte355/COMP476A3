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
    float boostTimer;
    [SerializeField]
    float respawnDuration = 3.0f;
    float respawnTimer;

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

        if(respawnTimer >= 0.0f)
        {
            respawnTimer -= Time.deltaTime;

            if(respawnTimer < 0.0f)
            {
                networkView.RPC("Respawn", RPCMode.AllBuffered);
            }
            else
            {
                return;
            }
        }

        float speed;
        if(boostTimer >= 0.0f)
        {
            boostTimer -= Time.deltaTime;
            speed = boostMoveSpeed;
        }
        else
        {
            speed = baseMoveSpeed;
        }

        if(Input.GetKey(KeyCode.W) && !Physics.Raycast(transform.position, Vector3.forward, 0.75f, 1 << 9))
        {
            direction = Vector3.forward;
        }
        else if(Input.GetKey(KeyCode.S) && !Physics.Raycast(transform.position, -Vector3.forward, 0.75f, 1 << 9))
        {
            direction = -Vector3.forward;
        }
        else if(Input.GetKey(KeyCode.A) && !Physics.Raycast(transform.position, -Vector3.right, 0.75f, 1 << 9))
        {
            direction = -Vector3.right;
        }
        else if(Input.GetKey(KeyCode.D) && !Physics.Raycast(transform.position, Vector3.right, 0.75f, 1 << 9))
        {
            direction = Vector3.right;
        }

        if(direction != Vector3.zero)
        {
            ChangeOrientation(transform.position + direction);
            if(!Physics.Raycast(transform.position, direction, 0.55f, 1 << 9))
            {
                transform.Translate(direction * speed * Time.deltaTime);
            }
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
            if(networkView.isMine)
            {
                networkView.RPC("ActivateRespawnTimer", RPCMode.AllBuffered);
            }
        }
    }

    void BoostSpeed()
    {
        boostTimer = boostDuration;
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

    [RPC]
    void ActivateRespawnTimer()
    {
        GetComponent<SphereCollider>().enabled = false;
        transform.FindChild("PacManModel").gameObject.SetActive(false);
        respawnTimer = respawnDuration;
    }

    [RPC]
    void Respawn()
    {
        transform.position = spawnPoint.transform.position;
        GetComponent<SphereCollider>().enabled = true;
        transform.FindChild("PacManModel").gameObject.SetActive(true);
        direction = Vector3.zero;
    }

    [RPC]
    void ChangePacManOrientationNetwork(Vector3 lookAtPoint)
    {
        model.transform.LookAt(lookAtPoint);
    }
}
