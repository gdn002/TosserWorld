﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Type;

namespace Entity.Component.Brain
{
    public class TestPackBrain : BrainComponent
    {
        protected TestPackBrain Leader = null;
        protected bool IsLeader = false;

        protected List<TestPackBrain> Pack = new List<TestPackBrain>();

        private Vector2 Destination;

        protected override IEnumerator MainLoop()
        {
            if (IsLeader)
            {
                // The leader brain commands the whole pack
                yield return LeaderLoop();
            }
            else
            {
                // Pack member behaviours are mostly emergency ones
                yield return PackLoop();
            }
        }

        private IEnumerator LeaderLoop()
        {
            Destination = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));

            GoTo(Destination);
            float angle = Vector2.SignedAngle(Vector2.up, Owner.Movement.WalkVector);

            while ((Vector2)transform.position != Destination)
            {
                GoTo(Destination);

                for (int i = 0; i < Pack.Count; i++)
                {
                    if (Pack[i].Owner.DistanceTo(Owner) < 0.4f)
                    {
                        // If the pack member is too close to the leader, stop and let the leader through
                        Pack[i].Owner.Stop();
                    }
                    else
                    {
                        // Move to formation
                        Vector2 pos = new Vector2();
                        int fact = i / 2;
                        fact++;

                        if (i % 2 == 0)
                        {
                            pos.x = -0.5f * fact;
                            pos.y = -0.5f * fact;
                        }
                        else
                        {
                            pos.x = 0.5f * fact;
                            pos.y = -0.5f * fact;
                        }

                        pos = Quaternion.Euler(0, 0, angle) * pos;
                        pos += (Vector2)transform.position;

                        // If the pack member is too far out of formation, speed it up
                        if (Vector2.Distance(Pack[i].Owner.Position, pos) > 0.5f)
                            Pack[i].Owner.Speed = Owner.Speed * 1.5f;
                        else
                            Pack[i].Owner.Speed = Owner.Speed * 0.95f;

                        Pack[i].GoTo(pos);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator PackLoop()
        {
            if (Leader == null)
            {
                // This member of the pack has no leader - look around for any leaders
                Leader = FindLeader();
                if (Leader == null)
                {
                    // No leaders around - this pack member becomes leader then
                    MakeLeader();
                    yield return null;
                }
            }
        }


        private TestPackBrain FindLeader()
        {
            foreach (var entity in Awareness)
            {
                // Entity must match the pack's type
                TestPackBrain brain = entity.GetComponent<TestPackBrain>();
                if (brain != null)
                {
                    if (brain.IsLeader)
                    {
                        return brain;
                    }
                }
            }

            return null;
        }

        private void MakeLeader()
        {
            // Make this pack member a leader and let any nearby members know
            IsLeader = true;
            foreach (var entity in Awareness)
            {
                // Entity must match the pack's type
                TestPackBrain brain = entity.GetComponent<TestPackBrain>();
                if (brain != null)
                {
                    brain.IsLeader = false;
                    brain.Leader = this;
                    Pack.Add(brain);
                }
            }
        }
    }
}
