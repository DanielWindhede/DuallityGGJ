using UnityEngine;

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
}
