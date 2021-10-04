using Godot;

namespace Unstable
{
    public class Player : RigidBody2D
    {
        [Signal]
        public delegate void HeatMilestone(float heatLevel);
        [Signal]
        public delegate void HitCheckpoint(Checkpoint checkpoint);

        public float MaxTorque { get; set; } = 3000f;
        public float Acceleration { get; set; } = 60000f;
        public float TorqueDampening { get; set; } = 0.999f;
        public float Speed { get; set; } = 0f;
        public float HighestSpeedDelta { get; set; } = 0f;
        public bool IsDead { get { return isDead; } }
        public float HeatLevel { get; set; } = 0f;

        private float inputX = 0;
        private float speedDelta = 0f;
        private float lastSpeed = 0f;
        private bool isDead = false;
        private bool isColliding = false;
        private Vector2 collisionPos = Vector2.Zero;
        private Vector2 collisionNormal = Vector2.Zero;
        private bool hasTouch = false;
        private Vector2 touchPos = Vector2.Zero;

        private UnstableGame unstableGame;
        private Sprite sprite;
        private Sprite hightlightSprite;
        private Sprite heatOverlay;
        private Area2D zoneArea;
        private Particles2D smoke;
        private ShaderMaterial smokeShader;
        private Particles2D dust;
        private ShaderMaterial dustShader;
        private AudioStreamPlayer explosionPlayer;
        private AudioStreamPlayer smackPlayer;

        public override void _Ready()
        {
            unstableGame = GetParent<UnstableGame>();
            sprite = GetNode<Sprite>("Sprite");
            hightlightSprite = GetNode<Sprite>("Highlight");
            heatOverlay = GetNode<Sprite>("HeatOverlay");
            smoke = GetNode<Particles2D>("SmokeParticles");
            smokeShader = (ShaderMaterial)smoke.ProcessMaterial;
            dust = GetNode<Particles2D>("DustParticles");
            dustShader = (ShaderMaterial)dust.ProcessMaterial;
            zoneArea = GetNode<Area2D>("ZoneArea");
            explosionPlayer = GetNode<AudioStreamPlayer>("ExplosionPlayer");
            smackPlayer = GetNode<AudioStreamPlayer>("SmackPlayer"); ;

            zoneArea.Connect("area_entered", this, nameof(CheckpointEntered));
        }

