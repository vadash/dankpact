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
        public int CountEntitiesAroundMouse(IEnumerable<Entity> entities, float maxDistance)
        {
            int count = 0;
            float maxDistanceSquare = maxDistance * maxDistance;
            foreach (var entity in entities)
            {
                var monsterPosition = entity.Pos;
                var screenPosition = GameController.IngameState.Camera.WorldToScreen(monsterPosition);
                var cursorPosition = Input.MousePosition;

                var xDiff = screenPosition.X - cursorPosition.X;
                var yDiff = screenPosition.Y - cursorPosition.Y;
                var monsterDistanceSquare = (xDiff * xDiff + yDiff * yDiff);

                if (monsterDistanceSquare <= maxDistanceSquare)
                {
                    count++;
                }
            }

            return count;
        }

        private DateTime LastDarkPact = DateTime.Now;
        private void StartDarkPact(int initialDelay = 0)
        {
            KeyPress(Settings.DarkPactKey, 0, true, initialDelay);
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

        private void KeyPress(Keys key, int keyUpDelay = 20, bool channeled = false, int initialDelay = 0)
        {
            var CoroutineWorker = new Coroutine(KeyPressRoutine(key, keyUpDelay, channeled, initialDelay), this, "KeyPress");
            Core.ParallelRunner.Run(CoroutineWorker);
        }

        private static IEnumerator KeyPressRoutine(Keys key, int keyUpDelay, bool channeled, int initialDelay)
        {
            if (initialDelay > 0) 
                yield return new WaitTime(initialDelay);
            Input.KeyDown(key);
            if (!channeled)
            {
                yield return new WaitTime(keyUpDelay);
                Input.KeyUp(key);
            }
        }
    }
}
