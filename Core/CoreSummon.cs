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
        private IEnumerator StartDarkPact()
        {
            yield return KeyPress(Settings.DarkPactKey, 0);
            LastDarkPact = DateTime.Now;
        }

        private IEnumerator EndDarkPact()
        {
            Input.KeyUp(Settings.DarkPactKey);
            yield break;
        }

        private DateTime LastSummonSkeles = DateTime.Now;
        private IEnumerator SummonSkeles()
        {
            if ((DateTime.Now - LastSummonSkeles).TotalMilliseconds < 2000)
                yield break;
            yield return KeyPress(Settings.SummonSkeleKey, 100);
            LastSummonSkeles = DateTime.Now;
        }

        private static IEnumerator KeyPress(Keys key, int delay)
        {
            Input.KeyDown(key);
            if (delay != 0)
            {
                yield return new WaitTime(delay);
                Input.KeyUp(key);
            }
        }
    }
}
