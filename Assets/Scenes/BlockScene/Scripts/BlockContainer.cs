using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public int CurrentIndex {
        get { return this.currentIndex; }
    }

    public BlockContainer(int maxItemCount = 3) {
        this.maxItemCount = maxItemCount;
        this.heldBlocks = new List<Block>();
    }

    public Block GetCurrentBlock() {
        if (this.heldBlocks.Count - 1 >= this.currentIndex)
            return this.heldBlocks[this.currentIndex];
        return null;
    }

    public Block Pop(ContainerDirection direction) {
        if (this.heldBlocks.Count > 0) {
            var block = this.heldBlocks[this.GetCycledIndex(direction)];
            if (block) {
                this.Remove(block);
                return block;
            }
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
        Debug.Log(this.currentIndex);
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
