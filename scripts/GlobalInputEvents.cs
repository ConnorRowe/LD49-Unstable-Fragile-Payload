using Godot;

namespace Unstable
{
    public class GlobalInputEvents : Node
    {
        public static int DeathCount { get; set; } = 0;

        private PackedScene mainMenuScene;

        public override void _Ready()
        {
            base._Ready();
            mainMenuScene = GD.Load<PackedScene>("res://scenes/MainMenu.tscn");
        }

        public override void _Input(InputEvent evt)
        {
            base._Input(evt);

            if (evt is InputEventKey ek && !ek.Pressed && (ek.Scancode == (uint)KeyList.F11 || (ek.Scancode == (uint)KeyList.Enter && ek.Alt)))
            {
                OS.WindowFullscreen = !OS.WindowFullscreen;
            }

            if (evt.IsActionReleased("g_return_to_mainmenu"))
            {
                GetTree().CurrentScene.QueueFree();

                GetTree().ChangeSceneTo(mainMenuScene);
            }
        }
    }
}