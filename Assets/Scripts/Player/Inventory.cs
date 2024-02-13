using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static class Keys
    {
        public static int NormalKey = 1;
        public static int GoldKey = 2;

    }
    public int keys {get; private set;} = 0;

    public bool HasKey(int key)
    {
        return (keys & key) != 0;
    }

    public void AddKey(int key)
    {
        // if hasn't that key already
        if ((keys & key) == 0)  keys |= key;
    }
}
