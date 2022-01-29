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

    public Block GetBlock(ContainerDirection direction) {
        return this.heldBlocks[this.GetCycledIndex(direction)];
    }

    public Block Pop(ContainerDirection direction) {
        var block = this.GetBlock(direction);
        this.Remove(block);
        this.Cycle(direction);
        return block;
    }

    public void Add(Block block) {
        if (this.heldBlocks.Count < maxItemCount) {
            Debug.Log(this.heldBlocks);
            this.heldBlocks.Add(block);
        }
    }

    public void Remove(Block block) {
        this.heldBlocks.Remove(block);
    }

    public void Cycle(ContainerDirection directon) {
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
            default:
                dir = 0;
                break;
        }

        int value = this.currentIndex + dir;
        if (value > this.heldBlocks.Count - 1)
            return 0;
        else if (value < 0)
            return this.heldBlocks.Count - 1;
        return value;
    }

    public void CycleRight() {
        this.Cycle(ContainerDirection.Right);
    }

    public void CycleLeft() {
        this.Cycle(ContainerDirection.Left);
    }
}

public class BlockManager : MonoBehaviour
{
    private bool enabledControls = true;
    [SerializeField] private bool loadPlaceholderResources = true;
    [SerializeField] private Camera camera;
    private BlockInputManager inputManager;

    [SerializeField] private BlockContainer blockContainer;
    [SerializeField] private List<GameObject> blockObjects;
    [SerializeField] private Transform uiBlockContainerObject;


    // Start is called before the first frame update
    void Start()
    {
        this.inputManager = GlobalState.state.blockInputManager;
        this.EnableControls();
        this.LoadResources();
        this.camera = Camera.main;

        int maxItemCount = 3;
        this.blockContainer = new BlockContainer(maxItemCount);
        for (int i = 0; i < maxItemCount; i++) {
            this.blockContainer.Add(this.blockObjects[i].GetComponent<Block>());
            GameObject imageGO = new GameObject();
            imageGO.AddComponent<Image>().sprite = this.blockObjects[i].GetComponent<Block>().Icon;

            Instantiate(imageGO, this.uiBlockContainerObject);
        }
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
            var obj = Instantiate(this.blockContainer.Pop(ContainerDirection.Current), this.inputManager.inputPosition, Quaternion.identity, this.transform);
            obj.name = "Block " + (transform.childCount - 1);
            this.DisableControls();
        }
    }

    private void Cycle(float value) {
        this.blockContainer.Cycle((ContainerDirection)(int)value);
    }

    private void OnDrawGizmos() {
        if (this.enabledControls && Application.isPlaying) {
            Gizmos.DrawSphere(this.inputManager.inputPosition, 0.5f);
        }
    }
}
