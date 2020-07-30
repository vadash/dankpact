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

        public override void OnLoad()
        {
            mySummons = new TimeCache<IEnumerable<Entity>>(UpdateDeployedObjects, 125);
            chatUi = new TimeCache<Element>(UpdateChatUi, 3000);
            Core.MainRunner.Run(new Coroutine(MainCoroutine(), this, "DankPact1"));
            base.OnLoad();
        }

        private IEnumerator MainCoroutine()
        {
            while (true)
            {
                if (!CanRun()) { yield return new WaitTime(500); continue; }

                var thirdClosest = mySummons.Value?.Skip(2)?.FirstOrDefault();
                var needSummon = 
                    thirdClosest?.IsValid != true ||
                    DistToCursor(thirdClosest) > Settings.DarkPactChainRange;

                if (Input.IsKeyDown(Settings.ActivateKey))
                {
                    if (needSummon)
                    {
                        yield return SummonSkeles();
                        yield return new WaitTime(50);
                    }
                    yield return StartDarkPact();
                }
                else
                {
                    var timeSinceDarkPact = (DateTime.Now - LastDarkPact).TotalMilliseconds;
                    if (timeSinceDarkPact > 100 &&
                        timeSinceDarkPact < 350)
                    {
                        yield return EndDarkPact();
                    }
                }

                yield return new WaitTime(33);
            }
        }

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

        public override void Render()
        {
            if (!CanRun()) return;
            if (Settings.Debug) DebugDrawDist();
        }
    }
}
