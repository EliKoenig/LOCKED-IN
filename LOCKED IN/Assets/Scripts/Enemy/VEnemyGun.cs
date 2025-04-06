using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VEnemyGun : MonoBehaviour
{
    private VStateMachine stateMachine;
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
    public AudioSource source;
    public AudioClip shootClip;
    public GameObject light;

    public Transform model;
    public Animator animator;

    public Vector3 LastKnowPos { get => lastKnowPos; set => lastKnowPos = value; }

    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;

    void Start()
    {
        animator.SetTrigger("Idle");
        stateMachine = GetComponent<VStateMachine>();
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
        health -= damage;
        Debug.Log("Enemy takes " + damage + " damage. Remaining health: " + health);


        if (health <= 0)
        {
            Debug.Log("Enemy has been defeated!");
            Destroy(gameObject);
        }
    }
    public void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
        AnimateEnemy();

        // Rotate the parent (for movement)
        RotateParent();

        // Make sure the child model doesn't rotate (locks X and Z rotation)
        KeepModelUpright();
    }
    private void AnimateEnemy()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float x = localVelocity.x;
        float z = localVelocity.z;

        Debug.Log("x = " + x + ", z = " +  z);

        if (Mathf.Abs(x) < 0.2 && Mathf.Abs(z) < 0.2)
        {
            animator.speed = 1f;
            animator.SetTrigger("Idle");
        }
        else if(x < -3.0f && z > -1.0f)
        {
            animator.speed = 1f;
            animator.SetTrigger("Left");
        }
        else if (x > 3.0f && z < 1.0f)
        {
            animator.speed = 1f;
            animator.SetTrigger("Right");
        }
        else if (Mathf.Abs(x) < 1.0f && z > 3.0f)
        {
            animator.speed = 1f;
            animator.SetTrigger("Walking");
        }
        else if (Mathf.Abs(x) < 1.0f && z >3.0f)
        {
            animator.speed = -1f;
            animator.SetTrigger("Walking");
        }
    }
    void RotateParent()
    {
        if (player != null)
        {
            // Rotate the parent GameObject to face the player on the Y-axis
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.y = 0; // Ignore vertical component to prevent tilting
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
        }
    }
    public void PlayMuzzleFlash()
    {
        StartCoroutine(ShotLight());
    }
    private IEnumerator ShotLight()
    {
        light.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        light.SetActive(false);
        yield break;
    }
    void KeepModelUpright()
    {
        if (model != null)
        {
            // Lock X and Z rotations on the child model (the actual enemy mesh)
            Vector3 modelRotation = model.rotation.eulerAngles;
            model.rotation = Quaternion.Euler(0f, modelRotation.y, 0f); // Keep model upright, only allow Y rotation
        }
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
}
