using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BlockContainer;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private bool _loadPlaceholderResources = true;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _selectedScaleMultiplier = 1.25f;
    [SerializeField] private BlockContainer _blockContainer;
    [SerializeField] private List<GameObject> _blockObjects;
    [SerializeField] private Transform _uiBlockContainerObject;
    [SerializeField] private int _maxItemCount = 3;
    [SerializeField] private float cooldownTime = 1f;

    private bool _enabledControls = true;
    private BlockInputManager _inputManager;
    private Timer _cooldownTimer;
    private bool _useTimer;

    private void Start()
    {
        this._cooldownTimer = new Timer(cooldownTime);
        this._inputManager = GlobalState.state.blockInputManager;
        this.EnableControls();
        this.LoadResources();
        this._camera = Camera.main;

        this._blockContainer = new BlockContainer(_maxItemCount);
        for (int i = 0; i < _maxItemCount; i++) {
            var block = this._blockObjects[i].GetComponent<Block>();
            this._blockContainer.Add(block);
            GameObject imageGO = new GameObject();
            imageGO.AddComponent<Image>().sprite = this._blockObjects[i].GetComponent<Block>().Icon;

            var go = Instantiate(imageGO, this._uiBlockContainerObject);
            block.iconGameObject = go;
        }
        this.CycleDirection(ContainerDirection.Right);
    }

    private void FixedUpdate() {
        if (this._useTimer) {
            this._cooldownTimer += Time.fixedDeltaTime;
            if (this._cooldownTimer.Done) {
                this._useTimer = false;
                this._cooldownTimer.Reset();
            }
        }
    }

    public void EnableControls() {
        this._inputManager.onAcceptClick += AcceptClick;
        this._inputManager.onCycle += Cycle;
        this._enabledControls = true;
    }

    public void RemoveFromCollection(Block block) {
        if (this._enabledControls) {
            this.EnableControls();
        }
        this._blockObjects.Remove(block.gameObject);
    }

    private void LoadResources() {
        string path = "Blocks\\" + (this._loadPlaceholderResources ? "Placeholders" : "") + "\\";
        this._blockObjects = new List<GameObject>();

        var resources = Resources.LoadAll<GameObject>(path);
        print(resources);
        foreach (GameObject block in resources) {
            this._blockObjects.Add(block);
        }
    }


    private void DisableControls() {
        this._inputManager.onAcceptClick -= AcceptClick;
        this._enabledControls = false;
    }

    private void AcceptClick() {
        if (this._enabledControls && !this._useTimer) {
            var block = this._blockContainer.Pop(ContainerDirection.Current);
            if (block)  {
                if (block.iconGameObject)
                    Destroy(block.iconGameObject);
                
                var obj = Instantiate(block, this._inputManager.inputPosition, Quaternion.identity, this.transform);
                obj.name = "Block " + (transform.childCount - 1);
                this.CycleDirection(ContainerDirection.Current);
                this.DisableControls();
                this._useTimer = true;
            }
        }
    }

    private void Cycle(float value) {
        this.CycleDirection((ContainerDirection)(int)value);
    }

    private void CycleDirection(ContainerDirection value) {
        if (this._blockContainer.GetCurrentBlock()) {
            this._blockContainer.GetCurrentBlock().iconGameObject.transform.localScale = Vector3.one;
            this._blockContainer.CycleToDirection(value);

            if (this._blockContainer.GetCurrentBlock()) {
                this._blockContainer.GetCurrentBlock().iconGameObject.transform.localScale = Vector3.one * _selectedScaleMultiplier;
            }
        }
    }

    private void OnDrawGizmos() {
        if (this._enabledControls && Application.isPlaying) {
            Gizmos.DrawSphere(this._inputManager.inputPosition, 0.5f);
        }
    }
}
