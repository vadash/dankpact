using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace dankpact
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

        [Menu("Debug")]
        public ToggleNode Debug { get; set; } = new ToggleNode(false);

        [Menu("Activate key")]
        public HotkeyNode ActivateKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.Space);

        [Menu("Summon Skeletons")]
        public HotkeyNode SummonSkeleKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.Q);

        [Menu("Dark Pact")]
        public HotkeyNode DarkPactKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.W);

        [Menu("Dark Pact chain range (pixels)")]
        public RangeNode<int> DarkPactChainRange { get; set; } = new RangeNode<int>(200, 200, 900);
    }
}