using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Entity.Type;
using Entity.Component;

public class StorageTest : MonoBehaviour
{
    public BasicEntity Item;
    public BasicInventory Container;

    private bool stored = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!stored)
            {
                Container.Add(Item);
            }
            else
            {
                BasicEntity taken = Container.Take(0);
                taken.OnRemovedFromContainer(Container);
            }

            stored = !stored;
        }
    }
}

