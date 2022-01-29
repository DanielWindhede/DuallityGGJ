using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInputManager : MonoBehaviour, BlockInput
{
    public delegate void AcceptClick();
    public event AcceptClick onAcceptClick;

    private float horzontalValue = 0;
    [SerializeField] private Camera camera;
    [SerializeField, Min(0)] private float width = 10f;
    private InputHandler<BlockInput> inputHandler;

    public Vector2 inputPosition {
        get {
            var offset = Vector2.up * 3;
            var pos = this.camera.transform.position;
            Vector2 centerPosition = new Vector2(pos.x, pos.y);

            return centerPosition + Vector2.right * this.width / 2 * this.horzontalValue + offset;
        }
    }

    void Awake()
    {
        this.inputHandler = new InputHandler<BlockInput>();
        this.inputHandler.Subscribe(this);
    }

    private void OnDestroy() {
        this.inputHandler.Unsubscribe();
    }

    // Input Interface Implementation
    public void onHorizontalCallback(float value) {
        this.horzontalValue = value;
    }
    public void onAcceptCallback() {
        this.onAcceptClick.Invoke();
        // Instantiate(this.blockObjects[0], inputPosition, Quaternion.identity);
    }
    private void OnDrawGizmos() {
        // var offset = Vector2.up * 3;
        // var pos = this.camera.transform.position;
        // Vector2 centerPosition = new Vector2(pos.x, pos.y);
        // Vector2 initialPosition = centerPosition + Vector2.left * this.width / 2;
        // Vector2 finalPosition = centerPosition + Vector2.right * this.width / 2;

        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(initialPosition + offset, finalPosition + offset);

        // Gizmos.DrawSphere(this.inputPosition, 0.5f);
    }
}
