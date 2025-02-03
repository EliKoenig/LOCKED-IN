using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if(hitTransform.CompareTag("Player"))
        {
            hitTransform.GetComponent<Health>().TakeDamage(15);
        }
        Destroy(gameObject);    
    }
}
