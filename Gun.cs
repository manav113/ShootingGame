using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Gun : MonoBehaviour {

    public GameObject end, start; // The gun start and end point
    public GameObject gun;
    public GameObject gun2;

    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;

    public GameObject shotSound;
    public GameObject muzzlePrefab;
    public GameObject ammo;

    public GameObject bulletHole;

    float gunShotTime = 0.1f;
    float gunReloadTime = 1.0f;
    Quaternion previousRotation;
    public float health = 100;
    public bool isDead;

    public GameObject ui;

    public Text health_string;
    public Text remainingHealth;
    public int old = 0;
    public Text magBullets;
    public Text remainingBullets;

    public int pull_out = 0;
    bool ammo_taken = false;
    float dist_ammo;
    int magBulletsVal = 30;
    int remainingBulletsVal = 90;
    int magSize = 30;
    public GameObject headMesh;
    public int damage = 100;

    public static bool leftHanded { get; private set; }

    public void restart()
    {
        SceneManager.LoadScene("SampleScene");

    }


    // Use this for initialization
    void Start() {
        headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
    }
    void check_distance_ammo()
    {
        dist_ammo = Vector3.Distance(this.transform.position, ammo.transform.position);
        if (dist_ammo < 1.8f)
        {
            GetComponent<Animator>().SetTrigger("ammo");

            print("Took ammo");
            remainingBulletsVal = remainingBulletsVal + 30;
            ammo_taken = true;


        }
        dist_ammo = 0f;

    }
    void gun_switcher()
    {
        gun.SetActive(false);

        gun2.SetActive(true);
        gun2.transform.position = gun.transform.position;

    }
    // Update is called once per frame
    void Update() {
        if(pull_out == 1)
        {
            GetComponent<Animator>().SetTrigger("weapon_change_continue");

            pull_out = 0;
            Invoke("gun_switcher", 1f);
            
        }
        if (Input.GetKeyDown("space"))
        {if (old == 0)
            {
                GetComponent<Animator>().SetTrigger("weapon_change");

                //gun2.SetActive(true);
//                gun2.transform.position = gun.transform.position;
                //gun.SetActive(false);

                old = 1;
                pull_out = 1;

            }
            else
            {
                gun.SetActive(true);
                gun2.SetActive(false);
                old = 0;
            }
        }







        // Cool down times
    if (ammo_taken == false)
        check_distance_ammo();




        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }


        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead)
        { 
            shotDetection(); // Should be completed

            addEffects(); // Should be completed

            animator.SetBool("fire", true);
            gunShotTime = 0.5f;
            
            // Instantiating the muzzle prefab and shot sound
            
            magBulletsVal = magBulletsVal - 1;
            if (magBulletsVal <= 0 && remainingBulletsVal > 0)
            {
                animator.SetBool("reloadAfterFire", true);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.5f);
            }
        }
        else
        {
            animator.SetBool("fire", false);
        }

        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.R)) && gunReloadTime <= 0.0f && gunShotTime <= 0.1f && remainingBulletsVal > 0 && magBulletsVal < magSize && !isDead )
        {
            animator.SetBool("reload", true);
            gunReloadTime = 2.5f;
            Invoke("reloaded", 2.0f);
        }
        else
        {
            animator.SetBool("reload", false);
        }
        updateText();
       updateHealth();
       
    }



    public void Being_shot(float damage) // getting hit from enemy
    {
        health = health - damage;
        if (health <= 0)
        {
            print("Player Dead :( ");
            isDead = true;
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
            GetComponent<Animator>().SetBool("dead", true);
            GetComponent<CharacterMovement>().isDead = true;
            GetComponent<CharacterController>().enabled = false;
            Invoke("restart",10);
        }
    }
    public void ReloadEvent(int eventNumber) // appearing and disappearing the handMag and gunMag
    {
        if (eventNumber == 1)
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = false;

        }
        if (eventNumber == 2)
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = true;

        }
    }

    void reloaded()
    {
        int newMagBulletsVal = Mathf.Min(remainingBulletsVal + magBulletsVal, magSize);
        int addedBullets = newMagBulletsVal - magBulletsVal;
        magBulletsVal = newMagBulletsVal;
        remainingBulletsVal = Mathf.Max(0, remainingBulletsVal - addedBullets);
        animator.SetBool("reloadAfterFire", false);
    }

    void updateHealth()
    {

        health_string.text = health.ToString();

    }
    void updateText()
    {
        magBullets.text = magBulletsVal.ToString() ;
        remainingBullets.text = remainingBulletsVal.ToString();
    }

    void shotDetection() // Detecting the object which player shot 
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit rayHit;
        //RaycastHit[] hits;
        //hits = Physics.RaycastAll(end.transform.position, (end.transform.position - start.transform.position), 100.0f);
        //for (int i = 0; i < hits.Length; i++)
        //{
         //   RaycastHit hit = hits[i];
          //  print(hit.transform.tag);
//        }

            if (Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position), out rayHit, 100.0f, layerMask))
        {
            damage = 30;
            //print(rayHit.collider);
            //print(rayHit.transform.tag);

            if (rayHit.transform.tag == "head")
            {
                damage = 100;
                print("Hit Head");
            }
            if (rayHit.transform.tag == "hands")
            {
                damage = 10;
                print("hit Hand");
            }
            if (rayHit.transform.tag == "legs")
            {
                damage = 20;
                print("hit legs");
            }

            
            if (rayHit.transform.tag == "enemy")
            {
                print("HIT Enemy");
                rayHit.transform.GetComponent<Enemy>().Being_shot(damage);
            }
            else

            {
                Destroy(Instantiate(bulletHole, rayHit.point+ rayHit.collider.transform.up*0.01f, rayHit.collider.transform.rotation), 1.0f);

            }

            }

        
    }

    

    void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    {
        Destroy(Instantiate(shotSound, transform.position, transform.rotation), 2.0f);

        GameObject tempMuzzle = Instantiate(muzzlePrefab, end.transform.position, end.transform.rotation);
        tempMuzzle.GetComponent<ParticleSystem>().Play();

        Destroy(tempMuzzle, 2.0f);

    }

}
