﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using SharpDX;
using Vector2 = System.Numerics.Vector2;

namespace dankpact
{
    public class SkillDpsCore : BaseSettingsPlugin<Settings>
    {
        private readonly int CLOSE_DIST_SQR = 40 * 40;
        private readonly int MED_DIST_SQR = 60 * 60;
        private readonly int FAR_DIST_SQR = 80 * 80;

        int nClose = 0;
        int nMed = 0;
        int nFar = 0;
        List<Entity> skelesEntities;
        TimeCache<ExileCore.PoEMemory.Components.DeployedObject[]> mySummons;
        Entity LocalPlayer => GameController.Game.IngameState.Data.LocalPlayer;

        public override void OnLoad()
        {
            mySummons = new TimeCache<ExileCore.PoEMemory.Components.DeployedObject[]>(UpdateDeployedObjects, 250);
            base.OnLoad();
        }

        public override Job Tick()
        {
            if (!Settings.Enable) return base.Tick();

            skelesEntities = new List<Entity>();
            nClose = 0;
            nMed = 0;
            nFar = 0;

            foreach (var obj in mySummons.Value)
            {
                var squareDist = DistanceSquared(LocalPlayer.GridPos, obj.Entity.GridPos);
                if (squareDist < CLOSE_DIST_SQR)
                {
                    nClose++;
                }
                else if (squareDist < MED_DIST_SQR)
                {
                    nMed++;
                }
                else if (squareDist < FAR_DIST_SQR)
                {
                    nFar++;
                }
                if (Settings.Debug) skelesEntities.Add(obj.Entity);
            }

            return base.Tick();
        }

        private ExileCore.PoEMemory.Components.DeployedObject[] UpdateDeployedObjects()
        {
            return LocalPlayer
                .GetComponent<Actor>()
                .DeployedObjects
                .Where(x => 
                    x != null && 
                    x.Entity != null && 
                    x.Entity.IsAlive &&
                    x.Entity.Path.Contains("RaisedSkeleton"))
                .ToArray();
        }

        public override void Render()
        {
            if (!Settings.Enable) return;
            if (Settings.Debug) DebugDrawDist();

            if (Input.IsKeyDown(Settings.ActivateKey))
            {
                var needSummon = 
                    nClose < 1 ||
                    nClose + nMed < 2 ||
                    nClose + nMed + nFar < 3;
                if (needSummon)
                {
                    SummonSkeles();
                }
                if (!needSummon ||
                    LocalPlayer.GetComponent<Life>().HPPercentage > 90) // dont wait for entity update
                {
                    CastDarkPact();
                }
            }
            else
            {
                var timeSinceDarkPact = (DateTime.Now - LastDarkPact).TotalMilliseconds;
                if (timeSinceDarkPact < 500 &&
                    Input.IsKeyDown(Settings.DarkPactKey))
                {
                    Input.KeyUp(Settings.DarkPactKey);
                }
            }
        }

        private void DebugDrawDist()
        {
            skelesEntities.Sort(EntityComparison);
            var closestSkele = skelesEntities.FirstOrDefault();
            if (closestSkele == null) return;
            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var dist = Math.Sqrt(DistanceSquared(localPlayer.GridPos, closestSkele.GridPos));
            Graphics.DrawText(dist.ToString(), new Vector2(300, 300));
        }

        private int EntityComparison(Entity x, Entity y)
        {
            if (x.DistancePlayer < y.DistancePlayer) return -1;
            else if (x.DistancePlayer > y.DistancePlayer) return 1;
            else return 0;
        }

        private double DistanceSquared(SharpDX.Vector2 v1, SharpDX.Vector2 v2)
        {
            return Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2);
        }

        private DateTime LastDarkPact = DateTime.Now;
        private void CastDarkPact()
        {
            Input.KeyDown(Settings.DarkPactKey);
            LastDarkPact = DateTime.Now;
        }

        private DateTime LastSummonSkeles = DateTime.Now;
        private void SummonSkeles()
        {
            if ((DateTime.Now - LastSummonSkeles).TotalMilliseconds < 1000)
                return;
            KeyPress(Settings.SummonSkeleKey, 100);
            LastSummonSkeles = DateTime.Now;
        }

        private void KeyPress(Keys key, int delay = 20)
        {
            var CoroutineWorker = new Coroutine(KeyPressRoutine(key, delay), this, "KeyPress");
            Core.ParallelRunner.Run(CoroutineWorker);
        }

        private static IEnumerator KeyPressRoutine(Keys key, int delay)
        {
            Input.KeyDown(key);
            yield return new WaitTime(delay);
            Input.KeyUp(key);
        }
    }
}
