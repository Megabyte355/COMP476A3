using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
    int myScore = 0;
    int otherScore = 0;
    int totalPacDots = 0;

    void Start ()
    {
        totalPacDots = GameObject.Find("PacDots").transform.childCount;
    }

    void OnGUI ()
    {
        if (Network.isClient || Network.isServer) {


//            ipAddress = GUI.TextField (new Rect (100, 150, 250, 30), ipAddress);
//            portNumber_string = GUI.TextField (new Rect (100, 200, 250, 30), portNumber_string);
//            
//            if (GUI.Button (new Rect (100, 100, 250, 30), "Start Server")) {
//                StartLocalServer ();
//            }
//            
//            if (GUI.Button (new Rect (100, 250, 250, 30), "Join")) {
//                JoinIP (ipAddress, portNumber_string);
//            }


            GUI.Box(new Rect(10, 10, 275, 50), "Score");
            GUI.Box(new Rect(10, 30, 275, 30), myScore.ToString());
//            GUI.Box(new Rect(10, 60, 275, 30), "Opponent's score" + otherScore);
            GUI.Box(new Rect(10, 80, 275, 50), "Opponent's score");
            GUI.Box(new Rect(10, 100, 275, 30), otherScore.ToString());
//            GUI.Box(new Rect(10, 150, 275, 30), "Visited node -> Yellow, Path node -> Green");   

            GUI.Box(new Rect(10, 150, 275, 50), "Remaining Pac-Dots");
            GUI.Box(new Rect(10, 170, 275, 30), totalPacDots.ToString());
        }
    }

    public void IncrementScore()
    {
        myScore++;
        totalPacDots--;
    }

    public void IncrementOpponentScore()
    {
        otherScore++;
        totalPacDots--;
    }

    void Update()
    {

    }
}
