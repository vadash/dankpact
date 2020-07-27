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
    public class DrawSkills
    {
        
    }
    public class SkillDpsCore : BaseSettingsPlugin<Settings>
    {
        public override void Render()
        {
            if (Input.IsKeyDown(Settings.ActivateKey))
            {
                var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
                var mySummons = localPlayer
                    .GetComponent<Actor>()
                    .DeployedObjects
                    .Where(x => x != null && x.Entity != null && x.Entity.IsAlive);
                var skeletons = 0;
                foreach (var obj in mySummons)
                {
                    if (obj.Entity.Path.Contains("Skele"))
                        skeletons++;
                }
                if (skeletons < 4)
                {
                    SummonSkeles();
                    return;
                }
                CastDarkPact();
            }
        }

        private DateTime LastDarkPact = DateTime.Now;
        private void CastDarkPact()
        {
            if ((DateTime.Now - LastDarkPact).TotalMilliseconds < 250)
                return;
            KeyPress(Settings.DarkPactKey, 500);
            LastDarkPact = DateTime.Now;
        }

        private DateTime LastSummonSkeles = DateTime.Now;
        private void SummonSkeles()
        {
            if ((DateTime.Now - LastSummonSkeles).TotalMilliseconds < 600)
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
