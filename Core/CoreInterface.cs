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
        TimeCache<Element> chatUi;

        private IEnumerable<Element> GetVisibleUi()
        {
            var elements = GameController?.IngameState?.IngameUi?.Children?.Where(
                x =>
                    x?.IsValid == true &&
                    x?.IsVisible == true &&
                    x?.IsVisibleLocal == true);
            return elements ?? new List<Element>();
        }

        private Element GetChatUi()
        {
            var value = GetVisibleUi().FirstOrDefault(
                x => 
                    x?.ChildCount == 5 &&
                    x?.Children?[0]?.ChildCount == 2 &&
                    x?.Children?[1]?.ChildCount == 3 &&
                    x?.Children?[2]?.ChildCount == 4);
            return value;
        }

        private bool ChatIsOpened()
        {
            return chatUi.Value?.Children?[3]?.IsVisible == true;
        }
    }
}
