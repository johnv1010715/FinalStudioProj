using UnityEngine;
using System.Collections;

public class MenuItemUpgradeable : MonoBehaviour
{
    public Schematic[] schematics;
    int index;

    MenuController menuController;

    void Start()
    {
        menuController = FindObjectOfType<MenuController>();
    }

    public void Craft()
    {
        if (GameController.resources >= schematics[index].cost)
        {
            schematics[index].Craft(GameController.players[0]);
            if (index < schematics.Length - 1)
            {
                index++;
                menuController.Return();
                Display();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void Display()
    {
        schematics[index].Display(transform);
    }
}
