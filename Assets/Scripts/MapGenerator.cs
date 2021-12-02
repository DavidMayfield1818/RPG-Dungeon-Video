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

        // filles teh area with all 0's
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
            for(int x = 1; x < width-1; x++)
            {
                for(int y = 1; y < height-1; y++)
                {
                    int val = Random.Range(0,2);
                    map[x,y] = val;
                }
            }
        }

        // cleans up the map to be less shit
        // this is public so can be called from outside
        public void CleanUp()
        {
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
                            map[x,y] = 0;
                        }
                        else if(curVal == 0)
                        {
                            map[x,y] = 1;
                        }
                    }
                }
            }
        }
    }

    public level bestLevel;

    public Tile groundTile;
	public Tile wallTile;
	public Tilemap ground;
	public Tilemap wall;

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
				else
				{
					ground.SetTile(tilePos,null);
					wall.SetTile(tilePos,null);
				}
			}
		}
	}

    void awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // 73 by 33 fills screen
        level baseMap = new level(73,33);
        int[,] map = baseMap.getMap();
        baseMap.Generate();
        baseMap.CleanUp();
        DrawMap(baseMap);
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
}
