using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillZone : MonoBehaviour
{
    [SerializeField] GameObject endScreen;
    [SerializeField] AudioSource deathSoundSource;
    [SerializeField] TextMeshProUGUI textScore;

    private void OnTriggerEnter2D(Collider2D col) {
        var box = col.GetComponent<Block>();
        if (box) {
            box.BlockManager.RemoveFromCollection(box);
            Destroy(box.gameObject);
        }
        if (col.tag == "Player")
        {
            endScreen.SetActive(true);
            textScore.text = "Your Score: " + GlobalState.state.score;
            deathSoundSource.Play();
        }
    }
}
