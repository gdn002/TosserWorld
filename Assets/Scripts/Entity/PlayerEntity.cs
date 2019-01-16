using UnityEngine;
using TosserWorld.Modules;
using UnityEngine.EventSystems;
using TosserWorld.Modules.BrainScripts;
using TosserWorld.UI;

namespace TosserWorld
{
    /// <summary>
    /// ENTITY CLASS FOR THE PLAYER
    /// Handles user input and applies it to the player NPC
    /// BasicEntity -> NPCEntity -> PlayerEntity
    /// </summary>
    public class PlayerEntity : Entity
    {
        public static PlayerEntity Player { get; private set; }


        private BrainModule Brain;

        void Awake()
        {
            Player = this;
        }

        public void QueueInteraction(Entity entity)
        {
            Brain.Triggers.Set(TosserBrain.LocalTriggers.INTERACT, entity);
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            Brain = GetModule<BrainModule>();
        }

        // Update is called once per frame
        protected override void Update()
        {
            HandleInput();

            base.Update();
        }

        private void HandleInput()
        {
            // Ensure the player is able to receive inputs
            if (IsInputAllowed())
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
                    Brain.Triggers.Set(TosserBrain.LocalTriggers.WALK_KEYBOARD, walk);
                }
            }

            // ACTIONS
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    GetModule<ContainerModule>().DropAll();
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    GetModule<ContainerModule>().OpenCloseContainer();
                }
            }
        }

        private void HandleMouse()
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
                        if (!Physics.Raycast(ray, 100))
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