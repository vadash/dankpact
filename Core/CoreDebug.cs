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
        private void DebugDrawDist()
        {
            var closestSummon = mySummons.Value.FirstOrDefault();
            if (closestSummon == null) return;
            Graphics.DrawText(DistToCursor(closestSummon).ToString(), new Vector2(300, 300));
        }
    }
}
