using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class VAttackState : VBaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float losingPlayerTimer;
    private float shotTimer;

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {

            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            shotTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);
            if (enemy.CanSeePlayer())
            {
                losingPlayerTimer = 0;
                if (shotTimer > enemy.fireRate)
                {
                    enemy.PlayMuzzleFlash();
                    Shoot();
                }
                if (moveTimer > Random.Range(3, 7))
                {
                    enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                    moveTimer = 0;
                }
            }
            enemy.LastKnowPos = enemy.Player.transform.position;
        }
        else
        {
            losingPlayerTimer += Time.deltaTime;
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > .5)
            {
                stateMachine.ChangeState(new VSearchState());
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Shoot()
    {
        enemy.source.PlayOneShot(enemy.shootClip);
        Transform gunBarrel = enemy.gunBarrel;
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/EnemyBullet") as GameObject, gunBarrel.position, enemy.transform.rotation);
        Vector3 shootDir = (enemy.Player.transform.position - gunBarrel.position).normalized;
        bullet.GetComponent<Rigidbody>().linearVelocity = Quaternion.AngleAxis(Random.Range(-enemy.accuracy, enemy.accuracy), Vector3.up) * shootDir * 60;

        shotTimer = 0;
    }

}
