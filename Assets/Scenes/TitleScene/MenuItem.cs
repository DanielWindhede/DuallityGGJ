using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MenuItem : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject sprite1;
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

    }

    public void ChangeSpriteBack()
    {

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
