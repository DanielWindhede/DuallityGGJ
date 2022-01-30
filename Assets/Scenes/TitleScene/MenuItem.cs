using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MenuItem : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Sprite sprite;
    [SerializeField] Sprite sprite1;
    public enum MenuActionType {
        Play,
        Quit,
    }

    [SerializeField] private MenuActionType type;

    public void PlaySound()
    {
        audioSource.Play();
    }

    public void ChangeSprite()
    {
        GetComponent<Image>().sprite = sprite;
    }

    public void ChangeSpriteBack()
    {
        GetComponent<Image>().sprite = sprite1;
    }

    public void PerformAction() {
        switch (type) {
            case MenuActionType.Play:
                SceneManager.LoadScene(1);
                break;
            case MenuActionType.Quit:
                ExitGame();
                break;
        }
    }

    private void ExitGame() {
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
         Application.Quit();
        #endif
    }
}
