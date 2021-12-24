using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DoorTrigger : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemy2;

    // Start is called before the first frame update
    void Start()
    {

// enemy = GameObject.FindWithTag("Enemy");
// enemy2 = GameObject.FindWithTag("Enemy (1)");


    }
    public void restart()
    {
        SceneManager.LoadScene("SampleScene");

    }

    /// <summary>
    /// Determines if player win's by checking if both the enemies are dead and restarts the game
    /// </summary>
    
    void OnTriggerEnter(Collider other)
        {

         if ((other.tag == "Player") &&(enemy.GetComponent<Enemy>().dead_or_not() == true)&&(enemy2.GetComponent<Enemy>().dead_or_not() == true))

        {
            print("Won!!!");
                Invoke("restart", 10);
        }
        else
        {
 print("Still enemies alive!!!");
        }
    }
// Update is called once per frame
void Update()
    {
       
    }
}
