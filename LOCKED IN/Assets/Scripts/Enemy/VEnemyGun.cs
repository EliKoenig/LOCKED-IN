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
    public AnimationClip deathAnimation;

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
        agent.updateRotation = false;
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<Path>();

        if (path.possibleWaypoints.Count < 4)
        {
            Debug.LogError("Not enough waypoints in the possibleWaypoints list!");
            return;
        }

        List<Transform> tempWaypoints = new List<Transform>(path.possibleWaypoints);

        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, tempWaypoints.Count);
            waypoints.Add(tempWaypoints[randomIndex]);
            tempWaypoints.RemoveAt(randomIndex);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy takes " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            StartCoroutine(EnemyDeath());
            Debug.Log("Enemy has been defeated!");
        }
    }

    private IEnumerator EnemyDeath()
    {
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, 0f);
        model.localRotation = Quaternion.Euler(0f, model.localRotation.eulerAngles.y, 0f);
        animator.speed = 1f;

        animator.SetTrigger("Die");
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        stateMachine.enabled = false;
        this.enabled = false;

        yield return new WaitForSeconds(deathAnimation.length);
        Destroy(gameObject);
    }

    public void Update()
    {
        // Lock GameObject's X and Z rotation to 0


        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, 0f);
        Debug.Log(transform.rotation.x);
        AnimateEnemy();
    }

    private void AnimateEnemy()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float x = localVelocity.x;
        float z = localVelocity.z;

        Debug.Log("x = " + x + ", z = " + z);

        if (Mathf.Abs(x) < 0.2 && Mathf.Abs(z) < 0.2)
        {
            animator.speed = 1f;
            animator.SetTrigger("Idle");
        }
        else if (x < -3.0f && z > -1.0f)
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
        else if (Mathf.Abs(x) < 1.0f && z > 3.0f)
        {
            animator.speed = -1f;
            animator.SetTrigger("Walking");
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
