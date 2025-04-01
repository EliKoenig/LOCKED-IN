using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;

public class AMeleeEnemy : MonoBehaviour
{
    public float attackRange = 2f;  // Distance at which the enemy can "attack" the player
    public float attackCooldown = 1.5f;  // Time between attacks
    public int attackDamage = 20;  // Damage dealt per attack

    public int health = 60;
    private NavMeshAgent agent;
    private GameObject target;
    private Health targetHealth;
    private bool isAttacking;
    private Animator animator;
    public AudioSource source;
    public AudioClip hit, enemyDie, shoot;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();

        // Access the player's Health component
        targetHealth = target.GetComponent<Health>();
        isAttacking = false;
    }

    private void Update()
    {
        GoToTarget();

        // Check if the enemy is close enough to attack
        if (!isAttacking && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private void GoToTarget()
    {
        agent.SetDestination(target.transform.position);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // Play "Stab Anim" animation
        animator.Play("Stab Anim");

        // Deal damage to the player
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy hits the player for " + attackDamage + " damage!");
        }

        // Return to idle state
        animator.Play("New State");

        // Wait for cooldown before next attack
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    // Method to handle damage from bullets
    public void TakeDamage(int damage)
    {

        health -= damage;
        Debug.Log("Enemy takes " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            
            Debug.Log("Enemy has been defeated!");
            StartCoroutine(Die());
        }
        else
        {
            source.PlayOneShot(hit);
        }
    }
    private IEnumerator Die()
    {
        // Disable enemy behavior
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false; // Stops movement
        this.enabled = false; // Disables this script

        // Hide the enemy
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        // Play death sound
        source.PlayOneShot(enemyDie);

        // Wait for the sound to finish playing
        yield return new WaitForSeconds(enemyDie.length);

        // Destroy the enemy after the sound finishes
        Destroy(gameObject);
    }
}