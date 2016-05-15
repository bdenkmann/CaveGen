using UnityEngine;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour {

    public const int SurroundingWallCountThreshold = 4;

    public int width;
    public int height;

    public int randomSeed;
    public bool useRandomSeed;

    [Range(0,100)]
    public int randomFillPercent;

    int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for(int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        if(useRandomSeed)
        {
            randomSeed = DateTime.Now.ToString().GetHashCode();
        }

        System.Random random = new System.Random(randomSeed);

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    if (random.Next(0, 100) < randomFillPercent)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        map[x, y] = 0;
                    }
                }
            }
        }
    }

    void SmoothMap()
    {
        // use cellular automata to smooth the cave walls, 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingWallCount = GetSurroundingWallCount(x, y);
                if(surroundingWallCount > SurroundingWallCountThreshold)
                {
                    // if it's above the threshold, change it to a wall
                    map[x, y] = 1;
                } else if(surroundingWallCount < SurroundingWallCountThreshold)
                {
                    // if it's below the threshold, change it to a space
                    map[x, y] = 0;
                }
                // if it's equal to the threshold, keep it the same
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        // get all neighboring cells, including diagonals
        for (int x = gridX-1; x <= gridX+1; x++)
        {
            for (int y = gridY-1; y <= gridY+1; y++)
            {
                // don't count the grid coord we are actually looking at
                if(x != gridX || y != gridY)
                {
                    // if our coord is outside the map, or it's inside an it's a wall, count it
                    if (CoordIsInsideMapBoundary(x, y))
                    {
                        wallCount += map[x,y];
                    } else
                    {
                        wallCount++;
                    }
                }
            }
        }
        return wallCount;
    }

    bool CoordIsInsideMapBoundary(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
