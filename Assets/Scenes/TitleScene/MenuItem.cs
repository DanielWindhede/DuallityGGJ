using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuItem : MonoBehaviour
{
    public enum MenuActionType {
        Play,
        Quit,
    }

    [SerializeField] private MenuActionType type;
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