using System;
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
    public partial class DankPactCore : BaseSettingsPlugin<Settings>
    {
        private readonly int CLOSE_DIST_SQR = 40 * 40;
        private readonly int MED_DIST_SQR = 80 * 80;
        private readonly int FAR_DIST_SQR = 120 * 120;

        int nClose = 0;
        int nMed = 0;
        int nFar = 0;
        List<Entity> skelesEntities;
        TimeCache<ExileCore.PoEMemory.Components.DeployedObject[]> mySummons;
        Entity LocalPlayer => GameController.Game.IngameState.Data.LocalPlayer;

        private bool CanRun()
        {
            if (!Settings.Enable) return false;
            if (GameController?.InGame == false) return false;
            if (GameController?.Area?.CurrentArea?.IsTown == true) return false;
            if (MenuWindow.IsOpened) return false;
            if (GameController?.Entities?.Count == 0) return false;
            if (GameController?.IsForeGroundCache == false) return false;
            if (ChatIsOpened()) return false;
            return true;
        }

        public override void OnLoad()
        {
            mySummons = new TimeCache<ExileCore.PoEMemory.Components.DeployedObject[]>(UpdateDeployedObjects, 250);
            chatUi = new TimeCache<Element>(UpdateChatUi, 3000);
            base.OnLoad();
        }

        public override Job Tick()
        {
            if (!CanRun()) return base.Tick();

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

            var highHp = LocalPlayer.GetComponent<Life>().HPPercentage > 0.9;
            var needSummon =
                nClose < 1 ||
                nClose + nMed < 2 ||
                nClose + nMed + nFar < 3;
            if (!highHp && needSummon)
            {
                EndDarkPact();
            }

            if (Input.IsKeyDown(Settings.ActivateKey))
            {
                if (needSummon)
                {
                    SummonSkeles();
                }
                if (!needSummon)
                {
                    StartDarkPact();
                }
            }
            else
            {
                var timeSinceDarkPact = (DateTime.Now - LastDarkPact).TotalMilliseconds;
                if (timeSinceDarkPact > 100 &&
                    timeSinceDarkPact < 350)
                {
                    EndDarkPact();
                }
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

        private Element UpdateChatUi()
        {
            return GetChatUi();
        }

        public override void Render()
        {
            if (!CanRun()) return;
            if (Settings.Debug) DebugDrawDist();
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
        private void StartDarkPact()
        {
            Input.KeyDown(Settings.DarkPactKey);
            LastDarkPact = DateTime.Now;
        }

        private void EndDarkPact()
        {
            Input.KeyUp(Settings.DarkPactKey);
        }

        private DateTime LastSummonSkeles = DateTime.Now;
        private void SummonSkeles()
        {
            if ((DateTime.Now - LastSummonSkeles).TotalMilliseconds < 2000)
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
