using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private float speed = 2.0f;
    Rigidbody2D rb;

    private float health = 300;
    private float startHealth;
    
    //Trackers for map generation 
     
    public float healthAtEndOfLevel;
    public int totalZombies = 0;
    public int totalKilled = 0;
    public float timeToFinishLevel;
    public float xStart = 0; // based off the point of entry 
    public float yStart = 0; // on previous level 
    public int itemsCollected = 0;
    public int spawnerDestroyed = 0;
    public int spawnersGen = 0;
    public int[,] explored;
    
    //end of trackers

    public bool turnedLeft = false;
    public Image healthFill;
    private float healthWidth;

    public Text mainText;
    public Image redOverlay;
    public Text expText;

    public GameManager gameManager;
    public Tilemap floor;

    private int experience = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthWidth = healthFill.sprite.rect.width;
        startHealth = health;
        mainText.gameObject.SetActive(true);
        redOverlay.gameObject.SetActive(true);
        Invoke("HideTitle", 2);

    }

    // Update is called once per frame
    void Update()
    {
        updateExplored();
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
        xStart = GameObject.FindGameObjectWithTag("Player").transform.position.x;
        yStart = GameObject.FindGameObjectWithTag("Player").transform.position.y;
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

    public void SetSpawnerDestroyed() {
        spawnerDestroyed += 1;
    }
    
    public int GetSpawnerDestroyed() {
        return spawnerDestroyed;
    }
    
    public void SetSpawnerGen(int amount){
        spawnersGen  = amount;
    }

    public int GetSpawnerGen(){
        return spawnersGen; 
    }

    public void copyMap(int[,] map, int width , int height){
        explored = map;
    }
    public void updateExplored(){
        Vector3Int curPos = floor.WorldToCell(this.transform.position);
        
        explored[curPos.x,curPos.y] = 3;
        explored[curPos.x+1,curPos.y] = 3;
        explored[curPos.x-1,curPos.y] = 3;
        explored[curPos.x,curPos.y+1] = 3;
        explored[curPos.x,curPos.y-1] = 3;

    }

    // end of trackers

    public void UseHealthPotion()
    {
        if(health > 200){
            health = 300; // make sure potion doesnt go over max health 
        }
        else{
            health += 100; // change to fit
        }
        Vector2 temp = new Vector2(healthWidth * (health / startHealth), healthFill.sprite.rect.height);
            healthFill.rectTransform.sizeDelta = temp;
    }
 
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
            collision.gameObject.GetComponent<SpawnerScript>().GetGatewayPotion();
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

    public void setupExplored()
    {
        explored = new int[73,33];
        for(int x = 0; x < 73; x++)
        {
            for(int y = 0; y < 33; y++)
            {
                explored[x,y] = 0;
            }
        }
    }

}
