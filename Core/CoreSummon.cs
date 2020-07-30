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
            if ((DateTime.Now - LastSummonSkeles).TotalMilliseconds < 1000)
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
