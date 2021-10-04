using Godot;

namespace Unstable
{
    public class EndScreen : InfoScreen
    {
        private Node2D shakeText;
        private bool canShake = true;
        private Label deathCount;

        public override void _Ready()
        {
            base._Ready();

            shakeText = GetNode<Node2D>("ShakeText");
            deathCount = GetNode<Label>("DeathcountLabel");

            GetNode<GlobalMusic>("/root/GlobalMusic").PlayEndTrack();
            deathCount.Text = $"Your bomb rolling skills are truly impeccable. And you only died {GlobalInputEvents.DeathCount} times! Thats good right? Probably. Yeah.";
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (canShake)
                shakeText.Position = new Vector2(Helpers.Rng.RandfRange(-1.5f, 1.5f), Helpers.Rng.RandfRange(-1.5f, 1.5f));

            canShake = !canShake;
        }
    }
}