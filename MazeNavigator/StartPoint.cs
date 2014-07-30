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
    public class StartPoint : PowerUps.PowerUp
    {
        public const int WIDTH = 10;
        public const int HEIGHT = 10;

        public StartPoint(Game game, Vector2 Position) : base(game)
        {
            this.Position = Position;
        }

        public override void Initialize()
        {
            // TODO I should make an InputStateScreen : DrawableGameComponent class in XNAUtils.
            //      This class simply maintains the current and previous frame's KeyboardState and MouseState
            //      in a set of variables.
            base.Initialize();
            this.Bounds = new Rectangle((int)Position.X,
                                        (int)Position.Y,
                                        StartPoint.WIDTH,
                                        StartPoint.HEIGHT);
        }

        protected override void LoadContent()
        {
            this.PowerUpTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.PowerUpTexture.SetData<Color>(new Color[] { Color.OrangeRed });

            base.LoadContent();
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
