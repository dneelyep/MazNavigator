using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeNavigator
{
    /// <summary>
    /// A location that the player must contact in order to complete the current level.
    /// </summary>
    public class EndPoint : PowerUps.PowerUp
    {
        public const int WIDTH = 10;
        public const int HEIGHT = 10;

        public EndPoint(Game game, Vector2 Position) : base(game)
        {
            this.Position = Position;
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Bounds = new Rectangle((int)this.Position.X,
                                        (int)this.Position.Y,
                                        EndPoint.WIDTH,
                                        EndPoint.HEIGHT);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            this.PowerUpTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.PowerUpTexture.SetData<Color>(new Color[] { Color.Aquamarine });
        }

        public override void Draw(GameTime gameTime)
        {
            ((MainGame)Game).spriteBatch.Draw(this.PowerUpTexture,
                                              this.Bounds,
                                              Color.White);

            base.Draw(gameTime);
        }
    }
}
