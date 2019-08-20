using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Agent
{
    public Equipment equipment;
    public int maxWeight;
    public bool autoSwitch;
    bool inputIsEnabled;

    [SerializeField] RectTransform healthBar, armourBar, shieldBar;
    [SerializeField] Canvas respawnMenu;

    [SerializeField] Transform handSlot, offhandSlot, backSlot, leftSlot, rightSlot;

    GameController gameController;

    [SerializeField] float pickupRange;

    [HideInInspector] public int selectedWeapon;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < equipment.weapons.Length; i++)
        {
            var weapon = equipment.weapons[i];
            if (weapon)
            {
                weapon.equipmentIndex = i;
                weapon.equiped = true;
                weapon.agent = this;
            }
        }
        SelectWeapon(0);
        ToggleInputs(true);
        gameController = FindObjectOfType<GameController>();
        id = 1;
    }

    protected override void Update()
    {
        base.Update();
        if (inputIsEnabled)
        {
            var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            movement = input.magnitude > 1 ? input.normalized : input;
            var screenPos = Camera.main.WorldToScreenPoint(transform.position);
            aimDir = new Vector3(Input.mousePosition.x - screenPos.x, 0f, Input.mousePosition.y - screenPos.y).normalized;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectWeapon(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectWeapon(1);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                NextWeapon();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                PreviousWeapon();
            }

            if (autoSwitch)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    SelectWeapon(0);
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    SelectWeapon(1);
                }
                else if (Input.GetButtonDown("SwapWeapon"))
                {
                    if (equipment.weapons[selectedWeapon] && Input.GetButton("Modifier"))
                    {
                        equipment.weapons[selectedWeapon].SwapHands(this);
                    }
                    else
                    {
                        SwapWeapon(0, 1);
                    }
                }
            }
            else if (Input.GetButtonDown("SwapWeapon"))
            {
                if (equipment.weapons[selectedWeapon] && Input.GetButton("Modifier"))
                {
                    equipment.weapons[selectedWeapon].SwapHands(this);
                }
                else
                {
                    SwapWeapon();
                }
            }

            if (equipment.weapons[selectedWeapon])
            {
                if (Input.GetButton("Fire1") || (autoSwitch && Input.GetButton("Fire2")))
                {
                    if (equipment.weapons[selectedWeapon].AtTarget())
                    {
                        equipment.weapons[selectedWeapon].TriggerDown();
                    }
                }
                else if (Input.GetButtonUp("Fire1") || (autoSwitch && Input.GetButtonUp("Fire2")))
                {
                    equipment.weapons[selectedWeapon].TriggerUp();
                }
                else if (Input.GetButtonDown("ToggleFireMode"))
                {
                    equipment.weapons[selectedWeapon].ToggleFireMode();
                }
            }
            
            if (Input.GetButtonDown("Drop") && equipment.weapons[selectedWeapon])
            {
                if (equipment.weapons[selectedWeapon] && equipment.weapons[selectedWeapon].other && Input.GetButton("Modifier"))
                {
                    equipment.weapons[selectedWeapon].SwapHands(this);
                }
                DropWeapon(selectedWeapon, 0f, false);
            }

            else if (Input.GetButtonDown("Interact"))
            {
                Weapon closestWeapon = null;
                float shortestdistance = Mathf.Infinity;
                foreach (Collider collider in Physics.OverlapSphere(transform.position, pickupRange * 2))
                {
                    var weapon = collider.GetComponent<Weapon>();
                    if (weapon)
                    {
                        Vector3 dif = (weapon.transform.position - transform.position);
                        dif.y = 0;
                        if (dif.magnitude <= pickupRange && dif.magnitude < shortestdistance)
                        {
                            closestWeapon = weapon;
                        }
                    }
                }
                if (closestWeapon)
                {
                    if(equipment.FindIndex(closestWeapon, out int index, selectedWeapon))
                    {
                        closestWeapon.Pickup(this, index);
                    }
                    else
                    {
                        DropWeapon(400);
                        closestWeapon.Pickup(this, selectedWeapon);
                    }
                    Purge(index);
                }
                else if (equipment.weapons[selectedWeapon])
                {
                    DropWeapon(400);
                }
            }
        }

        DisplayStats();
    }

    public void DisplayStats()
    {
        health.SetScale(healthBar);
        armour.SetScale(armourBar);
        shield.SetScale(shieldBar);
    }

    public void Purge(int keep)
    {
        if (equipment.GetWeight() > maxWeight && equipment.FindIndex(false,false,out int dropIndex,keep,true))
        {
            DropWeapon(dropIndex, 0f, false);
            Purge(keep);
        }
    }

    public void SwapWeapon(int weapon1, int weapon2)
    {
        if (equipment.weapons[weapon1])
        {
            if (equipment.weapons[weapon2])
            {
                var tempWeapon = equipment.weapons[weapon1];
                equipment.weapons[weapon1] = equipment.weapons[weapon2];
                equipment.weapons[weapon2] = tempWeapon;

                equipment.weapons[weapon1].equipmentIndex = weapon1;
                equipment.weapons[weapon2].equipmentIndex = weapon2;
            }
            else
            {
                equipment.weapons[weapon2] = equipment.weapons[weapon1];
                equipment.weapons[weapon2].equipmentIndex = weapon2;
                equipment.weapons[weapon1] = null;
            }
        }
        else if (equipment.weapons[weapon2])
        {
            equipment.weapons[weapon1] = equipment.weapons[weapon2];
            equipment.weapons[weapon1].equipmentIndex = weapon1;
            equipment.weapons[weapon2] = null;
        }
        SelectWeapon(selectedWeapon);
    }

    public void SwapWeapon()
    {
        if (selectedWeapon == 0)
        {
            SelectWeapon(1);
        }
        else
        {
            SelectWeapon(0);
        }
    }

    void NextWeapon()
    {
        SelectWeapon(selectedWeapon < equipment.weapons.Length - 1 ? selectedWeapon + 1 : 0);
    }

    void PreviousWeapon()
    {
        SelectWeapon(selectedWeapon > 0 ? selectedWeapon - 1 : equipment.weapons.Length - 1);
    }

    public void SelectWeapon(int index)
    {
        this.selectedWeapon = index;
        foreach (Weapon weapon in equipment.weapons)
        {
            if (weapon)
            {
                if (weapon.equipmentIndex == this.selectedWeapon)
                {
                    weapon.targetTransform = handSlot;
                    if(weapon.other)
                    {
                        weapon.other.targetTransform = offhandSlot;
                    }
                }
                else
                {
                    weapon.targetTransform = weapon.data.oneHanded ? rightSlot : backSlot;
                    if(weapon.other)
                    {
                        weapon.other.targetTransform = leftSlot;
                    }
                    weapon.TriggerUp();
                }
            }
        }
        var selectedWeapon = equipment.weapons[this.selectedWeapon];
        
        float weightPenalty = 0.2f * ((float)equipment.GetWeight() / maxWeight);
        float mobility = selectedWeapon ? selectedWeapon.other ? selectedWeapon.data.mobility - (1 - selectedWeapon.other.data.mobility) : selectedWeapon.data.mobility : 1;
        speed = maxSpeed * mobility * (1 - weightPenalty);
    }

    public void DropWeapon(float force, bool dropAll = true)
    {
        DropWeapon(selectedWeapon, force, dropAll);
    }

    public void DropWeapon(int index, float force, bool dropAll = true)
    {
        equipment.weapons[index].Drop(this, force, dropAll);
        SelectWeapon(selectedWeapon);
    }

    public bool IsInsideBase(Vector4 bounds)
    {
        return (transform.position.x >= bounds.x && transform.position.x < bounds.z && transform.position.z >= bounds.y && transform.position.z < bounds.w);
    }

    public void Heal(float health)
    {
        this.health.Add(health);
    }

    public void RepairArmour(float value)
    {
        armour.Add(value);
    }

    protected override void Kill()
    {
        base.Kill();
        foreach (Weapon weapon in equipment.weapons)
        {
            if (weapon)
            {
                DropWeapon(weapon.equipmentIndex,0,true);
            }
        }
        gameController.EndGame();
        GetComponent<CapsuleCollider>().enabled = false;
        DisplayStats();
        enabled = false;
    }

    public void Respawn()
    {
        enabled = true;
        transform.position = Vector3.zero;
        GetComponent<CapsuleCollider>().enabled = true;
        enabled = true;
        shield = new RegenStat{};
        base.Start();
    }

    public void ToggleInputs()
    {
        ToggleInputs(!inputIsEnabled);
    }

    public void ToggleInputs(bool enable)
    {
        if (enable)
        {
            inputIsEnabled = true;
        }
        else
        {
            inputIsEnabled = false;
        }
    }
}
