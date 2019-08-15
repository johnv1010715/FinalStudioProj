using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public abstract class Schematic
{
    public string name;
    public int cost;
    public int unlockCost;
    public bool singleUse;

    protected GameController gameController;

    public virtual void Craft(PlayerController player)
    {
        gameController = Object.FindObjectOfType<GameController>();
        gameController.AddResource(-cost);
    }

    public virtual void Display(Transform item)
    {
        item.Find("Name").GetComponent<Text>().text = name;
        item.Find("Cost").GetComponent<Text>().text = "x " + (unlockCost > 0 ? unlockCost : cost).ToString();
    }
}

[System.Serializable]
public class WeaponSchematic : Schematic
{
    public int id;

    public override void Craft(PlayerController player)
    {
        base.Craft(player);
        gameController.SpawnWeapon(id, player);
    }

    public override void Display(Transform item)
    {
        base.Display(item);
    }
}

[System.Serializable]
public class ShieldSchematic : Schematic
{
    public RegenStat shield;

    public override void Craft(PlayerController player)
    {
        base.Craft(player);
        player.shield = shield;
    }

    public override void Display(Transform item)
    {
        base.Display(item);
    }
}

[System.Serializable]
public class ArmourSchematic : Schematic
{
    public int value;
    public bool addToMax;

    public override void Craft(PlayerController player)
    {
        base.Craft(player);
        player.RepairArmour(value);
    }

    public override void Display(Transform item)
    {
        base.Display(item);
    }
}