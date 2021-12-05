using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class MapGenerator : MonoBehaviour
{   
    public static GameObject player;
    public class level
    {
        public class node
        {
            public Vector3Int loc;
            public int distTraveled;
            public float distToGo;
            public node parent;

            public node(Vector3Int L, int T, float TG, node P){
                loc = L;
                distTraveled = T;
                distToGo = TG;
                parent = P;
            }
        }

        // this class contains one level
        // variables here
        public int[,] map;
        public int width;
        public int height;
        public int rowCount;
        public int columnCount;
        public float rating;

        // data vars for rating
        public Vector3Int playerSpawn;
        public int spawnerCount;
        public Vector3Int[] spawners;

        // contructor
        // this is public so can be called from outside
        public level(int w, int h)
        {
            // add check to make sure w and h are legal
            // TBDL
            width = w;
            height = h;
            clearLevel();
        }

        // filles the area with all 0's
        // requires w and h to be positve
        void clearLevel()
        {
            map = new int[width,height];
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    map[x,y] = 0;
                    // 0 will be open air
                }
            }
            addPerimeter();
        }

        // add the perimeter wall around the map
        void addPerimeter()
        {
            for(int x = 0; x < width; x++)
            {
                map[x,0] = 1;
                map[x,height-1] = 1;
                // 1 be wall
            }
            for(int y = 0; y < height; y++)
            {
                map[0,y] = 1;
                map[width-1,y] = 1;
                // 1 be wall
            }
        }

        // returns the map
        public int[,] getMap(){
            return map;
        }

        // generates a random spray of stuff onto the map
        // this is public so can be called from outside
        public void Generate()
        {
            // define number or rows and columns
            rowCount = Random.Range(2,5);
            columnCount = Random.Range(3,7);

            // inital put shit on the map
            for(int x = 1; x < width-1; x++)
            {
                for(int y = 1; y < height-1; y++)
                {
                    int val = Random.Range(0,2);
                    map[x,y] = val;
                }
            }

            // random count of spawners
            spawnerCount = Random.Range(1,15);
            spawners = new Vector3Int[spawnerCount];
            //track spawners generated 
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerScript>().SetSpawnerGen(spawnerCount);
            // place em
            for(int i = 0; i < spawnerCount; i++)
            {
                int x = Random.Range(3,width-3);
                int y = Random.Range(3,height - 3);
                for(int surX = x - 1; surX <= x + 1; surX++)
                {
                    for(int surY = y - 1; surY <= y + 1; surY++)
                    {
                        map[surX,surY] = 0;
                    }
                }
                map[x,y] = 2;
                spawners[i] = new Vector3Int(x,y,0);
            }

        }

        // helper placement function
        private void makeInto(int x, int y, int result)
        {
            if(map[x,y] != 2)
            {
                map[x,y] = result;
            }
        }

        // cleans up the map to be less shit
        // this is public so can be called from outside
        public void CleanUp(int spawnX,int spawnY)
        {
            playerSpawn = new Vector3Int(spawnX,spawnY,0);

            int[] rows = new int[rowCount];
            int[] cols = new int[columnCount];

            for(int r = 0; r < rowCount; r++)
            {
                if(r==0)
                {
                    rows[r] = Random.Range(2,height/((r+1)*rowCount));
                }
                else if(r==rowCount-1)
                {
                    rows[r] = Random.Range(height/(rowCount*r),height-2);
                }
                else
                {
                    rows[r] = Random.Range(height/(rowCount*r),height/(r+1*rowCount));
                }
            }

            for(int c = 0; c < columnCount; c++)
            {
                if(c==0)
                {
                    cols[c] = Random.Range(2,width/((c+1)*columnCount));
                }
                else if(c==columnCount-1)
                {
                    cols[c] = Random.Range(width/(columnCount*c),width-2);
                }
                else
                {
                    cols[c] = Random.Range(width/(columnCount*c),width/(c+1*columnCount));
                }
            }


            // slice
            for(int r = 0; r < rowCount; r++)
            {   
                for(int x = 1; x < width-1; x++)
                {
                    makeInto(x,rows[r],0);
                }
            }

            // slice
            for(int c = 0; c < columnCount; c++)
            {   
                for(int y = 1; y < height-1; y++)
                {
                    makeInto(cols[c],y,0);
                }
            }

            // clear area around player acts as spawn;
            for(int x = spawnX - 1; x <= spawnX + 1; x++)
            {
                for(int y = spawnY - 1; y <= spawnY + 1; y++)
                { 
                    if(x!=0 && x != width-1 && y != 0 && y != height-1)
                    {
                        makeInto(x,y,0);
                    }
                }
            }

            // soften
            for(int x = 1; x < width-1; x++)
            {
                for(int y = 1; y < height-1; y++)
                {
                    // find the current tiles value
                    var curVal = map[x,y];

                    // count how many tiles next to it are the same
                    var count = 0;
                    if(map[x+1,y]==curVal)
                    {
                        count++;
                    }
                    if(map[x-1,y]==curVal)
                    {
                        count++;
                    }
                    if(map[x,y+1]==curVal)
                    {
                        count++;
                    }
                    if(map[x,y-1]==curVal)
                    {
                        count++;
                    }

                    // if count is 0 or 1 swap this tile
                    if(count<2)
                    {
                        if(curVal == 1)
                        {
                            makeInto(x,y,0);
                        }
                        else if(curVal == 0)
                        {
                            makeInto(x,y,1);
                        }
                    }
                }
            }
        }
    
        // goal of this is to rate the level itself
        // rate must be called after CleanUp()
        public float rate(GameObject player, bool verbose)
        {
            // load the trackers in
            float remainingHealth = player.GetComponent<PlayerScript>().GetHealthEnd();
            int remainingZombies = player.GetComponent<PlayerScript>().GetTotalZombies();
            int killedZombies = player.GetComponent<PlayerScript>().GetTotalKilled();
            float timeToFinish = player.GetComponent<PlayerScript>().GetTimeToFinishLevel();
            float pSpawnX = player.GetComponent<PlayerScript>().GetStartX();
            float pSpawnY = player.GetComponent<PlayerScript>().GetStartY();
            int playerItems = player.GetComponent<PlayerScript>().GetItemsCollected();
            int spawnersDestroyed = player.GetComponent<PlayerScript>().GetSpawnerDestroyed();
            int spawnerCount = player.GetComponent<PlayerScript>().GetSpawnerGen();
            int[,] explored = player.GetComponent<PlayerScript>().explored;
            float totalSpawnerLife = player.GetComponent<PlayerScript>().gameManager.totalTimeOfSpawners;
            
            // extra variables assignment
            float beatAbilty = 10f;
            float pathValidity = validPath();
            float spawnerDistance = averageDistancetoSpawner();
            float wallToTotal = ratioOfWalltoFloor();
            float vertical = (float)columnCount/7;
            float horizontal = (float)rowCount/5;

            // area for math n stuff
            float doablity = pathValidity * beatAbilty;
            
            // if hp is high then put more weight on higher spawner counts
            // if hp is low put more wieght on less spawner count
            // if remainingHealth/200 is similar to spawnerCount/15 mucho bueno
            float hpSpawnerRatio = (1f-(Mathf.Abs(remainingHealth/200f - spawnerCount/15f)));            

            // if time to finish is low then favor farther spawners
            float timetoSpawnerDistance = (100f-timeToFinish)/80f*(spawnerDistance/40f);
            
            // if explored has a high amount of exploration (AKA many 3's) favor increased walls
            // if they explore a ton then wall matter, if they don't explore alot then walls mean less
            float explorationtoWall = numberof3s(explored) * ratioOfWalltoFloor();
            
            // if killedZombies is high and player hp is high then favor more vertical
            // more player powerups mean increase difficulty
            float verticalRating = (playerItems)*((remainingHealth+50)/200f)*vertical;
            float horizontalRating = (2-playerItems)*(1-remainingHealth/200f)*horizontal;

            // if player has more wepon upgrade wieght towards more spawners
            float weaponUpgradetoSpawnerCount = (playerItems)*(spawnerCount/15f);
            
            // additive polish metrics make a elite spawner if need mo difficult

            if(verbose)
            {
                Debug.Log("doablity: "+doablity);
                Debug.Log("hpSpawnerRatio: "+hpSpawnerRatio);
                Debug.Log("timetoSpawnerDistance: "+timetoSpawnerDistance);
                Debug.Log("explorationtoWall: "+explorationtoWall);
                Debug.Log("verticalRating: "+verticalRating);
                Debug.Log("horizontalRating: "+horizontalRating);
                Debug.Log("weaponUpgradetoSpawnerCount: "+weaponUpgradetoSpawnerCount);
                Debug.Log("Total: "+(doablity+hpSpawnerRatio+timetoSpawnerDistance+explorationtoWall+verticalRating+horizontalRating+weaponUpgradetoSpawnerCount));
            }

            return doablity+hpSpawnerRatio+timetoSpawnerDistance+explorationtoWall+verticalRating+horizontalRating+weaponUpgradetoSpawnerCount;
        }

        // area for rate() helper functions
        // find path from player spawn to every spawner
        // returns number of spawners that were reached/total spawners
        public float validPath()
        {
            float numberReached = 0;
            // try and find every spawner
            for(int i = 0; i < spawnerCount;i++)
            {

                int[,] travelled = new int[width,height];
                for(int w = 0; w < width; w++)
                {
                    for(int h = 0; h < height; h++);
                }
                Vector3Int curSpawner = spawners[i];
                bool notFound = true;

                // set up an array of nodes
                Dictionary<int,node> nodes = new Dictionary<int, node>();
                int currentKey = 0;
                nodes.Add(currentKey,new node(playerSpawn,0,Vector3Int.Distance(playerSpawn,curSpawner),null));
                currentKey++;

                // Q is a sorted list... like a dict is has a key and value, sorted based on value
                SortedList Q = new SortedList();

                // Q.Add(key: new node(loc,distTravled,distToGo),value: distToGo+distTravled)
                Q.Add(0,Vector3Int.Distance(playerSpawn,curSpawner)+0);

                // start of queue is set

                while(notFound && Q.Count!=0)
                {
                    // get the node
                    node N = nodes[(int)Q.GetKey(0)];
                    travelled[N.loc.x,N.loc.y]=2;
                    Q.RemoveAt(0);

                    // break the loop
                    if(N.loc.x==curSpawner.x && N.loc.y==curSpawner.y)
                    {
                        // Found the spawner
                        notFound = false;
                        numberReached++;
                        curSpawner.z = N.distTraveled;
                    }

                    // set up children
                    Vector3Int[] children = new Vector3Int[4];
                    children[0] = new Vector3Int(N.loc.x+1,N.loc.y+0,N.loc.z);
                    children[1] = new Vector3Int(N.loc.x-1,N.loc.y+0,N.loc.z);
                    children[2] = new Vector3Int(N.loc.x+0,N.loc.y+1,N.loc.z);
                    children[3] = new Vector3Int(N.loc.x+0,N.loc.y-1,N.loc.z);
                    
                    for(int c = 0; c < 4; c++)
                    {
                        // if empty or spawner AND we haven't seen it yet add it to the Q
                        if((map[children[c].x,children[c].y]==0 || map[children[c].x,children[c].y]==2) && travelled[children[c].x,children[c].y]==0)
                        {
                            // make the child into a node casue its valid
                            // node(vector3int position, distance traveled, distance to go, parent)
                            node childnode = new node(children[c],N.distTraveled+1,Vector3Int.Distance(N.loc,curSpawner),N);
                            nodes.Add(currentKey,childnode);
                            travelled[children[c].x,children[c].y]=1;
                            Q.Add(currentKey,childnode.distTraveled+childnode.distToGo);
                            currentKey++;
                        }
                    }
                    // end children setup
                }
            }
            return numberReached/(float)spawnerCount;
        }

        public float averageDistancetoSpawner()
        {
            float total = 0;
            for(int i = 0; i < spawnerCount; i++)
            {
                total += Vector3Int.Distance(spawners[i],playerSpawn);
            }

            return total/spawnerCount;
        }
    
        public float ratioOfWalltoFloor()
        {
            float wallCount = 0;
            float totalCount = 2201;
            for(int x = 1; x < width-1; x++)
            {
                for(int y = 1; y < height-1; y++)
                {
                    if(map[x,y]==1)
                    {
                        wallCount++;
                    }
                }
            }
            return wallCount/totalCount;
        }
    
        public float numberof3s(int[,] explored)
        {
            float threeCount = 0;
            float zeroCount = 0;
            for(int x = 1; x < width-1; x++)
            {
                for(int y = 1; y < height-1; y++)
                {
                    if(explored[x,y]==3)
                    {
                        threeCount++;
                    }
                    if(map[x,y]==0)
                    {
                        zeroCount++;
                    }
                }
            }
            return threeCount/(zeroCount/2f);
        }
    }

    

    public level bestLevel;

    public GameObject spawnerPrefab;

    public Tile groundTile;
	public Tile wallTile;
	public Tilemap ground;
	public Tilemap wall;
    public GameManager gameMan;

    public Transform playerLoc;
    public Vector3 spawn;
    public Vector3Int mapSpawn;

    //for trackers 
    // public GameObject player;
    void DrawMap(level lvl)
	{
		for(int x = 0; x < lvl.width; x++)
		{
			for(int y = 0; y < lvl.height; y++)
			{
				//Debug.Log("x: "+x+" y: "+y+" = "+map[x,y]);
				Vector3Int tilePos = new Vector3Int(x,y,0);
				if(lvl.map[x,y]==1)
				{
					wall.SetTile(tilePos,wallTile);
					ground.SetTile(tilePos,null);
				}
				else if(lvl.map[x,y]==0)
				{
					ground.SetTile(tilePos,groundTile);
					wall.SetTile(tilePos,null);
				}
                else if(lvl.map[x,y]==2)
				{
                    Vector3 destination = ground.GetCellCenterWorld(new Vector3Int(x,y,0));
					Instantiate(spawnerPrefab, destination, Quaternion.identity);
                    ground.SetTile(tilePos,groundTile);
					wall.SetTile(tilePos,null);
				}
				else
				{
					ground.SetTile(tilePos,null);
					wall.SetTile(tilePos,null);
				}
			}
		}
	}

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMan.player.GetComponent<PlayerScript>().setupExplored();
        PutMapOnScreen();
    }

    void update()
    {
        // check for a need new level thing
        // then trigger the map gen
        // then overwrite old map stuff
    }

    void StartingPopulation()
    {
        // make a starting population


    }

    public void PutMapOnScreen()
    {
        // make sure trackers are up to date
        GameObject player;
        player = gameMan.player;
        player.GetComponent<PlayerScript>().SetHealthEnd();
        player.GetComponent<PlayerScript>().SetTimeToFinishLevel();
        player.GetComponent<PlayerScript>().SetStartPostion();

        // clear all the existing stuffs
        GameObject[] spawners;
        GameObject[] enemies;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        for(int i = 0; i < spawners.Length; i++)
        {

            spawners[i].tag = "Untagged";
            Destroy(spawners[i].gameObject);
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].tag = "Untagged";
            Destroy(enemies[i].gameObject);
        }

        //spawners = GameObject.FindGameObjectsWithTag("Spawner");
        //Debug.Log("postKill: "+spawners.Length);

        // make the new one here
        // --------------------------------------------------------------------------------
        // setup some creation variables
        spawn = playerLoc.position;
        mapSpawn = ground.WorldToCell(spawn);
        int popCount = 1000;

        // new stuff here -----------------------------------------------------------------
        level bestLevel = new level(73,33);
        float bestLevelScore = -1;
        for(int i = 0; i < popCount; i++)
        {
            level currentLevel = new level(73,33);
            currentLevel.Generate();
            currentLevel.CleanUp(mapSpawn.x,mapSpawn.y);
            float currentScore = currentLevel.rate(player,false);
            if(currentScore > bestLevelScore)
            {
                bestLevel = currentLevel;
                bestLevelScore = currentScore;
            }
        }

        // --------------------------------------------------------------------------------
        // draw the ouput ot the previous section
        DrawMap(bestLevel);
        gameMan.PrepareSpawners();
        bestLevel.rate(player,true);

        // reset trackers
        player.GetComponent<PlayerScript>().copyMap(bestLevel.map, bestLevel.width, bestLevel.height);
        player.GetComponent<PlayerScript>().totalKilled = 0;
        player.GetComponent<PlayerScript>().totalZombies = 0;
        player.GetComponent<PlayerScript>().spawnerDestroyed = 0;
        gameMan.totalTimeOfSpawners = 0;
    }
}
