using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private float speed = 4.0f;
    Rigidbody2D rb;

    private float health = 200;
    private float startHealth;
    
    //Trackers for map generation 
    private Transform target; // for player postion 
    public float healthAtEndOfLevel;
    public int totalZombies = 0;
    public int totalKilled = 0;
    public float timeToFinishLevel;
    public float xStart; // based off the point of entry 
    public float yStart; // on previous level 
    public int itemsCollected = 0;

    public bool turnedLeft = false;
    public Image healthFill;
    private float healthWidth;

    public Text mainText;
    public Image redOverlay;
    public Text expText;

    private int experience = 0;

    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        healthWidth = healthFill.sprite.rect.width;
        startHealth = health;
        mainText.gameObject.SetActive(true);
        redOverlay.gameObject.SetActive(true);
        Invoke("HideTitle", 2);

        // add our variables starting values
        target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(horizontal * speed, vertical * speed);
        if (horizontal > 0)
        {
            GetComponent<Animator>().Play("Right");
            turnedLeft = false;
        } else if (horizontal < 0)
        {
            GetComponent<Animator>().Play("Left");
            turnedLeft = true;
        } else if (vertical > 0)
        {
            GetComponent<Animator>().Play("Up");
        } else if (vertical < 0)
        {
            GetComponent<Animator>().Play("Down");
        }
    }


    /// public functions for trackers 
    public void SetHealthEnd()
    {
        healthAtEndOfLevel = health;
    }

    public float GetHealthEnd()
    {
        return healthAtEndOfLevel;
    }

    public void SetZombieStats(int amount){
        if (amount < 0) {
            totalKilled+= (amount)*(-1);
        }
        else {
            totalZombies += amount ;
        }
    }

    public int GetTotalZombies()
    {
        return totalZombies;
    }

    public int GetTotalKilled()
    {
        return totalKilled;
    }

    public void SetTimeToFinishLevel(){
        timeToFinishLevel = Time.time - timeToFinishLevel;
    }
    
    public float GetTimeToFinishLevel() {
        return timeToFinishLevel;
    }

    
    public void SetStartPostion(){
        xStart = target.position.x;
        yStart = target.position.y;
    }
    
    public float GetStartX() {
        return xStart;
    }

    public float GetStartY() {
        return yStart;
    }
    public void SetItemsCollected() {
        itemsCollected += 1;
    }

    public int GetItemsCollected(){
        return itemsCollected;
    }
    // end of trackers
 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            health -= collision.gameObject.GetComponent<EnemyScript>().GetHitStrength();
            if (health < 1)
            {
                healthFill.enabled = false;
                mainText.gameObject.SetActive(true);
                mainText.text = "Game Over";
                redOverlay.gameObject.SetActive(true);
            }
            Vector2 temp = new Vector2(healthWidth * (health / startHealth), healthFill.sprite.rect.height);
            healthFill.rectTransform.sizeDelta = temp;
            Invoke("HidePlayerBlood", 0.25f);
        }
        else if (collision.gameObject.CompareTag("Spawner"))
        {
            collision.gameObject.GetComponent<SpawnerScript>().GetGatewayWeapon();
        }
    }

    void HidePlayerBlood()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void GainExperience(int amount)
    {
        experience += amount;
        expText.text = experience.ToString();
    }

    void HideTitle()
    {
        mainText.gameObject.SetActive(false);
        redOverlay.gameObject.SetActive(false);
    }

}
