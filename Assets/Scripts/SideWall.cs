using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour
{
    [SerializeField] GameObject otherWall;
    [SerializeField] float xOffset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.transform.position = new Vector2(otherWall.transform.position.x + xOffset, collision.gameObject.transform.position.y);
        }
    }
}
