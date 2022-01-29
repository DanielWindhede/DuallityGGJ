using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Block : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float offsetSpeed = 0.5f;
    private float offset = 0;
    private BlockInputManager inputManager;
    private bool hasBeenPlaced = false;

    // Start is called before the first frame update
    void Start() {
        this.body = GetComponent<Rigidbody2D>();
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
        print("click!");
        this.hasBeenPlaced = true;
        this.body.bodyType = RigidbodyType2D.Static;
        this.inputManager.onAcceptClick -= this.AcceptClick;
        transform.parent.GetComponent<BlockManager>().EnableControls();
    }
}
