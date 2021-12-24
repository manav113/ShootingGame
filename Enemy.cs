using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public GameObject player;
    public GameObject[] targets;
    public GameObject end;
    public GameObject start;
    public GameObject gun;

    public GameObject Hide;

    public int health;
    public bool cover_took = false;
    public bool isDead = false;
    public int current_target = 0; //ranging from 0-3
    public bool found_player = false;

    int upper_limit_for_detect = 8;
    int lower_limit_for_detect = 0;
    int lower_limit_for_following = 5;
    public bool shooted_at_enemy = false;
    public float time_elapsed = 0f;
    public float maxRate = 0.25f;
    public int damage = 100;

    /// <summary>
    /// Initializes player character and makes health = 100
    /// </summary>
    
    void Initialize_Game_Objects()
    {

        if (player == null)
            player = GameObject.FindWithTag("Player");

        health = 100;

    }

    // Start is called before the first frame update
    void Start()
    {

        Initialize_Game_Objects();


    }

    /// <summary>
    /// Returns player's position
    /// </summary>
    ///  <returns>
    /// Vector3 Positon of player
    /// </returns>


    Vector3 player_position()
    {
        return player.transform.position;
    }

    /// <summary>
    /// Returns enemy's position
    /// </summary>
    ///  <returns>
    /// Vector3 Positon of current enemy
    /// </returns>

    Vector3 enemy_position()
    {
        return transform.position;
    }

    /// <summary>
    /// Returns distance between player and enemy
    /// </summary>
    ///  <returns>
    /// float distance between player and enemy
    /// </returns>
    float dist_player_enemy()
    {
        return Vector3.Distance(player_position(), enemy_position());
    }

    /// <summary>
    /// Returns cover postion (Implementation half)y
    /// </summary>
    ///  <returns>
    /// Vector3 cover position
    /// </returns>
    Vector3 cover_position()
    {
        return Hide.transform.position;
    }

    /// <summary>
    /// Returns distance between cover and enemy
    /// </summary>
    ///  <returns>
    /// float distance between cover and enemy
    /// </returns>
    float dist_enemy_cover()
    {
        return Vector3.Distance(cover_position(), enemy_position());
    }

    /// <summary>
    /// Chekcs distance between player and enemy
    /// </summary>
    ///  <returns>
    /// returns True if in between the threshold else returns false
    /// </returns>
    bool detect_player()
    {
        if (dist_player_enemy() <= upper_limit_for_detect && dist_player_enemy() >= lower_limit_for_detect)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Moves towards targets nodes
    /// 1. Find the target node vector
    /// 2 Rotate and use Lerp for smooth transition
    /// 3.Check for distance-> if distance is smaller than limit, change target node
    /// </summary>
    ///  <returns>
    ///     None
    /// </returns>
    void walk_to_targets()
    {
        Vector3 tempPos = new Vector3(targets[current_target].transform.position.x, transform.position.y, targets[current_target].transform.position.z);
        Quaternion desiredRotation = Quaternion.LookRotation(tempPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);
        float dist = Vector3.Distance(tempPos, transform.position);
        if (dist < 0.1f)    
        {
            current_target = (current_target + 1) % 4;
        }
    }

    /// <summary>
    /// Moves towards the player
    /// 1. Checks for the distance between player and enemy. If it lies in the threshold, set fire state to true and run to be false and start firing
    /// 2. If not within threshold, run state is made true while fire is made false
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>
    void move_to_player()
    {
        if (dist_player_enemy() > lower_limit_for_detect && dist_player_enemy() < lower_limit_for_following)
        {
            GetComponent<Animator>().SetBool("fire", true);
            GetComponent<Animator>().SetBool("run", false);
            check_bullet_pace();
        }
        else 
        {

            GetComponent<Animator>().SetBool("run", true);
            GetComponent<Animator>().SetBool("fire", false);
        }
    }

    void hit()
    {
        GetComponent<Animator>().SetBool("fire", true);
        check_bullet_pace();
    }

 
    /// <summary>
    /// Moves towards the cover. Invokes hit function after 3 seconds to start firing
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>
    void move_to_cover()
    {
        if (dist_enemy_cover() > 5f)
        {
            GetComponent<Animator>().SetBool("run", true);
            GetComponent<Animator>().SetBool("hide", true);
        }
        else
        {
            if (dist_enemy_cover() <2.4f)
            {
                if (cover_took == false)
                {

                    GetComponent<Animator>().SetBool("cover", true);
                    GetComponent<Animator>().SetBool("run", false);
                    GetComponent<Animator>().SetBool("hide", false);
                    GetComponent<Animator>().SetBool("fire", false);

                    cover_took = true;
                }

                if (cover_took == true)
                {
                    Invoke("hit", 3);
                }
            }

        }
    }
    /// <summary>
    /// Hides in case the player is near and moves to cover
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>
    void go_to_hide()
    {
        if (cover_took == false)
        {
            transform.LookAt(Hide.transform);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(cover_position() - enemy_position()), Time.deltaTime);
        }
        else {
            transform.LookAt(player.transform);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player_position() - enemy_position()), Time.deltaTime);
        }
            move_to_cover();
        
    }

    /// <summary>
    /// After Death Manipulation
    /// 1. Sets dead variable to true and makes dead state true.
    /// gets deactivated.
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>
    void after_death()
    {
        isDead = true;
        print("Dead!");
        GetComponent<Animator>().SetBool("fire", false);
        GetComponent<Animator>().SetBool("run", false);
        GetComponent<Animator>().SetBool("dead", true);
        make_gun_independent();
        GetComponent<CharacterController>().enabled = false;

    }
    /// <summary>
    /// Gun Independent
    /// 1. The gun is assigned a box collider and rigid body. To make it independent, its parent is set to null. 
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>

    void make_gun_independent()
    {
        gun.transform.parent = null;
        gun.AddComponent<BoxCollider>();
        gun.GetComponent<BoxCollider>().enabled = true;
        gun.AddComponent<Rigidbody>();
        gun.GetComponent<Rigidbody>().isKinematic = false;
    }
    /// <summary>
    /// Health check
    /// 1. If health is 100 and player isn't detected, it sets detected to be true.
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>
    public void Being_shot(int damage) // getting hit from enemy
    {
        if (health == 100)
            shooted_at_enemy = true;
        health = health - damage;

        if (health <= 0)
        {
            after_death();   
        }

    }
    /// <summary>
    /// Check for bullet rate
    /// Can only shoot after timer gets greater than 0.2f. Better to use Invoke(x,1)?
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>
    void check_bullet_pace()
    { 
      if (time_elapsed > maxRate)
        {
            shooting_player();
            time_elapsed = 0f;
        }
        time_elapsed = time_elapsed + Time.deltaTime;
    }

    /// <summary>
    /// RayCast implementation for finding player.. 
    /// Randomizes the up and right vector of the source gun end. Subtracting the end and start gives us vector pointing to the player
    /// Sends RayCast and finds hit using Tag.
    /// Half implemented for head, hands, legs. Finds tag but mostly goes for parent tag
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>

    void shooting_player()
    {
        { 
            RaycastHit rayHit;
            Vector3 rand = end.transform.up * Random.Range(-0.25f, 0.25f) + end.transform.right * Random.Range(-0.15f, 0.15f);
            Vector3 end_rand = end.transform.position + rand;
            Vector3 forward = end_rand - start.transform.position;

            if (Physics.Raycast(end.transform.position, forward, out rayHit, 100.0f))
            {
                damage = 20;
 //               print(rayHit.transform.tag);
 //               print(rayHit.collider);
                if (rayHit.transform.tag == "head")
                {
                    damage = 100;
                    //print("Hit Head");
                }
                if (rayHit.transform.tag == "hands")
                {
                    damage = 20;
                  //  print("hit Hand");
                }
                if (rayHit.transform.tag == "legs")
                {
                    damage = 10;
                 //   print("hit legs");
                }

                if (rayHit.transform.tag == "Player")
                {
                    print("Shot player"); 
                rayHit.transform.GetComponent<Gun>().Being_shot(damage);
                }
            }
        }
    }

    /// <summary>
    /// Updating frame every Time.timedelta.. 
    /// 1. If the player is not found and the enemy is alive, it walks to target nodes.
    /// 2. If player is not found  but distance between enemy and player lies between threshold, player is marked as detected.
    /// 3.If player is not within the distance, but if player hits enemy, player gets detected.
    /// 4. Make first enemy follow while second enemy hide.
    /// 5. Using Lerp and rotation, the enemy moves towards player if distance is large or starts firing if distance is less.
    /// 6. Second enemy instead goes to cover rather than moving closer to player.
    /// </summary>
    ///  <returns>
    /// None
    /// </returns>
    /// 
    // Update is called once per frame
    void Update()

        {

        
        if (found_player == false && isDead == false)
  //         found_player = detect_player();

//        if (found_player == false)
//        {
            walk_to_targets();
        //       }


        if (found_player == false && dist_player_enemy() < 20f && isDead == false && detect_player() == true)
            found_player = true;

        if (found_player == false && isDead == false && shooted_at_enemy == true)
            found_player = true;

        if (transform.name == "Enemy")
        {
            if (found_player == true && isDead == false)

            {
                transform.LookAt(player.transform);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player_position() - enemy_position()), Time.deltaTime);
                move_to_player();

            }

        }
        if (transform.name == "Enemy (1)")
        {
            if (found_player == true && isDead == false)
            {
                go_to_hide();
            }

        }
     
    }
    //animator.SetBool("reload_bool", true);
    //        animator.SetTrigger("reload_trigger");



    /// <summary>
    /// Checks if enemy is dead or not
    /// </summary>
    public bool dead_or_not()
    {
        return isDead;
    }


}

