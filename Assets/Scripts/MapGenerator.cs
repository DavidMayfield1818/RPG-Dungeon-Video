using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class MapGenerator : MonoBehaviour
{   
    public class level
    {
        // this class contains one level
        // variables here
        public int[,] map;
        public int width;
        public int height;
        

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
            int rng = Random.Range(1,15);

            // place em
            for(int i = 0; i < rng; i++)
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
            }

        }

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
            // make 3 horizontal and 4 vertical hallways
            var row1 = Random.Range(2,height/3);
            var row2 = Random.Range(height/3,height/3*2);
            var row3 = Random.Range(height/3*2,height-2);
            var col1 = Random.Range(2,width/4);
            var col2 = Random.Range(width/4,width/4*2);
            var col3 = Random.Range(width/4*2,width/4*3);
            var col4 = Random.Range(width/4*3,width-2);

            // slice
            for(int x = 1; x < width-1; x++)
            {
                makeInto(x,row1,0);
                makeInto(x,row2,0);
                makeInto(x,row3,0);
            }

            // slice
            for(int y = 1; y < height-1; y++)
            {
                makeInto(col1,y,0);
                makeInto(col2,y,0);
                makeInto(col3,y,0);
                makeInto(col4,y,0);
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

        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        //Debug.Log("postKill: "+spawners.Length);

        // make teh new one here
        spawn = playerLoc.position;
        mapSpawn = ground.WorldToCell(spawn);
        // 73 by 33 fills screen
        level baseMap = new level(73,33);
        baseMap.Generate();
        baseMap.CleanUp(mapSpawn.x,mapSpawn.y);
        DrawMap(baseMap);
        
        // last step before running the map
        gameMan.PrepareSpawners();
    }
}
