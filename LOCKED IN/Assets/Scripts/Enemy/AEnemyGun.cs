using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AEnemyGun : MonoBehaviour
{
    private AStateMachine stateMachine;
    private NavMeshAgent agent;

    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }
    public List<Transform> waypoints = new List<Transform>();
    //debugging
    [SerializeField]
    private string currentState;
    public Path path;
    private GameObject player;
    private Vector3 lastKnowPos;
    public float sightDistance = 20f;
    public float fov = 85f;
    public int health = 80;
    public float accuracy;
    public float eyeHeight;
    public AudioSource source, hurtSource;
    public AudioClip shootClip, hitClip, deathClip;
    public Vector3 LastKnowPos { get => lastKnowPos; set => lastKnowPos = value; }

    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;

    void Start()
    {
        stateMachine = GetComponent<AStateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<Path>();

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
    public void TakeDamage(int damage)
    {
        Debug.Log("Playing hitClip sound!");
        hurtSource.PlayOneShot(hitClip);
        health -= damage;
        Debug.Log("Enemy takes " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            Debug.Log("Enemy has been defeated!");
            StartCoroutine(Die());
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
                Vector3 targetDir = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDir, transform.forward);
                if (angleToPlayer >= -fov && angleToPlayer <= fov)
                {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDir);
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
    private IEnumerator Die()
    {
        // Disable enemy behavior
        GetComponent<NavMeshAgent>().enabled = false; // Stops movement
        stateMachine.enabled = false; // Stops AI logic
        this.enabled = false; // Disables this script

        // Hide the enemy
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        // Play death sound
        source.PlayOneShot(deathClip);

        // Wait for the sound to finish playing
        yield return new WaitForSeconds(deathClip.length);

        // Destroy the enemy after the sound finishes
        Destroy(gameObject);
    }

}
