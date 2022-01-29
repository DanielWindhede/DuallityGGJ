 using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BlockContainer;

[System.Serializable]
public class BlockContainer {
    public enum ContainerDirection {
        Left = -1,
        Current = 0,
        Right = 1,
    }

    private int maxItemCount;
    private int currentIndex;
    private List<Block> heldBlocks;

    public BlockContainer(int maxItemCount = 3) {
        this.maxItemCount = maxItemCount;
        this.heldBlocks = new List<Block>();
    }

    public Block GetCurrentBlock() {
        return this.heldBlocks[this.currentIndex];
    }

    public Block Pop(ContainerDirection direction) {
        var block = this.heldBlocks[this.GetCycledIndex(direction)];
        if (block) {
            this.Remove(block);
            this.CycleLeft();
            return block;
        }
        return null;
    }

    public void Add(Block block) {
        if (this.heldBlocks.Count < maxItemCount) {
            this.heldBlocks.Add(block);
        }
    }

    public void Remove(Block block) {
        this.heldBlocks.Remove(block);
    }

    public void CycleToDirection(ContainerDirection directon) {
        this.currentIndex = GetCycledIndex(directon);
    }

    public void CycleRight() {
        this.CycleToDirection(ContainerDirection.Right);
    }

    public void CycleLeft() {
        this.CycleToDirection(ContainerDirection.Left);
    }

    private int GetCycledIndex(ContainerDirection directon) {
        int dir = 0;
        switch (directon) {
            case ContainerDirection.Left:
                dir = -1;
                break;
            case ContainerDirection.Right:
                dir = 1;
                break;
        }

        int value = this.currentIndex + dir;
        if (value > this.heldBlocks.Count - 1)
            return this.heldBlocks.Count - 1;
        else if (value < 0)
            return 0;
        return value;
    }
}

public class BlockManager : MonoBehaviour
{
    [SerializeField] private bool _loadPlaceholderResources = true;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _selectedScaleMultiplier = 1.25f;
    [SerializeField] private BlockContainer _blockContainer;
    [SerializeField] private List<GameObject> _blockObjects;
    [SerializeField] private Transform _uiBlockContainerObject;
    [SerializeField] private int _maxItemCount = 3;

    private bool _enabledControls = true;
    private BlockInputManager _inputManager;

    private void Start()
    {
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
        if (this._enabledControls) {
            var block = this._blockContainer.Pop(ContainerDirection.Current);
            if (block)  {
                if (block.iconGameObject)
                    Destroy(block.iconGameObject);
                
                var obj = Instantiate(block, this._inputManager.inputPosition, Quaternion.identity, this.transform);
                obj.name = "Block " + (transform.childCount - 1);
                this.CycleDirection(ContainerDirection.Current);
                this.DisableControls();
            }
        }
    }

    private void Cycle(float value) {
        this.CycleDirection((ContainerDirection)(int)value);
    }

    private void CycleDirection(ContainerDirection value) {
        this._blockContainer.GetCurrentBlock().iconGameObject.transform.localScale = Vector3.one;
        this._blockContainer.CycleToDirection(value);
        this._blockContainer.GetCurrentBlock().iconGameObject.transform.localScale = Vector3.one * _selectedScaleMultiplier;
    }

    private void OnDrawGizmos() {
        if (this._enabledControls && Application.isPlaying) {
            Gizmos.DrawSphere(this._inputManager.inputPosition, 0.5f);
        }
    }
}
