using Godot;
using System;

namespace Unstable
{
    public class FinishZone : Area2D
    {
        private PackedScene endScene;

        public override void _Ready()
        {
            endScene = GD.Load<PackedScene>("res://scenes/EndScreen.tscn");

            Connect("area_entered", this, nameof(AreaEntered));
        }

        private void AreaEntered(Area2D area)
        {
            if (area.GetParent() is Player p)
            {
                GotoEndScreen();
            }
        }

        private void GotoEndScreen()
        {
            GetTree().ChangeSceneTo(endScene);

            GetParent().QueueFree();
        }
    }
}