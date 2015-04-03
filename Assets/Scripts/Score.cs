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
            GUI.Box(new Rect(10, 10, 275, 50), "Score");
            GUI.Box(new Rect(10, 30, 275, 30), myScore.ToString());

            GUI.Box(new Rect(10, 80, 275, 50), "Opponent's score");
            GUI.Box(new Rect(10, 100, 275, 30), otherScore.ToString()); 

            if(totalPacDots != 0)
            {
                GUI.Box(new Rect(10, 150, 275, 50), "Remaining Pac-Dots");
                GUI.Box(new Rect(10, 170, 275, 30), totalPacDots.ToString());
            }
            else
            {
                string gameResult = "TIED GAME!";
                if(myScore > otherScore)
                {
                    gameResult = "YOU WIN!";
                }
                else if(myScore < otherScore)
                {
                    gameResult = "YOU LOSE!";
                }
                GUI.Box(new Rect(10, 150, 275, 50), "GAME OVER");
                GUI.Box(new Rect(10, 170, 275, 30), gameResult);
            }
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

}
