using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles camera movement, rotation and tracking
/// </summary>
public class CameraController : MonoBehaviour
{
    public static CameraController Controller { get; private set; }
    public static Camera Camera { get; private set; }

    /// <summary>
    /// Camera orientation data
    /// </summary>
    public class CameraOrientation
    {
        public enum Orientation
        {
            N = 0,
            NE,
            E,
            SE,
            S,
            SW,
            W,
            NW,
        }

        public Orientation CurrentOrientation;

        public float RotationAngle()
        {
            switch (CurrentOrientation)
            {
                case Orientation.N:
                    return 0;
                case Orientation.NE:
                    return -45;
                case Orientation.E:
                    return -90;
                case Orientation.SE:
                    return -135;
                case Orientation.S:
                    return -180;
                case Orientation.SW:
                    return -225;
                case Orientation.W:
                    return -270;
                case Orientation.NW:
                    return -315;
            }

            return 0;
        }

        public float RelativeHorizontal(Vector2 vector)
        {
            // Rotate 45 degrees to match the isometric camera
            Vector2 adjusted = Quaternion.Euler(0, 0, -45) * vector;

            // Extract the relative horizontal value
            switch(CurrentOrientation)
            {
                case Orientation.N:
                    return adjusted.x;
                case Orientation.NE:
                    return vector.x;
                case Orientation.E:
                    return -adjusted.y;
                case Orientation.SE:
                    return -vector.y;
                case Orientation.S:
                    return -adjusted.x;
                case Orientation.SW:
                    return -vector.x;
                case Orientation.W:
                    return adjusted.y;
                case Orientation.NW:
                    return vector.y;
            }

            return 0;
        }

        public float RelativeVertical(Vector2 vector)
        {
            // Rotate 45 degrees to match the isometric camera
            Vector2 adjusted = Quaternion.Euler(0, 0, -45) * vector;

            // Extract the relative horizontal value
            switch (CurrentOrientation)
            {
                case Orientation.N:
                    return adjusted.y;
                case Orientation.NE:
                    return vector.y;
                case Orientation.E:
                    return adjusted.x;
                case Orientation.SE:
                    return vector.x;
                case Orientation.S:
                    return -adjusted.y;
                case Orientation.SW:
                    return -vector.y;
                case Orientation.W:
                    return -adjusted.x;
                case Orientation.NW:
                    return -vector.x;
            }

            return 0;
        }

        public void Rotate(bool clockwise)
        {
            switch (CurrentOrientation)
            {
                case Orientation.N:
                    CurrentOrientation = clockwise ? Orientation.NE : Orientation.NW;
                    break;
                case Orientation.NE:
                    CurrentOrientation = clockwise ? Orientation.E : Orientation.N;
                    break;
                case Orientation.E:
                    CurrentOrientation = clockwise ? Orientation.SE : Orientation.NE;
                    break;
                case Orientation.SE:
                    CurrentOrientation = clockwise ? Orientation.S : Orientation.E;
                    break;
                case Orientation.S:
                    CurrentOrientation = clockwise ? Orientation.SW : Orientation.SE;
                    break;
                case Orientation.SW:
                    CurrentOrientation = clockwise ? Orientation.W : Orientation.S;
                    break;
                case Orientation.W:
                    CurrentOrientation = clockwise ? Orientation.NW : Orientation.SW;
                    break;
                case Orientation.NW:
                    CurrentOrientation = clockwise ? Orientation.N : Orientation.W;
                    break;
            }
        }
    }

    // Current camera target
    public Transform Target;

    public float RotationTime = 0.1f;

    // Camera will hover to this position relative to the target object
    private Vector3 ReferencePosition;

    public CameraOrientation Orientation { get; private set; }
    private bool IsRotating = false;

    void Awake()
    {
        Controller = this;
        Camera = GetComponentInChildren<Camera>();
    }

    void Start ()
    {
        // Take current position as reference position for targeting
        // Camera should be pointing at 0, 0 at the start for proper calibration
        ReferencePosition = transform.position;

        Orientation = new CameraOrientation();

        if (Target == null)
        {
            Debug.LogWarning("No target set for camera.");
        }
    }

    void Update()
    {
        if (!IsRotating)
        {
            if (Input.GetKeyDown("q"))
            {
                StartCoroutine(RotateCamera(true));
            }
            else if (Input.GetKeyDown("e"))
            {
                StartCoroutine(RotateCamera(false));
            }
        }
    }

    void LateUpdate () {
        // Snap camera to target
        transform.position = Target.position + ReferencePosition;
    }

    private IEnumerator RotateCamera(bool clockwise)
    {
        IsRotating = true;

        float from = Orientation.RotationAngle();
        float to = from + (clockwise ? -45 : 45);

        Orientation.Rotate(clockwise);

        float current = from;
        float elapsed = 0;

        while (elapsed < RotationTime)
        {
            elapsed += Time.deltaTime;
            current = Mathf.Lerp(from, to, elapsed / RotationTime);

            transform.eulerAngles = new Vector3(0, 0, current);

            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, to);

        IsRotating = false;
    }
}
