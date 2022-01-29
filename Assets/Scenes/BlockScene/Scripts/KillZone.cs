using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col) {
        var box = col.GetComponent<Block>();
        if (box) {
            box.BlockManager.RemoveFromCollection(box);
            Destroy(box.gameObject);
        }
    }
}
