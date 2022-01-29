using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    private bool enabledControls = true;
    [SerializeField] private bool loadPlaceholderResources = true;
    [SerializeField] private Camera camera;
    private BlockInputManager inputManager;

    // TODO: block pool
    [SerializeField] private List<GameObject> blockObjects;


    // Start is called before the first frame update
    void Start()
    {
        this.inputManager = GlobalState.state.blockInputManager;
        this.EnableControls();
        this.LoadResources();
        this.camera = Camera.main;
    } 

    private void LoadResources() {
        string path = "Blocks\\" + (this.loadPlaceholderResources ? "Placeholders" : "") + "\\BlockCube";
        this.blockObjects = new List<GameObject>();

        var resources = Resources.LoadAll<GameObject>(path);
        foreach (GameObject block in resources) {
            this.blockObjects.Add(block);
        }
    }

    public void EnableControls() {
        this.inputManager.onAcceptClick += AcceptClick;
        this.enabledControls = true;
    }

    private void DisableControls() {
        this.inputManager.onAcceptClick -= AcceptClick;
        this.enabledControls = false;
    }

    private void AcceptClick() {
        if (this.enabledControls) {
            print("acceptingg");
            Instantiate(this.blockObjects[0], this.inputManager.inputPosition, Quaternion.identity, this.transform);
            this.DisableControls();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
    private void OnDrawGizmos() {
        if (this.enabledControls && Application.isPlaying) {
            Gizmos.DrawSphere(this.inputManager.inputPosition, 0.5f);
        }
    }
}
