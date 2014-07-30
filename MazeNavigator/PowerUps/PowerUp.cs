using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeNavigator.PowerUps
{
    public class PowerUp
        : DrawableGameComponent
    {
        public Vector2 Position;

        /// <summary>
        /// The time when this PowerUp was activated.
        /// </summary>
        internal DateTime ActivationStartTime;

        // TODO A class that's a child of DrawableGameComponent, that contains a position and a texture, would be useful.
        /// <summary>
        /// The texture that visually represents this PowerUp in the game.
        /// </summary>
        public Texture2D PowerUpTexture;

        internal Rectangle Bounds;

        internal PowerUp(Game game)
            : base(game)
        {
        }
        
        public override void Initialize()
        {
            this.Bounds = new Rectangle((int)this.Position.X, (int)this.Position.Y, 15, 15);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.Bounds = new Rectangle((int)this.Position.X, (int)this.Position.Y, 15, 15);
        }
    }
}
