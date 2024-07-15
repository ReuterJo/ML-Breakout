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
    private List<GameObject> brickList;

    public int ChangeLevel(int level)
    {
        if (level == 1) 
        {
            GenerateLevelOne();
        }
        else if (level == 2) 
        {
            // pass
        }
        else if (level == 3) 
        {
            GenerateLevelThree();
        }
        else if (level == 4) 
        {
            GenerateLevelFour();
        }
        else if (level == 5) 
        {
            GenerateLevelFive();
        }
        return brickList.Count;
    }

    public void GenerateAllBricks()
    {
        brickList = new List<GameObject>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.transform.position = transform.position + new Vector3((float) ((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
                brickList.Add(newBrick);
            }
        }
    }


    public void GenerateLevelOne()
    {
        GenerateAllBricks();
    }

    public void GenerateLevelTwo()
    // ChangeLevel(int pointMultiplier, int maxBonusPoints, float decreasePaddlePercent, float increaseBallVelocityPercent)
    {
        // blocks stay in play, game behavior attributes change
    }
    
    public void GenerateLevelThree()
    {
        GenerateAllBricks();
    }

    public void GenerateLevelFour()
    {
        List<int> checkerboard = new List<int> 
        {
            0, 1, 0, 1, 0, 1, 0, 1, 0,
            1, 0, 1, 0, 1, 0, 1, 0, 1,
            0, 1, 0, 1, 0, 1, 0, 1, 0,
            1, 0, 1, 0, 1, 0, 1, 0, 1,
            0, 1, 0, 1, 0, 1, 0, 1, 0,
            1, 0, 1, 0, 1, 0, 1, 0, 1
        };
        GenerateAllBricks();
    }

    public void GenerateLevelFive()
    {
        GenerateAllBricks();
    }


    /// <summary>
    /// Destroys the remaining bricks
    /// </summary>
    public void DeconstructLevel()
    {
        foreach (GameObject brick in brickList)
        {
            if (!brick.IsDestroyed()) Destroy(brick);
        }
        brickList.Clear();
    }
}
