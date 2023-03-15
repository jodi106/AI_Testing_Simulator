using Assets.Entities;
using UnityEngine;

public class MenuStart : MonoBehaviour
{
    private MenuController menuController;

    private MenuOptions menuOptions;

    // Start is called before the first frame update
    void Start()
    {
        GameObject menu = GameObject.Find("Menu");
        Debug.Log(menu);
        this.menuController = menu.GetComponent<MenuController>();
        Debug.Log(menuController);
        this.menuController.init(this.menuOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
