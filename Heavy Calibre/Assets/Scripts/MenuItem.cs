using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public Schematic schematic;

    [HideInInspector] public MenuController menuController;

    public void Start()
    {
        menuController = FindObjectOfType<MenuController>();
    }

    public void Craft()
    {
        if (GameController.resources >= schematic.cost)
        {
            schematic.Craft(GameController.players[0]);
            menuController.Return();
            if (schematic.singleUse)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Display()
    {
        schematic.Display(transform);
    }
}