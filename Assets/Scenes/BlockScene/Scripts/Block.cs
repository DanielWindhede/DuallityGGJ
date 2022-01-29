using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Block : MonoBehaviour
{
    [SerializeField] private float speedUpMultiplier = 1.5f;
    [SerializeField] private float rotationSpeed = 45f;
    private int rotationDirection = 0;
    [SerializeField] private float toggleRotationAmount = 45f;
    private float rotationAngle;
    private List<Collider2D> collisions;
    private Collider2D collider;
    private Rigidbody2D body;
    [SerializeField] private float offsetSpeed = 0.5f;
    private float offset = 0;
    private BlockInputManager inputManager;
    private bool hasBeenPlaced = false;

    private bool IsPlaceable { get { return this.collisions.Count == 0; } }

    private BlockManager _blockManager;
    public BlockManager BlockManager {
        get { return this._blockManager; }
    }

    // Start is called before the first frame update
    void Start() {
        this.body = GetComponent<Rigidbody2D>();
        this.collider = GetComponent<Collider2D>();
        this.collisions = new List<Collider2D>();
        this._blockManager = transform.parent.GetComponent<BlockManager>();

        this.collider.isTrigger = true;
        this.inputManager = GlobalState.state.blockInputManager;
        this.inputManager.onAcceptClick += this.AcceptClick;
        this.inputManager.onRotationAnalog += this.AnalogRotation;
        this.inputManager.onRotateRelease += () => this.rotationDirection = 0;
        this.inputManager.onRotateLeftClick += () => this.rotationAngle -= toggleRotationAmount;
        this.inputManager.onRotateRightClick += () => this.rotationAngle += toggleRotationAmount;
    }

    void OnDestroy() {
        this.inputManager.onAcceptClick -= this.AcceptClick;
        this.inputManager.onRotationAnalog -= this.AnalogRotation;
        this.inputManager.onRotateRelease -= () => this.rotationDirection = 0;
        this.inputManager.onRotateLeftClick -= () => this.rotationAngle -= toggleRotationAmount;
        this.inputManager.onRotateRightClick -= () => this.rotationAngle += toggleRotationAmount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!this.hasBeenPlaced) {
            if (this.rotationDirection != 0) {
                this.rotationAngle += this.rotationSpeed * Time.fixedDeltaTime * this.rotationDirection;
            }
            var pos = this.inputManager.inputPosition;
            offset -= offsetSpeed * Time.fixedDeltaTime * (1 + this.speedUpMultiplier * this.inputManager.VerticalInput);
            transform.position = new Vector3(pos.x, pos.y + offset, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, this.rotationAngle);
        }
    }

    private void AnalogRotation(float value) {
        this.rotationDirection = -(int)value;
    }

    private void AcceptClick() {
        if (this.IsPlaceable) {
            this.hasBeenPlaced = true;
            this.collider.isTrigger = false;
            this.body.bodyType = RigidbodyType2D.Static;
            this.inputManager.onAcceptClick -= this.AcceptClick;
            this.BlockManager.EnableControls();
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (!this.hasBeenPlaced) {
            this.collisions.Add(col);
        }
        
    }
    private void OnTriggerExit2D(Collider2D col) {
        if (!this.hasBeenPlaced) {
            this.collisions.Remove(col);
        }
    }
}
