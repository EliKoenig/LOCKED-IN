using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGun : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;

    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player;  }
    public List<Transform> waypoints = new List<Transform>();
    //debugging
    [SerializeField]
    private string currentState;
    public Path path;
    private GameObject player;
    public float sightDistance = 20f;
    public float fov = 85f;
    public float eyeHeight;

    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");

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
    public void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }
    public bool CanSeePlayer()
    {
        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < sightDistance) 
            {
                Vector3 targetDir = player.transform.position - transform.position - (Vector3.up*eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDir, transform.forward);
                if (angleToPlayer >= -fov && angleToPlayer <= fov) 
                { 
                    Ray ray = new Ray(transform.position+(Vector3.up * eyeHeight), targetDir);
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(ray, out hitInfo, sightDistance))
                    {
                        if (hitInfo.transform.gameObject == player)
                        {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                            return true;
                        }
                    }
                    
                }
            }
        }
        return false;
    }
}
