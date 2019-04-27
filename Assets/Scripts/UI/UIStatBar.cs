using System.Collections;
using System.Collections.Generic;
using TosserWorld.Utilities;
using UnityEngine;

namespace TosserWorld.UI
{
    public class UIStatBar : MonoBehaviour
    {
        public Stat TrackedStat;
        public Color BarColor = Color.green;

        private Transform BarTransform;

        // Use this for initialization
        void Start()
        {
            BarTransform = transform.Find("Bar");
            BarTransform.gameObject.GetComponent<SpriteRenderer>().color = BarColor;

            transform.rotation = CameraController.CameraRotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (TrackedStat != null)
            {
                BarTransform.localScale = new Vector3(TrackedStat.PercentAt, BarTransform.localScale.y, BarTransform.localScale.z);
            }

            transform.rotation = CameraController.CameraRotation;
        }
    }
}