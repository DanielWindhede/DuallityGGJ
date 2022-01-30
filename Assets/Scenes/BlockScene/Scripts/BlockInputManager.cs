using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInputManager : MonoBehaviour, BlockInput
{
    [SerializeField, Min(0)] private float _controlWidth = 10f;

    public delegate void ButtonClick();
    public delegate void FloatFunc(float value);
    public event ButtonClick onAcceptClick;
    public event ButtonClick onRotateLeftClick;
    public event ButtonClick onRotateRightClick;
    public event FloatFunc onRotationAnalog;
    public event FloatFunc onCycle;
    public event ButtonClick onRotateRelease;

    private Camera _camera;
    private float _horzontalValue = 0;
    private float _verticalValue = 0;
    private InputHandler<BlockInput> _inputHandler;

    public Vector2 inputPosition {
        get {
            var offset = Vector2.up * 3;
            var pos = this._camera.transform.position;
            Vector2 centerPosition = new Vector2(pos.x, pos.y);

            return centerPosition + Vector2.right * this._controlWidth / 2 * this._horzontalValue + offset;
        }
    }

    public float VerticalInput {
        get { return this._verticalValue < 0 ? 1 : 0; }
    }

    private void Awake()
    {
        this._camera = Camera.main;
        this._inputHandler = new InputHandler<BlockInput>();
        this._inputHandler.Subscribe(this);
    }

    private void OnDestroy() {
        this._inputHandler.Unsubscribe();
    }

    public void onDirectionCallback(Vector2 value) {
        this._horzontalValue = value.x;
        this._verticalValue = value.y;
    }
    public void onAcceptCallback() { this.onAcceptClick.Invoke(); }
    public void onRotateAnalogCallback(float value) { this.onRotationAnalog.Invoke(value); }
    public void onRotateDigitalLeftCallback() { this.onRotateLeftClick.Invoke(); }
    public void onRotateDigitalRightCallback() { this.onRotateRightClick.Invoke(); }
    public void onRotateAnalogReleaseCallback() { this.onRotateRelease.Invoke(); }
    public void onCycleCallback(float value) { this.onCycle.Invoke(value); }
}
