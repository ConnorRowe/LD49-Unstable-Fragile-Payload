using Godot;

namespace Unstable
{
    public class MushroomCloud : AnimatedSprite
    {
        public override void _Ready()
        {
            Connect("animation_finished", this, nameof(AnimationFinished));

            Play("start");
        }

        private void AnimationFinished()
        {
            if (Animation == "start")
            {
                Play("loop");
            }

            if (Helpers.Rng.Randf() >= .5f)
            {
                FlipH = !FlipH;
            }
        }
    }
}