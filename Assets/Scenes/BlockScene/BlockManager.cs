using System;
using UnityEngine;
public interface BlockInput
{
  void onHorizontalCallback(float value);
  void onAcceptCallback();
}

public class InputHandler<T>
{
  private InputActions inputActions;

  public void Subscribe(T input)
  {
    this.inputActions = new InputActions();
    this.inputActions.Enable();
    switch(input) {
        case BlockInput i:
            this.inputActions.Block.Horizontal.Enable();
            this.inputActions.Block.Horizontal.performed += context => i.onHorizontalCallback(context.ReadValue<float>());

            this.inputActions.Block.Accept.Enable();
            this.inputActions.Block.Accept.performed += _ => i.onAcceptCallback();
            break;
    }
  }

  public void Unsubscribe()
  {
    this.inputActions.Block.Horizontal.Disable();
    this.inputActions = null;
  }
}

public class BlockManager : MonoBehaviour, BlockInput
{
    private float horzontalValue = 0;
    [SerializeField] private Camera camera;
    [SerializeField, Min(0)] private float width = 10f;
    private InputHandler<BlockInput> inputHandler;

    // Start is called before the first frame update
    void Start()
    {
        this.inputHandler = new InputHandler<BlockInput>();
        this.inputHandler.Subscribe(this);
        this.camera = Camera.main;
    } 

    private void OnDestroy() {
        this.inputHandler.Unsubscribe();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    // Input Interface Implementation
    public void onHorizontalCallback(float value) {
        this.horzontalValue = value;
    }
    public void onAcceptCallback() {
        print("pressed!");
    }

    private void OnDrawGizmos() {
        var offset = Vector2.up * 3;
        var pos = this.camera.transform.position;
        Vector2 centerPosition = new Vector2(pos.x, pos.y);
        Vector2 initialPosition = centerPosition + Vector2.left * this.width / 2;
        Vector2 finalPosition = centerPosition + Vector2.right * this.width / 2;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(initialPosition + offset, finalPosition + offset);

        Gizmos.DrawSphere(centerPosition + Vector2.right * this.width / 2 * this.horzontalValue + offset, 0.5f);
    }
}
