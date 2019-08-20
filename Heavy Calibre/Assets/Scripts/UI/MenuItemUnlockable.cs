using UnityEngine;
using System.Collections;

public class MenuItemUnlockable : MonoBehaviour
{
    [HideInInspector] public MenuItem unlockable;

    public void Unlock()
    {
        if (GameController.tokens >= unlockable.schematic.unlockCost)
        {
            Object.FindObjectOfType<GameController>().AddTokens(-unlockable.schematic.unlockCost);
            unlockable.schematic.unlockCost = 0;
            unlockable.schematic.Display(unlockable.transform);
            unlockable.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void Display()
    {
        unlockable.schematic.Display(transform);
    }
}
