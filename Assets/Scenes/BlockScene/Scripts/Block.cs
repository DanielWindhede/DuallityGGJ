using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private float _spawnWeight = 10;
    private int rotationDirection = 0;
    private float rotationAngle;
    private List<Collider2D> collisions;
    private Collider2D _collider;
    private Rigidbody2D body;
    private float offset = 0;
    private BlockInputManager inputManager;
    private bool hasBeenPlaced = false;

    private bool IsPlaceable { get { return this.collisions.Count == 0; } }

    private BlockManager _blockManager;
    public BlockManager BlockManager {
        get { return this._blockManager; }
    }

    public Sprite Icon {
        get { return this.icon; }
    }

    public float SpawnWeight {
        get { return this._spawnWeight; }
    }

    // Start is called before the first frame update
    void Start() {
        this.body = GetComponent<Rigidbody2D>();
        this._collider = GetComponent<Collider2D>();
        if (!this._collider) {
            this._collider = GetComponentInChildren<Collider2D>();
        }
        this.collisions = new List<Collider2D>();
        this._blockManager = transform.parent.GetComponent<BlockManager>();

        this._collider.isTrigger = true;
        this.inputManager = GlobalState.state.blockInputManager;
        this.inputManager.onAcceptClick += this.AcceptClick;
        this.inputManager.onRotationAnalog += this.AnalogRotation;
        this.inputManager.onRotateRelease += () => this.rotationDirection = 0;
        this.inputManager.onRotateLeftClick += () => this.rotationAngle -= this.BlockManager.blockToggleRotationAmount;
        this.inputManager.onRotateRightClick += () => this.rotationAngle += this.BlockManager.blockToggleRotationAmount;
    }

    void OnDestroy() {
        this.inputManager.onAcceptClick -= this.AcceptClick;
        this.inputManager.onRotationAnalog -= this.AnalogRotation;
        this.inputManager.onRotateRelease -= () => this.rotationDirection = 0;
        this.inputManager.onRotateLeftClick -= () => this.rotationAngle -= this.BlockManager.blockToggleRotationAmount;
        this.inputManager.onRotateRightClick -= () => this.rotationAngle += this.BlockManager.blockToggleRotationAmount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!this.hasBeenPlaced) {
            if (this.rotationDirection != 0) {
                this.rotationAngle += this.BlockManager.blockRotationSpeed * Time.fixedDeltaTime * this.rotationDirection;
            }
            var pos = this.inputManager.inputPosition;
            offset -= this.BlockManager.blockOffsetSpeed * Time.fixedDeltaTime * (1 + this.BlockManager.blockSpeedUpMultiplier * this.inputManager.VerticalInput);
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
            this._collider.isTrigger = false;
            this.body.bodyType = RigidbodyType2D.Static;
            this.inputManager.onAcceptClick -= this.AcceptClick;
            this.BlockManager.EnableControls();
            this.BlockManager.PlayPlacementSound();
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (!this.hasBeenPlaced) {
            print("on trigger!!");
            this.collisions.Add(col);
        }
        
    }
    private void OnTriggerExit2D(Collider2D col) {
        if (!this.hasBeenPlaced) {
            this.collisions.Remove(col);
        }
    }
}
