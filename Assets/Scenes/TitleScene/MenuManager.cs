using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour, MenuInput
{
    [SerializeField] private List<MenuItem> menuItems;
    private InputHandler<MenuInput> inputHandler;
    private int _selectedIndex = 0;
    public int SelectedIndex {
        get { return this._selectedIndex; }
        set {
            this.menuItems[this.SelectedIndex].GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;

            if (value > this.menuItems.Count - 1)
                this._selectedIndex = this.menuItems.Count - 1;
            else if (value < 0)
                this._selectedIndex = 0;
            else
                this._selectedIndex = value;

            this.menuItems[this.SelectedIndex].GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        this.inputHandler = new InputHandler<MenuInput>();
        this.inputHandler.Subscribe(this);
        this.SelectedIndex = 0;
    }

    public void OnDestroy() {
        this.inputHandler.Unsubscribe();
    }

    public void onDirectionCallback(int direction)
    {
        this.SelectedIndex += direction;
    }

    public void onAcceptCallback()
    {
        this.menuItems[this.SelectedIndex].PerformAction();
    }

    public void onBackCallback()
    {
        print("back!");
    }
}
