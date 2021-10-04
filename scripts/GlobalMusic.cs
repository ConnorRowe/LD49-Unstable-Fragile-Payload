using Godot;

namespace Unstable
{
    public class GlobalMusic : Node
    {
        [Export]
        private AudioStreamOGGVorbis mainTrack;
        [Export]
        private AudioStreamOGGVorbis endTrack;

        private AudioStreamPlayer player;

        public override void _Ready()
        {
            player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        }

        public void PlayMainTrack()
        {
            if (player.Stream != mainTrack)
            {
                player.Stream = mainTrack;
                player.Play();
            }
        }

        public void PlayEndTrack()
        {
            player.Stream = endTrack;
            player.Play();
        }
    }
}