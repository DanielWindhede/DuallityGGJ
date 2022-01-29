using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float maxYDistanceFromPlayer;
    [SerializeField] float cameraBaseMovementSpeed;
    [SerializeField] float cameraMovementSpeedMultiplyer;
    float startPosition;

    float currentSpeed;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(0f, maxYDistanceFromPlayer, 0f), 0.2f);
    }

    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        startPosition = transform.position.y - 1;

        //GetComponentInChildren<PlatformSpawnerScript>().startPosisionY = transform.position.y;
        //ScoreControllerScript.startPos = transform.position.y;
    }

    void Update()
    {
        if (player.transform.position.y > transform.position.y + maxYDistanceFromPlayer)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y - maxYDistanceFromPlayer, transform.position.z);

            //GetComponentInChildren<PlatformSpawnerScript>().currentYPos = transform.position.y;
            //ScoreControllerScript.addScore(transform.position.y);
        }
        else
        {
            transform.Translate(new Vector3(0f, (cameraBaseMovementSpeed + (transform.position.y - startPosition) * cameraMovementSpeedMultiplyer) * Time.deltaTime, 0f));
            //currentSpeed = cameraBaseMovementSpeed * (transform.position.y - startPosition) * cameraMovementSpeedMultiplyer * Time.deltaTime;
            //Debug.Log(currentSpeed);
        }
    }
}
