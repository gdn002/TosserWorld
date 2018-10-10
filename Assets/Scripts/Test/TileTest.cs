using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileTest : MonoBehaviour
{

    public Camera MainCamera;
    public Tilemap MapToPaint;
    public TileBase TileToPaint;
    public RuleTile EdgeTile;

    private Plane WorldPlane;

    // Use this for initialization
    void Start () {
        WorldPlane = new Plane(new Vector3(0, 0, -1), 0);
    }

    // Update is called once per frame
    void Update () {

        bool paint = Input.GetMouseButton(0);
        bool erase = Input.GetMouseButton(1);

        if (paint || erase)
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            float enter = 0;
            if (WorldPlane.Raycast(ray, out enter))
            {
                Vector3Int cell = GetComponent<Grid>().WorldToCell(ray.GetPoint(enter));

                if (paint)
                    PaintTile(cell);
                else
                    EraseTile(cell);
            }
        }
    }

    private void PaintTile(Vector3Int cell)
    {
        MapToPaint.SetTile(cell, TileToPaint);

        for(int x = -1; x < 2; ++x)
        {
            for (int y = -1; y < 2; ++y)
            {
                Vector3Int neighbor = new Vector3Int(cell.x + x, cell.y + y, cell.z);
                TileBase neighborTile = MapToPaint.GetTile(neighbor);
                if (neighborTile == null)
                {
                    MapToPaint.SetTile(neighbor, EdgeTile);
                }
                else
                {
                    MapToPaint.RefreshTile(neighbor);
                }
            }
        }
    }

    private void EraseTile(Vector3Int cell)
    {
        MapToPaint.SetTile(cell, EdgeTile);
    }
}
