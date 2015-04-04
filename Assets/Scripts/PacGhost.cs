using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacGhost : MonoBehaviour
{
    private Vector3 direction = Vector3.zero;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    Transform model;

    [SerializeField]
    float decisionTimer = 0.5f;
    float timer = 0.0f;

    void Update()
    {
        if(!Network.isServer)
        {
            return;
        }

        timer -= Time.deltaTime;
        if(timer <= 0.0f)
        {
            timer = decisionTimer;
            float minDistance = float.PositiveInfinity;
            Transform currentTarget = null;
            GameObject[] targets = GameObject.FindGameObjectsWithTag("PacMan");

            // Find closest target
            for(int i = 0; i < targets.Length; i++)
            {
                float distance = (targets[i].transform.position - transform.position).magnitude;
                
                if(distance < minDistance)
                {
                    currentTarget = targets[i].transform;
                    minDistance = distance;
                }
            }
            
            // Raycast to see available moves
            List<Vector3> possibleMoves = new List<Vector3>();
            if(!Physics.Raycast(transform.position, Vector3.forward, 1.0f, 1 << 9))
            {
                possibleMoves.Add (transform.position + Vector3.forward);
            }
            if(!Physics.Raycast(transform.position, -Vector3.forward, 1.0f, 1 << 9))
            {
                possibleMoves.Add (transform.position - Vector3.forward);
            }
            if(!Physics.Raycast(transform.position, -Vector3.right, 1.0f, 1 << 9))
            {
                possibleMoves.Add (transform.position - Vector3.right);
            }
            if(!Physics.Raycast(transform.position, Vector3.right, 1.0f, 1 << 9))
            {
                possibleMoves.Add (transform.position + Vector3.right);
            }
            
            // Check which move is best
            Vector3 bestMove = Vector3.zero;
            float bestMoveDistance = float.PositiveInfinity;
            foreach(Vector3 move in possibleMoves)
            {
                float distance = (currentTarget.position - move).magnitude;
                if(distance < bestMoveDistance)
                {
                    bestMoveDistance = distance;
                    bestMove = move;
                }
            }
            
            // Set direction based on best move
            direction = (bestMove - transform.position).normalized;
            ChangeOrientation(transform.position + direction);
        }
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
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

    public void SetColor(Color newColor)
    {
        model.renderer.material.color = newColor;

        // Set color for others player
        Vector3 colorVector = new Vector3(newColor.r, newColor.g, newColor.b);
        networkView.RPC("SetColorNetwork", RPCMode.OthersBuffered, colorVector);
    }

    void ChangeOrientation(Vector3 lookAtPoint)
    {
        transform.LookAt(lookAtPoint);
        networkView.RPC("ChangeOrientationNetwork", RPCMode.OthersBuffered, lookAtPoint);
    }

    [RPC]
    void SetColorNetwork(Vector3 colorVector)
    {
        model.renderer.material.color = new Color(colorVector.x, colorVector.y, colorVector.z);
    }

    [RPC]
    void ChangeOrientationNetwork(Vector3 lookAtPoint)
    {
        transform.LookAt(lookAtPoint);
    }
}
