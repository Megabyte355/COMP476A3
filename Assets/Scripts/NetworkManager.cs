using UnityEngine;
using System.Collections;

// Taken from Lab 7 tutorials
public class NetworkManager : MonoBehaviour
{
    private string ipAddress;
    private string portNumber_string;
    private int portNumber;
    private int playerCount = 0;
    private const string typeName = "COMP476 Network";
    private const string gameName = "Pacman Game";
    
    public GameObject pacManPrefab;
    public Transform spawnLocation;
    
    void Start ()
    {
        ipAddress = "Enter IP Address";
        portNumber_string = "35000";
    }
    
    void OnGUI ()
    {
        if (!Network.isClient && !Network.isServer) {
            ipAddress = GUI.TextField (new Rect (100, 150, 250, 30), ipAddress);
            portNumber_string = GUI.TextField (new Rect (100, 200, 250, 30), portNumber_string);

            if (GUI.Button (new Rect (100, 100, 250, 30), "Start Server")) {
                StartLocalServer ();
            }
                
            if (GUI.Button (new Rect (100, 250, 250, 30), "Join")) {
                JoinIP (ipAddress, portNumber_string);
            }
        }
    }
    
    public void StartLocalServer ()
    {
        Debug.Log ("Local Server created.");
        int portNumber = 35000;
        if(int.TryParse(portNumber_string, out portNumber)) {
            Network.InitializeServer (1, portNumber, !Network.HavePublicAddress ());
        }
    }
    
    void OnServerInitialized ()
    {
        SpawnPlayer ();
    }
    
    public void JoinIP (string ip, string port)
    {
        portNumber = int.Parse (portNumber_string);
        Network.Connect (ip, portNumber);
    }
    
    void OnConnectedToServer ()
    {
        Debug.Log ("Connected to server.");
        SpawnPlayer ();
    }
    
    void OnLevelWasLoaded (int level)
    {
        Debug.Log ("Level done loading. Spawning player");
    }
    
    private void SpawnPlayer ()
    {
        Network.Instantiate (pacManPrefab, spawnLocation.position, Quaternion.identity, 0);
    }
    
    void OnPlayerConnected (NetworkPlayer player)
    {
        Debug.Log ("Player " + (++playerCount) + " connected from " + player.ipAddress + ":" + player.port);
    }
    
    void OnPlayerDisconnected (NetworkPlayer player)
    {
        Network.RemoveRPCs (player);
        Network.DestroyPlayerObjects (player);
    }
    
    void OnDisconnectedFromServer (NetworkDisconnection info)
    {
        if (Network.isServer) {
            Debug.Log ("Local server connection lost");
        } else {
            if (info == NetworkDisconnection.LostConnection) {
                Debug.Log ("Lost connection to the server");
            } else {
                Debug.Log ("Successfully disconnected from the server");
            }
        }
    }
}
