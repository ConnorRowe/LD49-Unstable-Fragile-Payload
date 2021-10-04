using Godot;

namespace Unstable
{
    public class Helpers
    {

        public static RandomNumberGenerator Rng = new RandomNumberGenerator();

        static Helpers()
        {
            Rng.Randomize();
        }
    }
}