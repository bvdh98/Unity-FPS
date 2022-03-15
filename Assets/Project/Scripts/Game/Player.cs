using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Visuals")]
    public Camera playerCamera;

    [Header("Gameplay")]
    public int initialHealth = 100;
    public int initialAmmo = 24;
    public float knockbackForce = 10;
    public float hurtDuration = 0.5f;

    private int health;
    public int Health { get { return health; } }

    private int ammo;
    public int Ammo { get { return ammo; } }

    private bool killed;
    public bool Killed { get { return killed; } }

    private bool isHurt;
    void Start()
    {
        health = initialHealth;
        ammo = initialAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(ammo > 0 && killed == false)
            {
                ammo--;
                GameObject bulletObject = PoolingManager.Instance.GetBullet(true);
                bulletObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward;
                bulletObject.transform.forward = playerCamera.transform.forward;
            }
        }
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.GetComponent<AmmoCrate>() != null)
        {
            AmmoCrate ammoCrate = otherCollider.GetComponent<AmmoCrate>();
            ammo += ammoCrate.ammo;

            Destroy(ammoCrate.gameObject);
        }

        else if (otherCollider.GetComponent<HealthCrate>() != null)
        {
            HealthCrate healthCrate = otherCollider.GetComponent<HealthCrate>();
            health += healthCrate.health;

            Destroy(healthCrate.gameObject);
        }
        //if not currently hurt then can become hurt
        if (isHurt == false)
        {
            GameObject hazard = null;
            if (otherCollider.GetComponent<Enemy>() != null)
            {
                Enemy enemy = otherCollider.GetComponent<Enemy>();
                //if enemy is dead disable abillity to hurt player
                if(enemy.Killed == false)
                {
                hazard = enemy.gameObject;
                health -= enemy.damage;
                }
            }
            //check for enemy bullets
            else if (otherCollider.GetComponent<Bullet>() != null)
            {
                Bullet bullet = otherCollider.GetComponent<Bullet>();
                if(bullet.ShotByPlayer == false)
                {
                    hazard = bullet.gameObject;
                    health -= bullet.damage;
                    bullet.gameObject.SetActive(false);
                }
            }

            if(hazard != null)
            {
                isHurt = true;

                Vector3 hurtDirection = (transform.position - hazard.transform.position).normalized;
                Vector3 knockbackDirection = (hurtDirection + Vector3.up).normalized;
                GetComponent<ForceReceiver>().AddForce(knockbackDirection, knockbackForce);

                StartCoroutine(HurtRoutine());
            }

            if (health <= 0)
            {
                if(killed == false)
                {
                    killed = true;
                    OnKill();
                }
            }
        }
    }

    //used to make player hurt for period of time
    IEnumerator HurtRoutine()
    {
        yield return new WaitForSeconds(hurtDuration);

        isHurt = false;
    }

    private void OnKill()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
    }
}
