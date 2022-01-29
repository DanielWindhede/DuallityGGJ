using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlatformSpawnerScript : MonoBehaviour
{
    [SerializeField] float baseSpawnRate;
    float spawnRate;
    [SerializeField] float increesPerPlatform;

    [SerializeField] float gameScreenWidth;
    [SerializeField] float gameScreenHeight;
    
    float nextSpawnHeight;
    float currentYPosition;
    float startPosision;

    [SerializeField] GameObject[] platforms;

    float[] platformSpawnChanceArray;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position - new Vector3(gameScreenWidth, 0f, 0f), 0.1f);
        Gizmos.DrawSphere(transform.position + new Vector3(gameScreenWidth, 0f, 0f), 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + new Vector3(0f, gameScreenHeight, 0f), 0.1f);
    }

    private void Awake()
    {
        currentYPosition = startPosision;
        spawnRate = baseSpawnRate;
    }

    private void Start()
    {
        nextSpawnHeight = currentYPosition + spawnRate;
    }

    void Update()
    {
        if (transform.parent.position.y > nextSpawnHeight)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-gameScreenWidth, gameScreenWidth), nextSpawnHeight + gameScreenHeight);

            Instantiate(PickPlatform(), spawnPos, Quaternion.identity);

            spawnRate += increesPerPlatform;
            nextSpawnHeight += spawnRate;
        }
    }

    private GameObject PickPlatform()
    {
        return platforms[Random.Range(0, platforms.Length)];
    }
}
