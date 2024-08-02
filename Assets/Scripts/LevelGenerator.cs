using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public GameObject brickPrefab;
    public Gradient gradient;
    public int layer = 0; // for default layer
    private List<GameObject> brickList = null;

    public int ChangeLevel(int level)
    {
        if (level == 1) 
        {
            this.GenerateLevelOne();
        }
        else if (level == 2) 
        {
            return this.getBrickCount();
        }
        else if (level == 3) 
        {
            this.DeconstructLevel();
            this.GenerateLevelThree();
        }
        else if (level == 4) 
        {
            this.DeconstructLevel();
            this.GenerateLevelFour();
        }
        else if (level == 5) 
        {
            this.DeconstructLevel();
            this.GenerateLevelFive();
        }
        return this.getBrickCount();
    }


    public void GenerateLevelOne()
    {
        this.brickList = new List<GameObject>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.layer = layer;
                newBrick.transform.position = transform.position + new Vector3((float) ((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
                this.brickList.Add(newBrick);
            }
        }
    }

    public void GenerateLevelTwo()
    // ChangeLevel(int pointMultiplier, int maxBonusPoints, float decreasePaddlePercent, float increaseBallVelocityPercent)
    {
        // blocks stay in play, game behavior attributes change
    }
    
    public void GenerateLevelThree()
    {
        this.brickList = new List<GameObject>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                // Generate checkerboard pattern
                if ((i + j) % 2 == 0)
                {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.layer = layer;
                newBrick.transform.position = transform.position + new Vector3((float) ((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
                this.brickList.Add(newBrick);
                }
            }
        }
    }

    public void GenerateLevelFour()
    {
        this.brickList = new List<GameObject>();
        // Generate "diamond" pattern (Note: blocks generated bottom of the array to top)
        List<int> checkerboard = new List<int> 
        {
            0, 0, 1, 0, 0, 0, 1, 0, 0,
            1, 0, 0, 0, 1, 0, 0, 0, 1,
            0, 1, 0, 1, 0, 1, 0, 1, 0,
            0, 0, 1, 0, 0, 0, 1, 0, 0,
            0, 1, 0, 1, 0, 1, 0, 1, 0,
            1, 0, 0, 0, 1, 0, 0, 0, 1
        };
        for (int j = 0; j < size.y; j++)
        {
            for (int i = 0; i < size.x; i++)
            {
                // Calculate the index in the checkerboard list
                int index = j * (int)size.x + i;
                
                // Check if we should instantiate a brick (1 means instantiate, 0 means skip)
                if (checkerboard[index] == 1)
                {
                    GameObject newBrick = Instantiate(brickPrefab, transform);
                    newBrick.layer = layer;
                    newBrick.transform.position = transform.position + new Vector3(((size.x - 1) * 0.5f - i) * offset.x, j * offset.y, 0);
                    newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
                    this.brickList.Add(newBrick);
                }
            }
        }
    }

    public void GenerateLevelFive()
    {
        this.brickList = new List<GameObject>();
        // Generate "random" pattern (Note: blocks generated bottom of the array to top)
        List<int> checkerboard = new List<int> 
        {
            1, 0, 0, 0, 0, 0, 1, 0, 0,
            0, 0, 1, 0, 1, 0, 1, 0, 1,
            0, 0, 0, 1, 0, 0, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0, 1, 0,
            0, 1, 0, 0, 0, 1, 0, 1, 0,
            0, 0, 0, 0, 1, 0, 0, 0, 0
        };
        for (int j = 0; j < size.y; j++)
        {
            for (int i = 0; i < size.x; i++)
            {
                // Calculate the index in the checkerboard list
                int index = j * (int)size.x + i;
                
                // Check if we should instantiate a brick (1 means instantiate, 0 means skip)
                if (checkerboard[index] == 1)
                {
                    GameObject newBrick = Instantiate(brickPrefab, transform);
                    newBrick.layer = layer;
                    newBrick.transform.position = transform.position + new Vector3(((size.x - 1) * 0.5f - i) * offset.x, j * offset.y, 0);
                    newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
                    this.brickList.Add(newBrick);
                }
            }
        }
    }


    /// <summary>
    /// Destroys the remaining bricks
    /// </summary>
    public void DeconstructLevel()
    {
        if (brickList != null)
        {
            foreach (GameObject brick in this.brickList)
            {
                if (!brick.IsDestroyed()) Destroy(brick);
            }
            this.brickList.Clear();
        }
    }

    public int getBrickCount()
    {
        int count = 0;
        foreach (GameObject brick in this.brickList)
        {
            if (!brick.IsDestroyed()) count++;
        }
        return count;
    }
}
