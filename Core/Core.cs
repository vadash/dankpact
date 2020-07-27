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
        TimeCache<IEnumerable<Entity>> mySummons;
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
            mySummons = new TimeCache<IEnumerable<Entity>>(UpdateDeployedObjects, 250);
            chatUi = new TimeCache<Element>(UpdateChatUi, 3000);
            base.OnLoad();
        }

        public override Job Tick()
        {
            if (!CanRun()) return base.Tick();

            var chainRangeSqr = Settings.DarkPactChainRange * Settings.DarkPactChainRange;
            var sortedSummons = mySummons
                .Value
                .Where(x =>
                    DistToCursorSqr(x) < chainRangeSqr)
                .ToList();
            sortedSummons.Sort(ClosestToMouseComparison);
            var closestSummon = sortedSummons.FirstOrDefault();

            var highHp = LocalPlayer.GetComponent<Life>().HPPercentage > 0.9;
            var needSummon = closestSummon?.IsValid != true;
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
                    StartDarkPact(20);
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

        public override void Render()
        {
            if (!CanRun()) return;
            if (Settings.Debug) DebugDrawDist();
        }
    }
}
