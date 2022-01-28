using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region StateMachine
    StateMachine<PlayerController> stateMachine = new StateMachine<PlayerController>();

    PlayerIdleState idleState;
    PlayerWalkingState walkingState;
    PlayerJumpingState jumpingState;
    PlayerFallingState fallingState;
    PlayerDashingState dashingState;
    #endregion



    #region Private variables
    Rigidbody rigidbody;
    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        idleState = new PlayerIdleState(this);
        walkingState = new PlayerWalkingState(this);
        jumpingState = new PlayerJumpingState(this);
        fallingState = new PlayerFallingState(this);
        dashingState = new PlayerDashingState(this);


        stateMachine.ChangeState(idleState);
    }
    
    void Start()
    {
        
    }
    
    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }



}

public class PlayerIdleState : State<PlayerController>
{

    public PlayerIdleState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {
        Debug.Log("swaped to idle");
        ////data.anim.SetBool("IsGrounded", true);
        //data.movementSlowdown = data.SlowdownDelay();
        //data.StartCoroutine(data.movementSlowdown);
    }

    public override void Execute()
    {
        //data.DetectJumpInput();
        //data.DetectDirectionallInput();
        ////krävs det i idleState?
        //data.FlipSprite();

        //if (data.changeX != 0 && data.grounded)
        //{
        //    data.playerMachine.ChangeState(data.playerWalkingState);
        //}

        //if (!data.grounded && data.myRigidbody.velocity.y < 0)
        //{
        //    data.playerMachine.ChangeState(data.playerFallingState);
        //}
        //else if (!data.grounded && data.myRigidbody.velocity.y > 0)
        //{
        //    data.playerMachine.ChangeState(data.playerJumpingState);
        //}
    }

    public override void FixedExecute()
    {
        //data.SlowDown();
    }

    public override void Exit()
    {
    //    data.GetComponent<CapsuleCollider2D>().sharedMaterial = data.movingPhysicsMaterial;
    //    data.StopCoroutine(data.movementSlowdown);
    }

}
public class PlayerWalkingState : State<PlayerController>
{

    public PlayerWalkingState(PlayerController data)
    {
        this.owner = data;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void FixedExecute()
    {

    }

    public override void Exit()
    {

    }
}

public class PlayerJumpingState : State<PlayerController>
{
    float jumpTimeCounter;

    public PlayerJumpingState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void FixedExecute()
    {

    }

    public override void Exit()
    {

    }

}

public class PlayerFallingState : State<PlayerController>
{

    public PlayerFallingState(PlayerController data)
    {
        this.owner = data;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void FixedExecute()
    {

    }

    public override void Exit()
    {

    }

}

public class PlayerDashingState : State<PlayerController>
{

    public PlayerDashingState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void FixedExecute()
    {

    }

    public override void Exit()
    {

    }
}




public class PlayerCopyState : State<PlayerController>
{

    public PlayerCopyState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {
        Debug.LogError("DO NOT USE THIS!!!! >:(");
    }

    public override void Execute()
    {

    }

    public override void FixedExecute()
    {

    }

    public override void Exit()
    {

    }
}
