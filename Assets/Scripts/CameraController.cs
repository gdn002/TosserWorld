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
        public enum Orientations
        {
            North = 0,
            East,
            South,
            West
        }

        public Orientations CurrentOrientation;

        public float RotationAngle()
        {
            switch (CurrentOrientation)
            {
                case Orientations.North:
                    return 0;
                case Orientations.East:
                    return -90;
                case Orientations.South:
                    return -180;
                case Orientations.West:
                    return -270;
            }

            return 0;
        }

        public float RelativeHorizontal(Vector2 vector)
        {
            // Rotate 45 degrees to match the isometric camera
            vector = Quaternion.Euler(0, 0, -45) * vector;

            // Extract the relative horizontal value
            switch(CurrentOrientation)
            {
                case Orientations.North:
                    return vector.x;
                case Orientations.East:
                    return -vector.y;
                case Orientations.South:
                    return -vector.x;
                case Orientations.West:
                    return vector.y;
            }

            return 0;
        }

        public float RelativeVertical(Vector2 vector)
        {
            // Rotate 45 degrees to match the isometric camera
            vector = Quaternion.Euler(0, 0, -45) * vector;

            // Extract the relative horizontal value
            switch (CurrentOrientation)
            {
                case Orientations.North:
                    return vector.y;
                case Orientations.East:
                    return vector.x;
                case Orientations.South:
                    return -vector.y;
                case Orientations.West:
                    return -vector.x;
            }

            return 0;
        }

        public void Rotate(bool clockwise)
        {
            switch (CurrentOrientation)
            {
                case Orientations.North:
                    CurrentOrientation = clockwise ? Orientations.East : Orientations.West;
                    break;
                case Orientations.East:
                    CurrentOrientation = clockwise ? Orientations.South : Orientations.North;
                    break;
                case Orientations.South:
                    CurrentOrientation = clockwise ? Orientations.West : Orientations.East;
                    break;
                case Orientations.West:
                    CurrentOrientation = clockwise ? Orientations.North : Orientations.South;
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
        float to = from + (clockwise ? -90 : 90);

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