        public override void _Input(InputEvent evt)
        {
            base._Input(evt);

            if (evt is InputEventScreenTouch est && est.Index == 0)
            {
                if (est.Pressed)
                {
                    hasTouch = true;
                    touchPos = est.Position;
                }
                else
                {
                    hasTouch = false;
                }
            }
            else if (evt is InputEventScreenDrag esd && esd.Index == 0)
            {
                touchPos = esd.Position;
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (IsDead)
                return;

            if (hasTouch)
            {
                if (touchPos.x < 240)
                {
                    inputX = -1;
                }
                else
                {
                    inputX = 1;
                }
            }
            else
            {
                inputX = 0;
            }

            if (Input.IsActionPressed("g_move_left"))
                inputX -= 1f;

            if (Input.IsActionPressed("g_move_right"))
                inputX += 1f;



            unstableGame.DebugLabel.Text = $"heatLevel = {HeatLevel}\n";

            hightlightSprite.Rotation = -Rotation;
            smoke.Rotation = -Rotation;
            dust.Rotation = -Rotation;

            if (Speed >= 50f)
            {
                AddHeat((Speed - 50f) * delta);
            }
            else
            {
                // Cool down heat
                if (HeatLevel > 0f)
                {
                    AddHeat(-50f * delta);
                }
                else if (HeatLevel < 0f)
                {
                    HeatLevel = 0f;
                }
            }



            smokeShader.SetShaderParam("number_particles_shown", Mathf.Clamp(Mathf.FloorToInt((Speed / 50f) * 2), 0, 8));
            heatOverlay.Modulate = new Color(1f, 1f, 1f, Mathf.Min(HeatLevel / 100f, 1f));

            if (isColliding)
            {
                Vector2 dir = collisionNormal.LinearInterpolate(LinearVelocity.Normalized(), 0.5f);
                dir.x = -dir.x;

                dustShader.SetShaderParam("number_particles_shown", Mathf.Clamp(Mathf.FloorToInt((Speed / 50f) * 12), 0, 12));
                dustShader.SetShaderParam("direction", dir);
                dustShader.SetShaderParam("initial_linear_velocity", (Speed / 50f) * 30f);
                dust.Position = collisionPos;
                dust.Emitting = true;
            }
            else
            {
                dust.Emitting = false;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (isDead)
                return;

            AddTorque(Acceleration * delta * inputX);

            float dir = Mathf.Sign(AppliedTorque);

            if (Mathf.Abs(AppliedTorque) > MaxTorque)
            {
                AppliedTorque = MaxTorque * dir;
            }

            AppliedTorque -= (AppliedTorque * TorqueDampening * delta);

            lastSpeed = Speed;
            Speed = LinearVelocity.Length();

            speedDelta = lastSpeed - Speed;

            if (speedDelta >= 10f)
            {
                float vol = ((speedDelta - 10f) / 30f) * -40f;

                smackPlayer.VolumeDb = vol;
                smackPlayer.Play();
            }

            if (Mathf.Abs(speedDelta) > HighestSpeedDelta)
            {
                HighestSpeedDelta = Mathf.Abs(speedDelta);
            }
        }

        public override void _IntegrateForces(Physics2DDirectBodyState state)
        {
            base._IntegrateForces(state);

            if (state.GetContactCount() > 0)
            {
                collisionPos = state.GetContactLocalPosition(0);
                ((Node2D)state.GetContactColliderObject(0)).ToGlobal(collisionPos);
                collisionPos = ToLocal(collisionPos);
                collisionNormal = state.GetContactLocalNormal(0);
                isColliding = true;
            }
            else
            {
                isColliding = false;
            }
        }

        public void Kill()
        {
            isDead = true;

            explosionPlayer.Play();

            HeatLevel = 0f;
            HighestSpeedDelta = 0f;
            Speed = 0f;

            GravityScale = 0f;
            Mode = ModeEnum.Static;
            LinearVelocity = Vector2.Zero;
            AppliedForce = Vector2.Zero;
            AppliedTorque = 0f;
            sprite.Visible = false;
            hightlightSprite.Visible = false;
            heatOverlay.Visible = false;

            GlobalInputEvents.DeathCount++;
        }

        public void Reset()
        {
            isDead = false;

            GravityScale = 1f;
            Mode = ModeEnum.Rigid;
            sprite.Visible = true;
            hightlightSprite.Visible = true;
            heatOverlay.Visible = true;
        }

        private void CheckpointEntered(Area2D area)
        {
            if (area is Checkpoint checkpoint)
            {
                EmitSignal(nameof(HitCheckpoint), checkpoint);
            }
        }

        public void AddHeat(float heat)
        {
            float lastHeatLevel = HeatLevel;

            HeatLevel += heat;

            if (HeatLevel < 0f)
                HeatLevel = 0f;

            // Heat milestones
            if (lastHeatLevel < 25f && HeatLevel >= 25f)
            {
                EmitSignal(nameof(HeatMilestone), 25f);
            }
            else if (lastHeatLevel < 50f && HeatLevel >= 50f)
            {
                EmitSignal(nameof(HeatMilestone), 50f);
            }
            else if (lastHeatLevel < 75f && HeatLevel >= 75f)
            {
                EmitSignal(nameof(HeatMilestone), 75f);
            }
            else if (lastHeatLevel < 100f && HeatLevel >= 100f)
            {
                EmitSignal(nameof(HeatMilestone), 100f);
            }
        }

    }
}