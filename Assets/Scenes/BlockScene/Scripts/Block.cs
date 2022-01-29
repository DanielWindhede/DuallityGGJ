using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Block : MonoBehaviour
{
    private List<Collider2D> collisions;
    private Collider2D collider;
    private Rigidbody2D body;
    [SerializeField] private float offsetSpeed = 0.5f;
    private float offset = 0;
    private BlockInputManager inputManager;
    private bool hasBeenPlaced = false;

    private bool IsPlaceable { get { return this.collisions.Count == 0; } }

    // Start is called before the first frame update
    void Start() {
        this.body = GetComponent<Rigidbody2D>();
        this.collider = GetComponent<Collider2D>();
        this.collisions = new List<Collider2D>();

        this.collider.isTrigger = true;
        this.inputManager = GlobalState.state.blockInputManager;
        this.inputManager.onAcceptClick += this.AcceptClick;
    }

    void OnDestroy() {
        this.inputManager.onAcceptClick -= this.AcceptClick;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!this.hasBeenPlaced) {
            var pos = this.inputManager.inputPosition;
            offset -= offsetSpeed * Time.fixedDeltaTime;
            transform.position = new Vector3(pos.x, pos.y + offset, 0);
        }
    }

    private void AcceptClick() {
        if (this.IsPlaceable) {
            this.hasBeenPlaced = true;
            this.collider.isTrigger = false;
            this.body.bodyType = RigidbodyType2D.Static;
            this.inputManager.onAcceptClick -= this.AcceptClick;
            transform.parent.GetComponent<BlockManager>().EnableControls();
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
