﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Type
{
    /// <summary>
    /// ENTITY CLASS FOR THE PLAYER
    /// Handles user input and applies it to the player NPC
    /// BasicEntity -> NPCEntity -> PlayerEntity
    /// </summary>
    public class PlayerEntity : NPCEntity
    {
        public static PlayerEntity Player { get; private set; }

        void Awake()
        {
            Player = this;
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            // TEST
            if (Input.GetKeyDown("x"))
            {
                Physics.ApplyForce(new Vector3(5, 7, 0));
            }

            HandleInput();

            base.Update();
        }

        private void HandleInput()
        {
            // Ensure the player is able to receive inputs
            if (!Movement.Knockback)
            {
                HandleKeyboard();
                HandleMouse();
            }
        }

        private void HandleKeyboard()
        {
            // MOVEMENT
            {
                // Get movement inputs
                float hor = Input.GetAxisRaw("Horizontal");
                float ver = Input.GetAxisRaw("Vertical");

                Vector2 walk = new Vector2(hor, ver);
                if (walk.magnitude > 0)
                {
                    // If there's movement input, signal the brain to walk
                    Brain.Triggers.Set("player_walk_keyboard", walk);
                }
            }
        }

        private void HandleMouse()
        {
            // MOVEMENT
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (!UnityEngine.Physics.Raycast(ray, 100))
                    {
                        Plane worldPlane = new Plane(Vector3.forward, 0);

                        float enter = 0;
                        if (worldPlane.Raycast(ray, out enter))
                        {
                            // If the world was clicked, signal the brain to go to the location
                            Brain.Triggers.Set("player_walk_mouse", (Vector2)ray.GetPoint(enter));
                        }
                    }
                }
            }
        }
    }
}