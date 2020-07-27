using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace dankpact
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

        [Menu("Activate key")]
        public HotkeyNode ActivateKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.Space);

        [Menu("Summon Skeletons")]
        public HotkeyNode SummonSkeleKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.Q);

        [Menu("Dark Pact")]
        public HotkeyNode DarkPactKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.W);


    }
}