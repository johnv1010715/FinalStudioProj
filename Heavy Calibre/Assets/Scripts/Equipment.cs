using UnityEngine;

[System.Serializable]
public class Equipment
{
    public Weapon[] weapons;

    public bool FindIndex(Weapon weapon, out int index, int priorityCheck = 0)
    {
        return FindIndex(true, weapon.data.oneHanded, out index, priorityCheck, false);
    }

    public bool FindIndex(bool findEmpty, bool findOneHanded, out int index, int priorityCheck, bool skipCheck)
    {
        if (!skipCheck && IsAvailable(findEmpty, findOneHanded, priorityCheck))
        {
            index = priorityCheck;
            return true;
        }
        else
        {
            index = 0;
            for (int i = 0; i < weapons.Length; i++)
            {
                if (i != priorityCheck && IsAvailable(findEmpty, findOneHanded, i))
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }

    bool IsAvailable(bool findEmpty, bool findOneHanded, int index)
    {
        if (weapons[index])
        {
            if (findOneHanded && weapons[index].data.oneHanded) //one handed
            {
                return (!weapons[index].other || !findEmpty); //with offhand (findEmpty, findOneHanded) + with or w/out offhand (!findEmpty, findOneHanded)
            }
            else if(!findEmpty)
            {
                return true; //any weapon (!findEmpty, !findOneHanded)
            }
            else
            {
                return false;
            }
        }
        else
        {
        return (findEmpty||findOneHanded); //empty (findEmpty,!findOneHanded) || (findEmpty,findOneHanded) || (!findEmpty,findOneHanded)
        }
    }

    public bool IsEmpty()
    {
        foreach(Weapon weapon in weapons)
        {
            if(weapon)
            {
                return false;
            }
        }
        return true;
    }

    public int GetWeight()
    {
        int weight = 0;
        foreach (Weapon weapon in weapons)
        {
            if (weapon)
            {
                weight += weapon.data.weight;
                if(weapon.other)
                {
                    weight += weapon.other.data.weight;
                }
            }
        }
        return weight;
    }
    
    public enum LookFor { Empty, OneHanded };
}