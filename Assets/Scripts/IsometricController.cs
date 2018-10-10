using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Handles several isometric functions for sprite containers
/// </summary>
public class IsometricController : MonoBehaviour
{
    void Start ()
    {
        GameObject[] isometricObjects = GameObject.FindGameObjectsWithTag("IsometricSprite");
        foreach (var isoObj in isometricObjects)
        {
            SortingGroup sortGroup = isoObj.GetComponent<SortingGroup>();
            if (sortGroup != null)
                sortGroup.sortingOrder = 1;
        }
    }
    
    // Update is called once per frame
    void Update ()
    {
        GameObject[] isometricObjects = GameObject.FindGameObjectsWithTag("IsometricSprite");
        foreach (var isoObj in isometricObjects)
        {
            isoObj.transform.rotation = CameraController.Camera.transform.rotation;
        }
    }

    private void IsometricSorting(GameObject obj)
    {
        // Sort sprites by rotating their world position by 45 degrees and measuring them along the X axis
        float position = CameraController.Controller.Orientation.RelativeVertical(obj.transform.position);

        SortingGroup sortGroup = obj.GetComponent<SortingGroup>();
        if (sortGroup != null)
        {
            sortGroup.sortingOrder = (int)(-position * 1000);
        }
    }
}
