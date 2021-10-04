using Godot;

namespace Unstable
{
    public class Checkpoint : Area2D
    {
        private Sprite sprite;
        private AnimationPlayer animPlayer;
        private bool idle = true;
        private UnstableGame unstableGame;
        private AudioStream checkpointStream;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            checkpointStream = GD.Load<AudioStream>("res://audio/sfx/checkpoint.wav");
            unstableGame = (UnstableGame)GetTree().CurrentScene;
        }

        public void Toggle()
        {
            idle = !idle;
            sprite.RegionRect = new Rect2(new Vector2(idle ? 0f : 32f, 0f), new Vector2(32f, 32f));
            animPlayer.PlaybackSpeed = idle ? 1f : 0f;

            if (!idle)
            {
                unstableGame.PlayerOtherEffect(checkpointStream);
            }
        }
    }
}