using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlatformSpawnerScript : MonoBehaviour
{
    [SerializeField] float baseSpawnRate;
    float spawnRate;
    [SerializeField] float increesPerPlatform;

    Vector2 spawnPos;
    float nextSpawnHeight;
    float mCurrentYPos;
    public float currentYPos { set { mCurrentYPos = value; } }
    float mStartPosisionY;
    public float startPosisionY { set { mStartPosisionY = value; } }

    float totalSpawnChance;

    [SerializeField] GameObject[] platformArray = new GameObject[2];

    float[] platformSpawnChanceArray;

    private void Awake()
    {
        platformSpawnChanceArray = new float[platformArray.Length + 1];
        platformSpawnChanceArray[0] = 0;

        currentYPos = mStartPosisionY;
        spawnRate = baseSpawnRate;

        for (int i = 0; i < platformArray.Length; i++)
        {
            //totalSpawnChance += platformArray[i].GetComponent<BasePlatformScript>().spawnChance;
            platformSpawnChanceArray[i + 1] = totalSpawnChance;
        }
    }

    private void Start()
    {
        nextSpawnHeight = mCurrentYPos + spawnRate;
    }

    void Update()
    {
        if (mCurrentYPos > nextSpawnHeight)
        {
            spawnPos = new Vector2(Random.Range(-2.5f, 2.5f), nextSpawnHeight + 5.5f);
            if (platformArray.Length > 0)
            {
                Instantiate(PickPlatform(), spawnPos, Quaternion.identity);
            }
            spawnRate += increesPerPlatform;
            nextSpawnHeight += spawnRate;
        }
    }

    GameObject PickPlatform()
    {
        float rnd = Random.Range(0, totalSpawnChance);
        Assert.IsTrue(platformArray.Length > 0);

        for (int i = 0; i < platformArray.Length; i++)
        {
            if (platformSpawnChanceArray[i] < rnd && rnd < platformSpawnChanceArray[i + 1])
            {
                return platformArray[i];
            }
        }
        Debug.LogWarning("wops");
        return platformArray[0];
    }
}
