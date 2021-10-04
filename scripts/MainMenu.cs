using Godot;

namespace Unstable
{
    public class MainMenu : Node2D
    {
        private PackedScene gameScene;
        private PackedScene controlsScene;
        private PackedScene settingsScene;

        public override void _Ready()
        {
            gameScene = GD.Load<PackedScene>("res://scenes/UnstableGame.tscn");
            controlsScene = GD.Load<PackedScene>("res://scenes/ControlsScreen.tscn");
            settingsScene = GD.Load<PackedScene>("res://scenes/SettingsScreen.tscn");

            GetNode<TextureButton>("PlayButton").GrabFocus();
            GetNode<TextureButton>("PlayButton").Connect("pressed", this, nameof(Play));
            GetNode<TextureButton>("ControlsButton").Connect("pressed", this, nameof(GotoControls));
            GetNode<TextureButton>("SettingsButton").Connect("pressed", this, nameof(GotoSettings));

            GetNode<GlobalMusic>("/root/GlobalMusic").PlayMainTrack();
        }

        private void Play()
        {
            GetTree().ChangeSceneTo(gameScene);

            QueueFree();
        }

        private void GotoControls()
        {
            GetTree().ChangeSceneTo(controlsScene);

            QueueFree();
        }

        private void GotoSettings()
        {
            GetTree().ChangeSceneTo(settingsScene);

            QueueFree();
        }
    }
}
