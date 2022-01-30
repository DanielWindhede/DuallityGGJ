using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalState : MonoBehaviour
{
    public static GlobalState state { get; private set; }

    private void Awake() {
        if (state != null && state != this)
            Destroy(this);
        else
            state = this;
    }

    public BlockInputManager blockInputManager;
    public float score;
    public LayerMask groundMask;

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
