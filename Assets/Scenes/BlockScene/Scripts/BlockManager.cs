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
        Debug.Log(dir);

        int value = this.currentIndex + dir;
        if (value > this.heldBlocks.Count - 1)
            return this.heldBlocks.Count - 1;
        else if (value < 0)
            return 0;
        return value;
    }

    public void CycleRight() {
        this.CycleToDirection(ContainerDirection.Right);
    }

    public void CycleLeft() {
        this.CycleToDirection(ContainerDirection.Left);
    }
}

public class BlockManager : MonoBehaviour
{
    private bool enabledControls = true;
    [SerializeField] private bool loadPlaceholderResources = true;
    [SerializeField] private Camera camera;
    private BlockInputManager inputManager;

    [SerializeField] private float selectedScaleMultiplier = 1.25f;

    [SerializeField] private BlockContainer blockContainer;
    [SerializeField] private List<GameObject> blockObjects;
    [SerializeField] private Transform uiBlockContainerObject;

    int maxItemCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        this.inputManager = GlobalState.state.blockInputManager;
        this.EnableControls();
        this.LoadResources();
        this.camera = Camera.main;

        this.blockContainer = new BlockContainer(maxItemCount);
        for (int i = 0; i < maxItemCount; i++) {
            var block = this.blockObjects[i].GetComponent<Block>();
            this.blockContainer.Add(block);
            GameObject imageGO = new GameObject();
            imageGO.AddComponent<Image>().sprite = this.blockObjects[i].GetComponent<Block>().Icon;

            var go = Instantiate(imageGO, this.uiBlockContainerObject);
            block.iconGameObject = go;
        }
        this.CycleDirection(ContainerDirection.Right);
    } 

    private void LoadResources() {
        string path = "Blocks\\" + (this.loadPlaceholderResources ? "Placeholders" : "") + "\\";
        this.blockObjects = new List<GameObject>();

        var resources = Resources.LoadAll<GameObject>(path);
        print(resources);
        foreach (GameObject block in resources) {
            this.blockObjects.Add(block);
        }
    }

    public void EnableControls() {
        this.inputManager.onAcceptClick += AcceptClick;
        this.inputManager.onCycle += Cycle;
        this.enabledControls = true;
    }

    public void RemoveFromCollection(Block block) {
        if (this.enabledControls) {
            this.EnableControls();
        }
        this.blockObjects.Remove(block.gameObject);
    }

    private void DisableControls() {
        this.inputManager.onAcceptClick -= AcceptClick;
        this.enabledControls = false;
    }

    private void AcceptClick() {
        if (this.enabledControls) {
            var block = this.blockContainer.Pop(ContainerDirection.Current);
            if (block)  {
                if (block.iconGameObject)
                    Destroy(block.iconGameObject);
                
                var obj = Instantiate(block, this.inputManager.inputPosition, Quaternion.identity, this.transform);
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
        this.blockContainer.GetCurrentBlock().iconGameObject.transform.localScale = Vector3.one;
        this.blockContainer.CycleToDirection(value);
        this.blockContainer.GetCurrentBlock().iconGameObject.transform.localScale = Vector3.one * selectedScaleMultiplier;
    }

    private void OnDrawGizmos() {
        if (this.enabledControls && Application.isPlaying) {
            Gizmos.DrawSphere(this.inputManager.inputPosition, 0.5f);
        }
    }
}
