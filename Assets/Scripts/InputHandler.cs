using UnityEngine;

public interface BlockInput
{
  void onHorizontalCallback(float value);
  void onAcceptCallback();
  void onRotateAnalogCallback(float value);
  void onRotateAnalogReleaseCallback();
  void onRotateDigitalLeftCallback();
  void onRotateDigitalRightCallback();
}

public interface PlayerInput
{
    void onMoveCallback(Vector2 value);
    void onJumpCallback(bool value);
    void onDashCallback();
}

public interface MenuInput {
  void onDirectionCallback(int direction);
  void onAcceptCallback();
  void onBackCallback();
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

                this.inputActions.Block.RotateAnalog.Enable();
                this.inputActions.Block.RotateAnalog.performed += context => i.onRotateAnalogCallback(context.ReadValue<float>());
                this.inputActions.Block.RotateAnalog.canceled += _ => i.onRotateAnalogReleaseCallback();

                this.inputActions.Block.RotateDigitalLeft.Enable();
                this.inputActions.Block.RotateDigitalLeft.performed += _ => i.onRotateDigitalLeftCallback();
                this.inputActions.Block.RotateDigitalRight.Enable();
                this.inputActions.Block.RotateDigitalRight.performed += _ => i.onRotateDigitalRightCallback();
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
            case MenuInput i:
                this.inputActions.Menu.Direction.Enable();
                this.inputActions.Menu.Direction.performed += context => i.onDirectionCallback((int)context.ReadValue<float>());

                this.inputActions.Menu.Accept.Enable();
                this.inputActions.Menu.Accept.performed += context => i.onAcceptCallback();

                this.inputActions.Menu.Back.Enable();
                this.inputActions.Menu.Back.performed += context => i.onBackCallback();
                break;
        }
  }

  public void Unsubscribe()
  {
    this.inputActions.Block.Disable();
    this.inputActions.Menu.Disable();
    this.inputActions.Player.Disable();
    this.inputActions = null;
  }
}