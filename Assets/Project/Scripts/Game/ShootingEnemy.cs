using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootingEnemy : Enemy
{
    public float shootingInterval = 4f;
    public float chasingInterval = 2f;
    private Player player;
    private float shootingTimer;
    private float chasingTimer;
    private bool shotByPlayer;
    private NavMeshAgent agent;

    public float shootingDistance = 6f;
    public float chasingDistance = 12f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        shootingTimer = Random.Range(0, shootingInterval);

        agent.SetDestination(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.Killed == true)
        {
            agent.enabled = false;
            this.enabled = false;
            //turn off physics, so enemies fall to ground
            GetComponent<Rigidbody>().isKinematic = true;
        }
        //shooting logic: shoot only when player is in range and when time is up
        shootingTimer -= Time.deltaTime;
        if(shootingTimer <= 0 && Vector3.Distance(transform.position, player.transform.position) <= shootingDistance)
        {
            shootingTimer = shootingInterval;
            GameObject bullet = PoolingManager.Instance.GetBullet(false);
            bullet.transform.position = transform.position;
            bullet.transform.forward = (player.transform.position - transform.position).normalized;
        }

        //chasing logic
        chasingTimer -= Time.deltaTime;
        if(chasingTimer <= 0 && Vector3.Distance(transform.position, player.transform.position) <= chasingDistance)
        {
            chasingTimer = chasingInterval;
            agent.SetDestination(player.transform.position);
        }
    }

    protected override void OnKill()
    {
        base.OnKill();

        agent.enabled = false;
        this.enabled = false;
        transform.localEulerAngles = new Vector3(10, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

}
