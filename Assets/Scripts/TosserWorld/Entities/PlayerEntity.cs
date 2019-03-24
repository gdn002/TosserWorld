using UnityEngine;
using TosserWorld.Modules;
using UnityEngine.EventSystems;
using TosserWorld.Modules.BrainScripts;
using TosserWorld.UI;

namespace TosserWorld.Entities
{
    /// <summary>
    /// ENTITY CLASS FOR THE PLAYER
    /// Handles user input and applies it to the player NPC
    /// </summary>
    public class PlayerEntity : Entity
    {
        public static PlayerEntity Player { get; private set; }


        void Awake()
        {
            Player = this;
        }

        public void QueueInteraction(Entity entity)
        {
            Brain.Triggers.Set(TosserBrain.LocalTriggers.INTERACT, entity);
        }

        // Update is called once per frame
        protected override void Update()
        {
            Inputs();

            base.Update();
        }

        private void Inputs()
        {
            // Ensure the player is able to receive inputs
            if (IsInputAllowed())
            {
                Mouse();
                Keyboard();
            }
        }

        private void Keyboard()
        {
            // MOVEMENT
            {
                // Get movement inputs
                float hor = Input.GetAxisRaw("Horizontal");
                float ver = Input.GetAxisRaw("Vertical");

                Vector2 walk = new Vector2(hor, ver);
                if (walk.magnitude > 0)
                {
                    // If there's movement input, reset queued actions and walk
                    Brain.Triggers.Set(TosserBrain.LocalTriggers.RESET, null);
                    Movement.MoveScreenFull(walk);
                }
            }

            // ACTIONS
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // First shot
                    ActivateEquipment();
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    // Auto fire
                    ActivateEquipment(true);
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    Inventory.DropAll();
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    Inventory.OpenCloseContainer();
                }
            }
        }

        private void Mouse()
        {
            // MOVEMENT
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // LMB clicked
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        // Mouse is not already over a selectable element
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (!UnityEngine.Physics.Raycast(ray, 100))
                        {
                            Plane worldPlane = new Plane(Vector3.forward, 0);

                            float enter = 0;
                            if (worldPlane.Raycast(ray, out enter))
                            {
                                // A spot in the world was clicked
                                Vector2 worldLocation = (Vector2)ray.GetPoint(enter);

                                if (UICursor.Cursor.AttachedEntity == null)
                                {
                                    // Nothing in the cursor slot, signal the brain to go to the location
                                    Brain.Triggers.Set(TosserBrain.LocalTriggers.WALK_MOUSE, worldLocation);
                                }
                                else
                                {
                                    // Signal the brain to drop whatever's in the cursor at the spot
                                    Brain.Triggers.Set(TosserBrain.LocalTriggers.DROP_CURSOR, worldLocation);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsInputAllowed()
        {
            return true;
        }
    }
}