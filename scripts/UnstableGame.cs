using Godot;

namespace Unstable
{
    public class UnstableGame : Node2D
    {
        public Vector2 CameraOffset { get; set; } = new Vector2(0, -64f);
        public Label DebugLabel { get; set; }

        private Vector2 baseRespawnPoint;

        private Camera2D camera;
        private Player player;
        private AudioStreamPlayer heatBeepPlayer;
        private MushroomCloud lastShroom;
        private PlayerDeadChunks lastChunks;
        private Checkpoint currentCheckpoint = null;
        private SceneTreeTimer resetTimer = null;
        private Tween tween;
        private Node2D deathIndicator;
        private AudioStreamPlayer otherEffectPlayer;

        private PackedScene mushroomScene;
        private PackedScene pDeadChunksScene;
        private AudioStreamSample nin25;
        private AudioStreamSample nin50;
        private AudioStreamSample nin75;

        public override void _Ready()
        {
            camera = GetNode<Camera2D>("Camera2D");
            player = GetNode<Player>("Player");
            DebugLabel = GetNode<Label>("CanvasLayer/DebugLabel");
            heatBeepPlayer = GetNode<AudioStreamPlayer>("HeatBeepPlayer");
            tween = GetNode<Tween>("Tween");
            deathIndicator = GetNode<Node2D>("CanvasLayer/DeathIndicator");
            otherEffectPlayer = GetNode<AudioStreamPlayer>("OtherEffectPlayer");

            mushroomScene = GD.Load<PackedScene>("res://scenes/MushroomCloud.tscn");
            pDeadChunksScene = GD.Load<PackedScene>("res://scenes/PlayerDeadChunks.tscn");
            nin25 = GD.Load<AudioStreamSample>("res://audio/sfx/nin25.wav");
            nin50 = GD.Load<AudioStreamSample>("res://audio/sfx/nin50.wav");
            nin75 = GD.Load<AudioStreamSample>("res://audio/sfx/nin75.wav");

            player.Connect(nameof(Player.HeatMilestone), this, nameof(HeatMilestone));
            player.Connect(nameof(Player.HitCheckpoint), this, nameof(PlayerHitCheckpoint));

            baseRespawnPoint = player.GlobalPosition;

            Node2D objective1 = GetNode<Node2D>("CanvasLayer/Objective1");
            Node2D objective2 = GetNode<Node2D>("CanvasLayer/Objective2");
            tween.InterpolateProperty(objective1, "scale", Vector2.Zero, Vector2.One, 1f, Tween.TransitionType.Cubic, delay: 0.5f);
            tween.InterpolateProperty(objective2, "scale", Vector2.Zero, Vector2.One, 1f, Tween.TransitionType.Cubic, delay: 1f);

            tween.InterpolateProperty(objective1, "scale", Vector2.One, Vector2.Zero, 0.85f, Tween.TransitionType.Cubic, delay: 7f);
            tween.InterpolateProperty(objective2, "scale", Vector2.One, Vector2.Zero, 0.85f, Tween.TransitionType.Cubic, delay: 7f);

            tween.Start();

            GlobalInputEvents.DeathCount = 0;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            camera.GlobalPosition = player.GlobalPosition + CameraOffset;

            if ((player.HighestSpeedDelta >= 40f || player.HeatLevel >= 100f) && !player.IsDead)
            {
                ExplodePlayer();
            }
        }

        public override void _Input(InputEvent evt)
        {
            base._Input(evt);

            if (evt.IsActionReleased("g_reset") || (evt is InputEventScreenTouch est && est.Pressed && player.IsDead))
            {
                if (player.IsDead)
                {
                    Reset();
                }
            }

            if (evt.IsActionPressed("g_hold_reset"))
            {
                StartResetTimer();
            }

            if (evt.IsActionReleased("g_hold_reset"))
            {
                CancelResetTimer();
            }
        }

        private void StartResetTimer()
        {
            resetTimer = GetTree().CreateTimer(3f);
            resetTimer.Connect("timeout", this, nameof(Reset));
        }

        private void CancelResetTimer()
        {
            resetTimer.Disconnect("timeout", this, nameof(Reset));
            resetTimer = null;
        }

        private void ExplodePlayer()
        {
            player.Kill();

            lastShroom = mushroomScene.Instance<MushroomCloud>();
            AddChild(lastShroom);
            lastShroom.GlobalPosition = player.GlobalPosition;

            lastChunks = pDeadChunksScene.Instance<PlayerDeadChunks>();
            AddChild(lastChunks);
            lastChunks.GlobalPosition = player.GlobalPosition;

            tween.InterpolateProperty(deathIndicator, "scale", Vector2.Zero, Vector2.One, .5f, Tween.TransitionType.Elastic);
            tween.Start();
        }

        private void Reset()
        {
            if (IsInstanceValid(lastShroom))
                lastShroom.QueueFree();
            if (IsInstanceValid(lastChunks))
                lastChunks?.QueueFree();

            if (currentCheckpoint != null)
            {
                player.GlobalPosition = currentCheckpoint.GlobalPosition;
            }
            else
            {
                player.GlobalPosition = baseRespawnPoint;
            }

            player.Reset();

            tween.InterpolateProperty(deathIndicator, "scale", Vector2.One, Vector2.Zero, .5f, Tween.TransitionType.Elastic);
            tween.Start();
        }

        private void HeatMilestone(float heatLevel)
        {
            if (heatLevel == 25f)
            {
                heatBeepPlayer.Stream = nin25;
                heatBeepPlayer.Play(0);
            }
            else if (heatLevel == 50f)
            {
                heatBeepPlayer.Stream = nin50;
                heatBeepPlayer.Play(0);
            }
            else if (heatLevel == 75f)
            {
                heatBeepPlayer.Stream = nin75;
                heatBeepPlayer.Play(0);
            }
            else if (heatLevel == 100f && !player.IsDead)
            {
                ExplodePlayer();
            }
        }

        private void PlayerHitCheckpoint(Checkpoint checkpoint)
        {
            if (currentCheckpoint != null)
            {
                currentCheckpoint.Toggle();
            }

            checkpoint.Toggle();
            currentCheckpoint = checkpoint;
        }

        public void PlayerOtherEffect(AudioStream audioStream)
        {
            otherEffectPlayer.Stream = audioStream;
            otherEffectPlayer.Play();
        }
    }
}