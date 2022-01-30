using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public interface BlockInput
{
  void onDirectionCallback(Vector2 value);
  void onAcceptCallback();
  void onCycleCallback(float value);
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

    bool hasAssigedPlayer1 = false;
    bool hasAssigedPlayer2 = false;
    var p1Controls = new InputActions();
    var p2Controls = new InputActions();
    InputUser p1User;
    InputUser p2User;

    // Really poor implementation but we are super low on time:DDD
    foreach (var device in InputSystem.devices) {
      if (device.GetType() == typeof(UnityEngine.InputSystem.XInput.XInputControllerWindows)) {
        Debug.Log(device);
        if (!hasAssigedPlayer1) {
          // P1 gets gamepad.
          p1User = InputUser.PerformPairingWithDevice(device);
          p1User.AssociateActionsWithUser(p1Controls);
          p1User.ActivateControlScheme("Block");
          p1Controls.Enable();
          hasAssigedPlayer1 = true;
        }
        else if (!hasAssigedPlayer2) {
          // P2 gets gamepad.
          p2User = InputUser.PerformPairingWithDevice(device);
          p2User.AssociateActionsWithUser(p2Controls);
          p2User.ActivateControlScheme("Player");
          p2Controls.Enable();
          hasAssigedPlayer2 = true;
        }
      }
    }
  
    switch(input)
        {
            case BlockInput i:
                p1Controls.Block.Direction.Enable();
                p1Controls.Block.Direction.performed += context => i.onDirectionCallback(context.ReadValue<Vector2>());

                p1Controls.Block.Accept.Enable();
                p1Controls.Block.Accept.performed += _ => i.onAcceptCallback();

                p1Controls.Block.Cycle.Enable();
                p1Controls.Block.Cycle.performed += context => i.onCycleCallback(context.ReadValue<float>());

                p1Controls.Block.RotateAnalog.Enable();
                p1Controls.Block.RotateAnalog.performed += context => i.onRotateAnalogCallback(context.ReadValue<float>());
                p1Controls.Block.RotateAnalog.canceled += _ => i.onRotateAnalogReleaseCallback();

                p1Controls.Block.RotateDigitalLeft.Enable();
                p1Controls.Block.RotateDigitalLeft.performed += _ => i.onRotateDigitalLeftCallback();
                p1Controls.Block.RotateDigitalRight.Enable();
                p1Controls.Block.RotateDigitalRight.performed += _ => i.onRotateDigitalRightCallback();
                break;
            case PlayerInput i:
                p2Controls.Player.Move.Enable();
                p2Controls.Player.Move.performed += context => i.onMoveCallback(context.ReadValue<Vector2>());
                p2Controls.Player.Move.canceled += context => i.onMoveCallback(context.ReadValue<Vector2>());

                p2Controls.Player.Jump.Enable();
                p2Controls.Player.Jump.performed += context => i.onJumpCallback(true);
                p2Controls.Player.Jump.canceled += context => i.onJumpCallback(false);

                p2Controls.Player.Dash.Enable();
                p2Controls.Player.Dash.performed += context => i.onDashCallback();
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