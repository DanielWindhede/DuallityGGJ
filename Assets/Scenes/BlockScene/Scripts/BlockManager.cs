using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static BlockContainer;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private bool _loadPlaceholderResources = true;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _selectedScaleMultiplier = 1.25f;
    [SerializeField] private BlockContainer _blockContainer;
    [SerializeField] private List<GameObject> _blockObjects;
    [SerializeField] private Transform _uiBlockContainerObject;
    [SerializeField] private Transform _uiTargetObject;
    [SerializeField] private int _maxItemCount = 3;
    [SerializeField] private float cooldownTime = 1f;
    public float blockOffsetSpeed = 0.5f;
    public float blockSpeedUpMultiplier = 1.5f;

    private bool _enabledControls = true;
    private BlockInputManager _inputManager;
    private Timer _cooldownTimer;
    private WeightedObjectPicker objPicker;
    private bool _useTimer;

    private void Start()
    {
        this._cooldownTimer = new Timer(cooldownTime);
        this._inputManager = GlobalState.state.blockInputManager;
        this.EnableControls();
        this.LoadResources();
        this._camera = Camera.main;

        var odds = _blockObjects.ToArray()
            .Select(obj => obj.GetComponent<Block>())
            .Select(block => block.SpawnWeight).ToArray();
        this.objPicker = new WeightedObjectPicker(_blockObjects.ToArray(), odds);

        this._blockContainer = new BlockContainer(_maxItemCount);
        for (int i = 0; i < _maxItemCount; i++) {
            var block = this.objPicker.GetRandomEntry().GetComponent<Block>();
            this.AddBlockToContainer(block);
        }
    
        this.CycleDirection(ContainerDirection.Right);
        print("Start");
    }

    private void AddBlockToContainer(Block block) {
        var iconObject = new GameObject(block.Icon.name);
        iconObject.transform.parent = this._uiBlockContainerObject;
        iconObject.transform.localScale = Vector3.one;
        iconObject.AddComponent<Image>().sprite = block.Icon;
        
        this._blockContainer.Add(block);
    }

    private void FixedUpdate() {
        if (this._useTimer) {
            this._cooldownTimer += Time.fixedDeltaTime;
            if (this._cooldownTimer.Done) {
                this._useTimer = false;
                this._cooldownTimer.Reset();
            }
        }
        this._uiTargetObject.transform.position = this._camera.WorldToScreenPoint(this._inputManager.inputPosition);
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
        string path = "Blocks\\" + (this._loadPlaceholderResources ? "Placeholders\\" : "");
        this._blockObjects = new List<GameObject>();

        var resources = Resources.LoadAll<GameObject>(path);
        print(resources);
        foreach (GameObject block in resources) {
            this._blockObjects.Add(block);
        }
    }


    private void DisableControls() {
        this._inputManager.onAcceptClick -= AcceptClick;
        this._inputManager.onCycle -= Cycle;
        this._enabledControls = false;
    }

    private void AcceptClick() {
        if (this._enabledControls && !this._useTimer) {
            int iconIndex = this._blockContainer.CurrentIndex;
            var block = this._blockContainer.Pop(ContainerDirection.Current);
            if (block)  {
                var iconObject = this._uiBlockContainerObject.GetChild(iconIndex);
                if (iconObject) {
                    Destroy(iconObject.gameObject);
                }
                
                var obj = Instantiate(block, this._inputManager.inputPosition, Quaternion.identity, this.transform);
                obj.name = "Block " + (transform.childCount - 1);
                this.DisableControls();
                this._useTimer = true;
                
                this.AddBlockToContainer(this.objPicker.GetRandomEntry().GetComponent<Block>());
                this.CycleDirection(ContainerDirection.Current, 1);
            }
        }
    }

    private void Cycle(float value) {
        this.CycleDirection((ContainerDirection)(int)value);
    }

    private void CycleDirection(ContainerDirection value, int offset = 0) {
        this._uiBlockContainerObject.GetChild(this._blockContainer.CurrentIndex + offset).transform.localScale = Vector3.one;
        this._blockContainer.CycleToDirection(value);
        this._uiBlockContainerObject.GetChild(this._blockContainer.CurrentIndex + offset).transform.localScale = Vector3.one * _selectedScaleMultiplier;
    }

    private void OnDrawGizmos() {
        if (this._enabledControls && Application.isPlaying) {
            Gizmos.DrawSphere(this._inputManager.inputPosition, 0.5f);
        }
    }
}
