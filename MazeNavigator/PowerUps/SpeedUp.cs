using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeNavigator.PowerUps
{
    internal class SpeedUp
        : PowerUp
    {
        internal SpeedUp(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            this.PowerUpTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.PowerUpTexture.SetData<Color>(new Color[] { Color.Blue });
        }

        // TODO Cool effect: display a shadow of gradually decreasing intensity behind the block.
        //      Will make a cool "swooshing" effect to show off acceleration.
        public override void Draw(GameTime gameTime)
        {
            ((MainGame)Game).spriteBatch.Draw(this.PowerUpTexture,
                                              this.Bounds,
                                              Color.White);

            base.Draw(gameTime);
        }
    }
}