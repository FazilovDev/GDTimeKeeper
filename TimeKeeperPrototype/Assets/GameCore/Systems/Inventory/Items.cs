using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Items : MonoBehaviour
{
    public List<Item> Data = new List<Item>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
