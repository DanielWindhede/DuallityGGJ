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
    public PlayerSlidingState slidingState;
    #endregion

    [Header("Ground Check")]
    public float groundCheckRadius;
    public Transform groundCheckPosition;
    public LayerMask groundaLayermask;
    public float playerWidth;
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

    [Header("Dashing Variables")]
    public float dashTime;
    public float dashSpeedIncrease;
    public float dashMinSpeed;


    [HideInInspector] public bool grounded;
    [HideInInspector] public bool canDash = true;
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position - new Vector3(playerWidth, 0f, 0f), 0.1f);
        Gizmos.DrawSphere(transform.position + new Vector3(playerWidth, 0f, 0f), 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position - new Vector3(playerWidth, 0f, 0f), Vector3.down * groundRaycastLength);
        Gizmos.DrawRay(transform.position, Vector3.down * groundRaycastLength);
        Gizmos.DrawRay(transform.position + new Vector3(playerWidth, 0f, 0f), Vector3.down * groundRaycastLength);
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        idleState = new PlayerIdleState(this);
        walkingState = new PlayerWalkingState(this);
        jumpingState = new PlayerJumpingState(this);
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
    }

    public void onDashCallback()
    {
        if (canDash)
        {
            stateMachine.ChangeState(dashingState);
        }
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
        else if (Mathf.Abs(angleBetweenVectors) > owner.maxSlopeAngle)
        {
            owner.stateMachine.ChangeState(owner.slidingState);
        }
    }

    public override void FixedExecute()
    {
        //slow down
        owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, Vector3.zero, ref owner.zeroVector, owner.groundedMovementSmoothing);

        RaycastHit2D hit1 = Physics2D.Raycast(owner.transform.position - new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit2 = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit3 = Physics2D.Raycast(owner.transform.position + new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);

        Vector2 sumOfNormals = Vector2.zero;


        if (hit1 != null)
            sumOfNormals += hit1.normal;
        if (hit2 != null)
            sumOfNormals += hit2.normal;
        if (hit3 != null)
            sumOfNormals += hit3.normal;

        //Vector2 debugSumOfHitPoint = new Vector2(100000,100000);

        //if (hit1 != null)
        //{
        //    sumOfNormals += hit1.normal;
        //    debugSumOfHitPoint = hit1.point;
        //}
        //if (hit2 != null)
        //{
        //    sumOfNormals += hit2.normal;

        //    if (Vector2.Distance(debugSumOfHitPoint, new Vector2(100000, 100000)) < 0.1f)
        //    {
        //        debugSumOfHitPoint = hit2.point;
        //    }
        //    else
        //    {
        //        debugSumOfHitPoint += (hit2.point - debugSumOfHitPoint) / 2;
        //    }
        //}
        //if (hit3 != null)
        //{
        //    sumOfNormals += hit3.normal;

        //    if (Vector2.Distance(debugSumOfHitPoint, new Vector2(100000, 100000)) < 0.1f)
        //    {
        //        debugSumOfHitPoint = hit3.point;
        //    }
        //    else
        //    {
        //        debugSumOfHitPoint += (hit3.point - debugSumOfHitPoint) / 2;
        //    }
        //}

        sumOfNormals.Normalize();

        //Debug.DrawRay(debugSumOfHitPoint, sumOfNormals * 3f);


        angleBetweenVectors = Vector3.SignedAngle(Vector3.up, sumOfNormals, Vector3.forward);
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
        RaycastHit2D hit1 = Physics2D.Raycast(owner.transform.position - new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit2 = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit3 = Physics2D.Raycast(owner.transform.position + new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);

        Vector2 sumOfNormals = Vector2.zero;


        if (hit1 != null)
            sumOfNormals += hit1.normal;
        if (hit2 != null)
            sumOfNormals += hit2.normal;
        if (hit3 != null)
            sumOfNormals += hit3.normal;

        sumOfNormals.Normalize();

        Vector3 targetDirection = new Vector3(owner.inputDirection.normalized.x, 0f, 0f);

        angleBetweenVectors = Vector3.SignedAngle(Vector3.up, sumOfNormals, Vector3.forward);

        targetDirection = Quaternion.AngleAxis(angleBetweenVectors, Vector3.forward) * targetDirection;

        Vector3 targetVelocity = targetDirection * Mathf.Abs(owner.inputDirection.x) * owner.horizontalMovementSpeed;

        owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, targetVelocity, ref owner.zeroVector, owner.groundedMovementSmoothing);
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
        
        RaycastHit2D hit1 = Physics2D.Raycast(owner.transform.position - new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit2 = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit3 = Physics2D.Raycast(owner.transform.position + new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);

        Vector2 sumOfNormals = Vector2.zero;


        if (hit1 != null)
            sumOfNormals += hit1.normal;
        if (hit2 != null)
            sumOfNormals += hit2.normal;
        if (hit3 != null)
            sumOfNormals += hit3.normal;

        sumOfNormals.Normalize();

        owner.rigidbody.velocity = new Vector2(owner.rigidbody.velocity.x, 0f) + sumOfNormals * owner.jumpSpeed;
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
    }

    public override void FixedExecute()
    {
        owner.AerialStrafing();

        owner.rigidbody.velocity = new Vector3(owner.rigidbody.velocity.x, owner.jumpSpeed, 0f);
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
        owner.AerialStrafing();

        owner.rigidbody.velocity -= new Vector2(0f, owner.fallForce);
    }

    public override void Exit()
    {
        owner.canDash = true;
    }

}

