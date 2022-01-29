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
    public PlayerPreJumpState preJumpState;
    public PlayerFallingState fallingState;
    public PlayerDashingState dashingState;
    public PlayerSlidingState slidingState;
    #endregion

    [Header("Ground Check")]
    public float groundCheckRadius;
    public Transform groundCheckPosition;
    public LayerMask groundaLayermask;
    public float groundRaycastLength;

    [Header("Horizontal Movement")]
    public float horizontalMovementSpeed;
    [Range(0, .3f)] public float groundedMovementSmoothing = .05f;
    [Range(0, .3f)] public float aerialMovementSmoothing = .05f;
    public float maxSlopeAngle;
    public float slideSpeed;

    [Header("Jumping Variables")]
    public float maxJumpTime;
    public float jumpSpeed;
    public float fallForce;


    [HideInInspector] public bool grounded;
    [HideInInspector] public Vector3 zeroVector = Vector3.zero;
    [HideInInspector] public Rigidbody2D rigidbody;


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
        rigidbody = GetComponent<Rigidbody2D>();

        idleState = new PlayerIdleState(this);
        walkingState = new PlayerWalkingState(this);
        jumpingState = new PlayerJumpingState(this);
        preJumpState = new PlayerPreJumpState(this);
        fallingState = new PlayerFallingState(this);
        dashingState = new PlayerDashingState(this);
        slidingState = new PlayerSlidingState(this);

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
        grounded = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundaLayermask);
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void AerialStrafing()
    {
        Vector3 targetVelocity = new Vector3(inputDirection.x * horizontalMovementSpeed, rigidbody.velocity.y, 0f);
        rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref zeroVector, aerialMovementSmoothing);
    }

    #region Input Callbacks
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

    public void onDashCallback(bool value)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}

public class PlayerIdleState : State<PlayerController>
{
    float angleBetweenVectors;
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
            owner.stateMachine.ChangeState(owner.preJumpState);
        }
        else if (!owner.grounded)
        {
            owner.stateMachine.ChangeState(owner.fallingState);
        }
        else if (Mathf.Abs(owner.inputDirection.x) > 0.1f)
        {
            owner.stateMachine.ChangeState(owner.walkingState);
        }
        else if (Mathf.Abs(angleBetweenVectors) > owner.maxSlopeAngle)
        {
            owner.stateMachine.ChangeState(owner.slidingState);
        }
    }

    public override void FixedExecute()
    {
        //slow down
        owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, Vector3.zero, ref owner.zeroVector, owner.groundedMovementSmoothing);

        RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        angleBetweenVectors = Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.forward);

    }

    public override void Exit()
    {
    }

}

public class PlayerWalkingState : State<PlayerController>
{
    float angleBetweenVectors;

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
        if (Mathf.Abs(angleBetweenVectors) > owner.maxSlopeAngle)
        {
            owner.stateMachine.ChangeState(owner.slidingState);
        }
        else if (owner.grounded && owner.inputJump)
        {
            owner.stateMachine.ChangeState(owner.preJumpState);
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

        RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);


        Vector3 targetDirection = new Vector3(owner.inputDirection.normalized.x, 0f, 0f);

        if (hit.collider != null)
        {
            Debug.DrawRay(hit.point, hit.normal.normalized * 3f);

            angleBetweenVectors = Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.forward);

            targetDirection = Quaternion.AngleAxis(angleBetweenVectors, Vector3.forward) * targetDirection;

            Debug.DrawRay(owner.transform.position, targetDirection * 3f);

            Vector3 targetVelocity = targetDirection * Mathf.Abs(owner.inputDirection.x) * owner.horizontalMovementSpeed;

            owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, targetVelocity, ref owner.zeroVector, owner.groundedMovementSmoothing);
        }
        else
        {
            //hämta från ground check och ta normalizerade summan av alla normaler 

            //owner.GetComponent<Collider2D>().GetContacts(Physics2D.OverlapCircleAll(owner.groundCheckPosition.position, owner.groundCheckRadius, owner.groundaLayermask));

            //Collision2D[] currentGroundColliders = Physics2D.OverlapCircleAll(owner.groundCheckPosition.position, owner.groundCheckRadius, owner.groundaLayermask);


            Vector3 targetVelocity = targetDirection * Mathf.Abs(owner.inputDirection.x) * owner.horizontalMovementSpeed;

            owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, targetVelocity, ref owner.zeroVector, owner.groundedMovementSmoothing);
        }

    }

    public override void Exit()
    {

    }
}

public class PlayerJumpingState : State<PlayerController>
{
    Timer jumpTimer;

    public PlayerJumpingState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {
        Debug.Log("YIPIEEE");
        jumpTimer = new Timer(owner.maxJumpTime);
    }

    public override void Execute()
    {
        jumpTimer += Time.deltaTime;
        if (jumpTimer.Done || !owner.inputJump)
        {
            owner.stateMachine.ChangeState(owner.fallingState);
        }
        else if (owner.rigidbody.velocity.y < 0.01f)
        {
            owner.stateMachine.ChangeState(owner.fallingState);
        }
        //TODO: fixa så man går in i falling efter en tid eller om man släpper (maio)
    }

    public override void FixedExecute()
    {
        //air strafe
        owner.AerialStrafing();
        //jump

        owner.rigidbody.velocity = new Vector3(owner.rigidbody.velocity.x, owner.jumpSpeed, 0f);

    }

    public override void Exit()
    {
    }

}

public class PlayerPreJumpState : State<PlayerController>
{

    public PlayerPreJumpState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {
        Debug.Log("we bout to jump");
    }

    public override void Execute()
    {

    }

    public override void FixedExecute()
    {
        RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);

        if (hit.collider != null)
        {

            owner.rigidbody.velocity = hit.normal * owner.jumpSpeed;
        }
        else
        {
            owner.rigidbody.velocity = new Vector3(owner.rigidbody.velocity.x, owner.jumpSpeed, 0f);
        }

        owner.stateMachine.ChangeState(owner.jumpingState);
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
        //air strafe
        owner.AerialStrafing();
        //fall
        owner.rigidbody.velocity -= new Vector2(0f, owner.fallForce);

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

public class PlayerSlidingState : State<PlayerController>
{
    float angleBetweenVectors;

    public PlayerSlidingState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {
        Debug.Log("slide");
    }

    public override void Execute()
    {
        if (owner.grounded && owner.inputJump)
        {
            owner.stateMachine.ChangeState(owner.preJumpState);
        }
        else if (Mathf.Abs(angleBetweenVectors) < owner.maxSlopeAngle)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void FixedExecute()
    {
        RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);

        angleBetweenVectors = Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.forward);
        Vector3 slideDiraction = Quaternion.AngleAxis(90 * Mathf.Sign(angleBetweenVectors), Vector3.forward) * hit.normal;


        Vector3 targetVelocity = slideDiraction * owner.slideSpeed;//if u want u can add angle fast slider

        owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, targetVelocity, ref owner.zeroVector, owner.groundedMovementSmoothing);

        Debug.Log("abs value " + Mathf.Abs(angleBetweenVectors));


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
