using Godot;

namespace Unstable
{
    public class InfoScreen : Node2D
    {
        private PackedScene mainMenuScene;

        [Export]
        private NodePath returnButton;

        public override void _Ready()
        {
            mainMenuScene = GD.Load<PackedScene>("res://scenes/MainMenu.tscn");

            if (Input.GetConnectedJoypads().Count > 0)
                GetNode<TextureButton>(returnButton).GrabFocus();

            GetNode<TextureButton>(returnButton).Connect("pressed", this, nameof(GotoMainMenu));
        }

        private void GotoMainMenu()
        {
            GetTree().ChangeSceneTo(mainMenuScene);

            QueueFree();
        }
    }
}
