using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] Canvas[] menus;
    [SerializeField] Canvas gameHUD;
    [SerializeField] CanvasScaler[] UIElements;

    [SerializeField] Toggle autoScaleButton;

    [HideInInspector] public Canvas playerHUD;
    List<string> history = new List<string>();

    [SerializeField] GameObject weaponItem;
    [SerializeField] GameObject armourItem;
    [SerializeField] GameObject shieldItem;
    [SerializeField] GameObject lockedItem;
    [SerializeField] Transform weaponGrid;
    [SerializeField] Transform equipmentGrid;

    public void Start()
    {
        bool scaleMode = PlayerPrefs.GetInt("scaleMode", 0) == 1;
        autoScaleButton.isOn = scaleMode;
        SetScaleMode(scaleMode);
        float uiScale = PlayerPrefs.GetFloat("uiScale");
        SetScale(uiScale > 0 ? uiScale : 2, false);
    }

    public void SetMenu(string name)
    {
        SetMenu(name,true);
    }

    public void SetMenu(string name, bool save, bool clear = false, bool bounce = true)
    {
        if (clear)
        {
            history.Clear();
        }
        foreach (Canvas menu in menus)
        {
            if (menu.name == name)
            {
                if (menu.enabled == true)
                {
                    save = false;
                    if (bounce)
                    {
                        Return();
                    }
                }
                else
                {
                    menu.enabled = true;
                }
            }
            else
            {
                menu.enabled = false;
            }
        }
        if (save)
        {
            history.Add(name);
        }
    }

    public string GetMenu()
    {
        return history[history.Count - 1];
    }

    public string GetMenu(int index)
    {
        return history[index];
    }

    public void Return(int index, bool clear)
    {
        SetMenu(history[index], true, clear, false);
    }

    public void Return()
    {
        history.Remove(GetMenu());
        SetMenu(GetMenu(), false);
    }
    
    public void SetScaleMode(bool auto)
    {
        CanvasScaler.ScaleMode scaleMode;
        if (auto)
        {
            PlayerPrefs.SetInt("scaleMode", 1);
            scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }
        else
        {
            PlayerPrefs.SetInt("scaleMode", 0);
            scaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        }
        foreach (CanvasScaler element in UIElements)
        {
            element.uiScaleMode = scaleMode;
        }
    }

    public void SetScale(float scale)
    {
        SetScale(scale, true);
    }

    void SetScale(float scale, bool save)
    {
        if (save)
        {
            Debug.Log(scale);
            PlayerPrefs.SetFloat("uiScale", scale);
            Debug.Log(PlayerPrefs.GetFloat("uiScale"));
        }
        foreach (CanvasScaler element in UIElements)
        {
            element.scaleFactor = scale;
        }
    }

    public void DisplayWeapons(WeaponSchematic[] weaponSchematics)
    {
        DisplaySchematics(weaponItem, weaponGrid, weaponSchematics, true);
    }

    public void DisplayEquipment(ArmourSchematic[] armourSchematics, ShieldSchematic[] shieldSchematics)
    {
        DisplaySchematics(armourItem, equipmentGrid, armourSchematics, true);
        AddUpgradable(shieldItem, equipmentGrid, shieldSchematics);
    }

    void DisplaySchematics(GameObject menuItem, Transform grid, Schematic[] schematics, bool clear = true)
    {
        if (clear)
        {
            ClearGrid(grid.transform);
        }

        for (int i = 0; i < schematics.Length; i++)
        {
            var newMenuItem = Instantiate(menuItem, grid).GetComponent<MenuItem>();
            newMenuItem.schematic = schematics[i];

            if (schematics[i].unlockCost > 0)
            {
                var newMenuItemUnlockable = Instantiate(lockedItem, grid).GetComponent<MenuItemUnlockable>();
                newMenuItemUnlockable.unlockable = newMenuItem;
                newMenuItemUnlockable.Display();
                newMenuItem.gameObject.SetActive(false);
            }
            else
            {
                newMenuItem.Display();
            }
        }
    }

    public void AddUpgradable(GameObject menuItem, Transform grid, Schematic[] schematics)
    {
        var newMenuItem = Instantiate(menuItem, grid).GetComponent<MenuItemUpgradeable>();
        newMenuItem.schematics = schematics;
        newMenuItem.Display();
    }

    public void ClearGrid(Transform grid)
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
    }

    public void EnableHUD(bool enable)
    {
        gameHUD.enabled = enable;
        playerHUD.enabled = enable;
    }

    public void SnapshotMode()
    {
        FindObjectOfType<CameraController>().enabled = false;
        SnapshotController.Enable();
        SetMenu("");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MenuReturn()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
