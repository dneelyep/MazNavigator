using MazeNavigator.PowerUps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace MazeNavigator
{
    /// <summary>
    /// Class that represents the player's "character"/"cursor" in the game.
    /// </summary>
    internal class Player
        : DrawableGameComponent
    {
        internal Vector2 Position;

        /// <summary>
        /// The texture that visually represents the player in the game.
        /// </summary>
        private Texture2D PlayerTexture;

        /// <summary>
        /// The player's acceleration in the X and Y directions.
        /// </summary>
        internal Vector2 Acceleration = new Vector2(0, 0);

        /// <summary>
        /// The player's velocity in the X and Y directions.
        /// </summary>
        internal Vector2 Velocity = new Vector2(0, 0);

        private const int TEXTURE_WIDTH = 10;
        private const int TEXTURE_HEIGHT = 10;

        private Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)this.Position.X, (int)this.Position.Y, TEXTURE_WIDTH, TEXTURE_HEIGHT);
            }
        }

        private List<PowerUp> PowerUps;

        internal Player(MainGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.PowerUps = new List<PowerUp>();
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
            this.PlayerTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.PlayerTexture.SetData<Color>(new Color[] { Color.Yellow });
        }

        public override void Draw(GameTime gameTime)
        {
            if (((MainGame)Game).UIState == MainGame.InterfaceState.PlayingLevel)
            {
                ((MainGame)Game).spriteBatch.Draw(this.PlayerTexture,
                                                  this.Bounds,
                                                  Color.White);
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (((MainGame)Game).UIState == MainGame.InterfaceState.PlayingLevel)
            {
                UpdateMovement();
                CheckForCollision();
                CheckForScreenBounds();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Use the player's current position, velocity, and acceleration to determine
        /// its next pos/vel/accel.
        /// </summary>
        private void UpdateMovement()
        {
            this.Position += this.Velocity;
            this.Velocity += this.Acceleration;
            this.Acceleration = Vector2.Zero; // Since the acceleration has been applied to the object,
                                              // the acceleration now turns to zero.
        }

        /// <summary>
        /// Check for collision with various objects in
        /// the game world, and react appropriately.
        /// </summary>
        private void CheckForCollision()
        {
            for (int intPowerUp = 0; intPowerUp < ((MainGame)this.Game).PowerUps.Count; intPowerUp++)
            {
                if (this.Bounds.Intersects(((MainGame)this.Game).PowerUps[intPowerUp].Bounds))
                {
                    AddPowerUp(((MainGame)this.Game).PowerUps[intPowerUp]);
                }

                if (this.Bounds.Intersects(((MainGame)this.Game).CurrentLevel.EndPoint.Bounds))
                {
                    ((MainGame)this.Game).CurrentLevelEndReached();
                }
            }
        }

        private void CheckForScreenBounds()
        {
            if (    this.Bounds.X < 0
                || (this.Bounds.X + this.Bounds.Width) > Game.GraphicsDevice.Viewport.Width)
            {
                this.Acceleration.X = -this.Acceleration.X;
                this.Velocity.X     = -this.Velocity.X;
            }
                
            if (    this.Bounds.Y < 0
                || (this.Bounds.Y + this.Bounds.Height) > Game.GraphicsDevice.Viewport.Height)
            {
                this.Acceleration.Y = -this.Acceleration.Y;
                this.Velocity.Y = -this.Velocity.Y;
            }
        }

        private void AddPowerUp(PowerUp powerUp)
        {
            // SpeedUps are activated immediately.
            if (powerUp.GetType() == typeof(SpeedUp))
            {
                powerUp.ActivationStartTime = DateTime.Now;
                this.PowerUps.Add(powerUp);
                ((MainGame)this.Game).PowerUps.Remove(powerUp);
                StartSpeedupDeactivationTimer();
                Constants.NUDGE_FACTOR *= 1.2f;
            }
        }

        /// <summary>
        /// Creates and runs a Timer that waits a pre-determined amount of time before deactivating
        /// a SpeedUp PowerUp applied to this Player.
        /// </summary>
        private void StartSpeedupDeactivationTimer()
        {
            Timer deactivationTimer = new Timer(Constants.SPEEDUP_DURATION_MS);
            deactivationTimer.Elapsed += deactivateSpeedUp;
            deactivationTimer.Start();
        }

        private static void deactivateSpeedUp(Object source, ElapsedEventArgs e)
        {
            Constants.NUDGE_FACTOR /= 1.2f;
            ((Timer)source).Stop();
            ((Timer)source).Dispose();
        }
    }
}