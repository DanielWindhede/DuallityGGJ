using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInputManager : MonoBehaviour, BlockInput
{
    public delegate void ButtonClick();
    public delegate void FloatFunc(float value);
    public event ButtonClick onAcceptClick;
    public event ButtonClick onRotateLeftClick;
    public event ButtonClick onRotateRightClick;
    public event FloatFunc onRotationAnalog;
    public event ButtonClick onRotateRelease;

    private float horzontalValue = 0;
    private float verticalValue = 0;
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

    public float VerticalInput {
        get { return this.verticalValue < 0 ? 1 : 0; }
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
    public void onDirectionCallback(Vector2 value) {
        this.horzontalValue = value.x;
        this.verticalValue = value.y;
    }
    public void onAcceptCallback() {
        this.onAcceptClick.Invoke();
    }

    public void onRotateAnalogCallback(float value)
    {
        this.onRotationAnalog.Invoke(value);
    }

    public void onRotateDigitalLeftCallback()
    {
        this.onRotateLeftClick.Invoke();
    }

    public void onRotateDigitalRightCallback()
    {
        this.onRotateRightClick.Invoke();
    }

    public void onRotateAnalogReleaseCallback()
    {
        this.onRotateRelease.Invoke();
    }
}
