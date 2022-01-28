using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private bool loadPlaceholderResources = true;
    [SerializeField] private Camera camera;
    
    // TODO: block pool
    [SerializeField] private List<GameObject> blockObjects;


    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
