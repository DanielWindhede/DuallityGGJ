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
            collision.transform.parent.position = new Vector2(otherWall.transform.position.x + xOffset, collision.transform.parent.position.y);
        }
    }
}
