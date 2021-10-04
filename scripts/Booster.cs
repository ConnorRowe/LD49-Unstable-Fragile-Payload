using Godot;

namespace Unstable
{
    public class Booster : Area2D
    {
        [Export]
        public float BoostPower { get; set; } = 200f;

        private bool canBoost = true;
        private UnstableGame unstableGame;
        private AudioStream boostStream;

        public override void _Ready()
        {
            Connect("area_entered", this, nameof(AreaEntered));
            boostStream = GD.Load<AudioStream>("res://audio/sfx/boost.wav");
            unstableGame = (UnstableGame)GetTree().CurrentScene;
        }

        private Vector2 GetBoostDirection()
        {
            return Vector2.Right.Rotated(Rotation);
        }

        private void Reset()
        {
            Modulate = Colors.White;

            canBoost = true;
        }

        private void BoostPlayer(Player player)
        {
            player.ApplyCentralImpulse(GetBoostDirection() * BoostPower);

            canBoost = false;
            Modulate = new Color(1f, .8f, .8f, .5f);

            GetTree().CreateTimer(2f).Connect("timeout", this, nameof(Reset));

            unstableGame.PlayerOtherEffect(boostStream);
        }

        private void AreaEntered(Area2D area)
        {
            if (area.GetParent() is Player p && canBoost)
            {
                BoostPlayer(p);
            }
        }
    }
}