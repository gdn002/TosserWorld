using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Entity;
using Entity.Component;

public class StorageTest : MonoBehaviour
{
    public GameObject Item;
    public ContainerComponent Container;

    private bool stored = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!stored)
            {
                Item.AddComponent<EntityStack>();
                Container.Add(Item.GetComponent<EntityStack>());
            }
            else
            {
                EntityStack taken = Container.Take(0);
                taken.Entity.OnRemovedFromContainer(Container);
            }

            stored = !stored;
        }
    }
}

