using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemType
{
    None = -1,
    Weapon
}

[Serializable]
public class Item
{
    public string Name;
    public ItemType Type;
    public GameObject Prefab;
}