public class PlayerDashingState : State<PlayerController>
{
    Timer dashTimer;
    Vector2 dashDirection;
    float dashSpeed;

    public PlayerDashingState(PlayerController owner)
    {
        this.owner = owner;
    }

    public override void Enter()
    {
        dashTimer = new Timer(owner.dashTime);
        owner.canDash = false;
        dashDirection = (owner.inputDirection.magnitude > 0.03f) ? owner.inputDirection.normalized : Vector2.up;
        dashSpeed = (owner.rigidbody.velocity.magnitude + owner.dashSpeedIncrease > owner.dashMinSpeed) ? (owner.rigidbody.velocity.magnitude + owner.dashSpeedIncrease) : owner.dashMinSpeed;

        Debug.Log("Dashing");
    }

    public override void Execute()
    {
        dashTimer += Time.deltaTime;
        if (dashTimer.Done)
        {
            owner.stateMachine.ChangeState(owner.fallingState);
        }
    }

    public override void FixedExecute()
    {
        owner.rigidbody.velocity = dashDirection * dashSpeed;
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
            owner.stateMachine.ChangeState(owner.jumpingState);
        }
        else if (!owner.grounded)
        {
            owner.stateMachine.ChangeState(owner.fallingState);
        }
        else if (Mathf.Abs(angleBetweenVectors) < owner.maxSlopeAngle)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void FixedExecute()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(owner.transform.position - new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit2 = Physics2D.Raycast(owner.transform.position, -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);
        RaycastHit2D hit3 = Physics2D.Raycast(owner.transform.position + new Vector3(owner.playerWidth, 0f, 0f), -owner.transform.up, owner.groundRaycastLength, owner.groundaLayermask);

        Vector2 sumOfNormals = Vector2.zero;


        if (hit1 != null)
            sumOfNormals += hit1.normal;
        if (hit2 != null)
            sumOfNormals += hit2.normal;
        if (hit3 != null)
            sumOfNormals += hit3.normal;

        sumOfNormals.Normalize();

        angleBetweenVectors = Vector3.SignedAngle(Vector3.up, sumOfNormals, Vector3.forward);
        Vector3 slideDiraction = Quaternion.AngleAxis(90 * Mathf.Sign(angleBetweenVectors), Vector3.forward) * sumOfNormals;

        Vector3 targetVelocity = slideDiraction * owner.slideSpeed;//if u want u can add angle fast slider

        owner.rigidbody.velocity = Vector3.SmoothDamp(owner.rigidbody.velocity, targetVelocity, ref owner.zeroVector, owner.groundedMovementSmoothing);
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
