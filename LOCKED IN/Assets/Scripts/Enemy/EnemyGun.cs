using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGun : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;

    public NavMeshAgent Agent { get => agent; }
    public List<Transform> waypoints = new List<Transform>();
    //debugging
    [SerializeField]
    private string currentState;
    public Path path;

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();

        if (path.possibleWaypoints.Count < 4)
        {
            Debug.LogError("Not enough waypoints in the possibleWaypoints list!");
            return;
        }

        // Create a temporary list to avoid modifying the original possibleWaypoints list
        List<Transform> tempWaypoints = new List<Transform>(path.possibleWaypoints);

        // Randomly pick 4 waypoints
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, tempWaypoints.Count);
            waypoints.Add(tempWaypoints[randomIndex]);
            tempWaypoints.RemoveAt(randomIndex); // Remove the chosen waypoint to avoid duplicates
        }
    }
}
