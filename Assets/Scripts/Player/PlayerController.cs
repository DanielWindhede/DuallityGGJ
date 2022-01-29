using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, PlayerInput
{
    #region StateMachine
    public StateMachine<PlayerController> stateMachine = new StateMachine<PlayerController>();

    public PlayerIdleState idleState;
    public PlayerWalkingState walkingState;
    public PlayerJumpingState jumpingState;
    public PlayerFallingState fallingState;
    public PlayerDashingState dashingState;
    #endregion

    [Header ("Ground Check")]
    [SerializeField] float groundCheckRadius;
    [SerializeField] Transform groundCheckPosition;
    [SerializeField] LayerMask groundaLayermask;

    [Header("Movement")]
    public float horizontalMovementSpeed;
    [Range(0, .3f)] public float movementSmoothing = .05f;


    [HideInInspector] public bool grounded;
    [HideInInspector] public Vector3 zeroVector = Vector3.zero;
    [HideInInspector] public Rigidbody rigidbody;


    #region Private variables
    InputHandler<PlayerInput> inputHandler;
    #endregion

    #region Input Variables
    [HideInInspector] public Vector2 inputDirection;
    [HideInInspector] public bool inputJump;


    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        idleState = new PlayerIdleState(this);
        walkingState = new PlayerWalkingState(this);
        jumpingState = new PlayerJumpingState(this);
        fallingState = new PlayerFallingState(this);
        dashingState = new PlayerDashingState(this);

        stateMachine.ChangeState(idleState);

        this.inputHandler = new InputHandler<PlayerInput>();
        this.inputHandler.Subscribe(this);
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        stateMachine.CurrentlyRunningState.Execute();
        grounded = Physics.OverlapSphere(groundCheckPosition.position, groundCheckRadius, groundaLayermask).Length > 0;
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void onMoveCallback(Vector2 value)
    {
        inputDirection = value;
        //TODO: lägg till fastfall
    }

    public void onJumpCallback(bool value)
    {
        inputJump = value;
        Debug.Log("JUMP!!! " + inputJump);
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
    }

    public override void Execute()
    {
        if (owner.grounded && owner.inputJump)
        {
            owner.stateMachine.ChangeState(owner.jumpingState);
        }
        else if (!owner.grounded)
        {
            owner.stateMachine.ChangeState(owner.fallingState);
        }
        else if (Mathf.Abs(owner.inputDirection.x) > 0.1f)
        {
            owner.stateMachine.ChangeState(owner.walkingState);
        }
    }

    public override void FixedExecute()
    {
        //slow down
        owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, Vector3.zero, ref owner.zeroVector, owner.movementSmoothing);
    
    }

    public override void Exit()
    {
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
        Debug.Log("IM WALKING HERE!!!");
    }

    public override void Execute()
    {
        if (owner.grounded && owner.inputJump)
        {
            owner.stateMachine.ChangeState(owner.jumpingState);
        }
        else if (!owner.grounded)
        {
            owner.stateMachine.ChangeState(owner.fallingState);
        }
        else if (Mathf.Abs(owner.inputDirection.x) < 0.1f)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void FixedExecute()
    {
        //walk (no slope)
        Vector3 targetVelocity = new Vector3(owner.inputDirection.x * owner.horizontalMovementSpeed, 0f, 0f);
        owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, targetVelocity, ref owner.zeroVector, owner.movementSmoothing);

        //TODO: add slope :)

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
        Debug.Log("YIPIEEE");
    }

    public override void Execute()
    {
        if (owner.grounded)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
        //TODO: fixa så man går in i falling efter en tid eller om man släpper (maio)
    }

    public override void FixedExecute()
    {
        //jump
        //air strafe
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
        Debug.Log("waaaaaaaaaaa");
    }

    public override void Execute()
    {
        if (owner.grounded)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void FixedExecute()
    {
        //fall
        //air strafe
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
