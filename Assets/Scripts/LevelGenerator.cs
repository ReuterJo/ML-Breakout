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

    [Tooltip("Sets the GameManager script")]
    public GameManager gameManager;

    public int ChangeLevel(int level)
    {
        if (level == 1) 
        {
            GenerateLevelOne();
            return size.x * size.y;
        }
        else if (level == 2) 
        {
            GenerateLevelTwo();
            return 27;   // TODO
        }
        else if (level == 3) 
        {
            GenerateLevelThree();
            return 27;
        }
        else if (level == 4) 
        {
            GenerateLevelFour();
            return 18;
        }
        else if (level == 5) 
        {
            GenerateLevelFive();
            return 12;
        }
        else return 0;
    }

    public void GenerateLevelOne()
    {
        brickList = new List<GameObject>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                brickList.Add(newBrick);
                newBrick.transform.position = transform.position + new Vector3((float) ((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
            }
        }
        // game behavior attributes remain the same
    }

    public void GenerateLevelTwo()
    // ChangeLevel(int pointMultiplier, int maxBonusPoints, float decreasePaddlePercent, float increaseBallVelocityPercent)
    {
        // blocks stay in play, game behavior attributes change
        gameManager.ChangeLevel(0.5f, 1000, 0.9f, 1.1f);
    }
    
    public void GenerateLevelThree()
    {
        brickList = new List<GameObject>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                brickList.Add(newBrick);
                newBrick.transform.position = transform.position + new Vector3((float) ((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
            }
        }
        gameManager.ChangeLevel(0.5f, 2000, 0.9f, 1.1f);
    }

    public void GenerateLevelFour()
    {
        brickList = new List<GameObject>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                brickList.Add(newBrick);
                newBrick.transform.position = transform.position + new Vector3((float) ((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
            }
        }
        gameManager.ChangeLevel(0.5f, 3000, 0.9f, 1.1f);
    }

    public void GenerateLevelFive()
    {
        brickList = new List<GameObject>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                brickList.Add(newBrick);
                newBrick.transform.position = transform.position + new Vector3((float) ((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j/(size.y-1));
            }
        }
        gameManager.ChangeLevel(0.5f, 1000, 0.9f, 1.1f);
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
