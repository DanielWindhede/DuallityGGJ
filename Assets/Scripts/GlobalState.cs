using UnityEngine;

public class GlobalState : MonoBehaviour
{
    public static GlobalState Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public BlockInputManager blockInputManager;
}
