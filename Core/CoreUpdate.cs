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
    public partial class DankPactCore : BaseSettingsPlugin<Settings>
    {
        private IEnumerable<Entity> UpdateDeployedObjects()
        {
            var skeles = LocalPlayer
                .GetComponent<Actor>()
                .DeployedObjects
                .Where(x =>
                    x != null &&
                    x.Entity != null &&
                    x.Entity.IsAlive &&
                    x.Entity.Path.Contains("RaisedSkeleton"))
                .Select(x =>
                    x.Entity);

            return skeles;
        }

        private Entity UpdateClosestSummon()
        {
            var chainRangeSqr = Settings.DarkPactChainRange * Settings.DarkPactChainRange;
            var sortedSummons = mySummons
                .Value
                .Where(x =>
                    DistToCursorSqr(x) < chainRangeSqr)
                .ToList();
            sortedSummons.Sort(ClosestToMouseComparison);
            var closestSummon = sortedSummons.FirstOrDefault();
            return closestSummon;
        }

        private int ClosestToMouseComparison(Entity x, Entity y)
        {
            if (DistToCursorSqr(x) < DistToCursorSqr(y)) return -1;
            else return 1;
        }

        private double DistToCursor(Entity x)
        {
            return Math.Sqrt(DistToCursorSqr(x));
        }

        private double DistToCursorSqr(Entity x)
        {
            var cursorPosition = Input.MousePosition;
            var xOnScreen = GameController.IngameState.Camera.WorldToScreen(x.Pos);
            var xToCursorSqr = Helpers.DistanceSquared(xOnScreen, cursorPosition);
            return xToCursorSqr;
        }

        private Element UpdateChatUi()
        {
            return GetChatUi();
        }
    }
}
