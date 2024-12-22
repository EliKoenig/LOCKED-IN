using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
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
            Destroy(gameObject);
        }
    }
}