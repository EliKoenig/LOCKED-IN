using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float time;
    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if(hitTransform.CompareTag("Player"))
        {
            if (hitTransform.GetComponent<Health>() != null)
            {
                hitTransform.GetComponent<Health>().TakeDamage(15);
            }
            else
            {
                hitTransform.GetComponent<VHealth>().TakeDamage(15);
            }
        }
        Destroy(gameObject);    
    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time > 3f)
        {
            Destroy(gameObject);
        }
    }
}
