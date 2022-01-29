using UnityEngine;

public interface BlockInput
{
    void onHorizontalCallback(float value);
    void onAcceptCallback();
}

public interface PlayerInput
{
    void onMoveCallback(Vector2 value);
    void onJumpCallback(bool value);
    void onDashCallback();
}

public class InputHandler<T>
{
  private InputActions inputActions;

  public void Subscribe(T input)
  {
    this.inputActions = new InputActions();
    this.inputActions.Enable();
    switch(input)
        {
            case BlockInput i:
                this.inputActions.Block.Horizontal.Enable();
                this.inputActions.Block.Horizontal.performed += context => i.onHorizontalCallback(context.ReadValue<float>());

                this.inputActions.Block.Accept.Enable();
                this.inputActions.Block.Accept.performed += _ => i.onAcceptCallback();
                break;
            case PlayerInput i:
                this.inputActions.Player.Move.Enable();
                this.inputActions.Player.Move.performed += context => i.onMoveCallback(context.ReadValue<Vector2>());
                this.inputActions.Player.Move.canceled += context => i.onMoveCallback(context.ReadValue<Vector2>());

                this.inputActions.Player.Jump.Enable();
                this.inputActions.Player.Jump.performed += context => i.onJumpCallback(true);
                this.inputActions.Player.Jump.canceled += context => i.onJumpCallback(false);

                this.inputActions.Player.Dash.Enable();
                this.inputActions.Player.Dash.performed += context => i.onDashCallback();
                break;
        }
  }

  public void Unsubscribe()
  {
    this.inputActions.Block.Horizontal.Disable();
    this.inputActions = null;
  }
}