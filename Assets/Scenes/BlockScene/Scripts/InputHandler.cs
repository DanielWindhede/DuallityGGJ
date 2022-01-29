public interface BlockInput
{
  void onHorizontalCallback(float value);
  void onAcceptCallback();
  void onRotateAnalogCallback(float value);
  void onRotateAnalogReleaseCallback();
  void onRotateDigitalLeftCallback();
  void onRotateDigitalRightCallback();
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

            this.inputActions.Block.RotateAnalog.Enable();
            this.inputActions.Block.RotateAnalog.performed += context => i.onRotateAnalogCallback(context.ReadValue<float>());
            this.inputActions.Block.RotateAnalog.canceled += _ => i.onRotateAnalogReleaseCallback();

            this.inputActions.Block.RotateDigitalLeft.Enable();
            this.inputActions.Block.RotateDigitalLeft.performed += _ => i.onRotateDigitalLeftCallback();
            this.inputActions.Block.RotateDigitalRight.Enable();
            this.inputActions.Block.RotateDigitalRight.performed += _ => i.onRotateDigitalRightCallback();
            break;
    }
  }

  public void Unsubscribe()
  {
    this.inputActions.Block.Horizontal.Disable();
    this.inputActions = null;
  }
}