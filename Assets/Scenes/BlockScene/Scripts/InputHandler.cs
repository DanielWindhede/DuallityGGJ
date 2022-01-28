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