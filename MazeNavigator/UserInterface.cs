using MazeNavigator.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNAUtils.Drawing;

namespace MazeNavigator
{
    internal class UserInterface
        : DrawableGameComponent
    {
        private const int MEDAL_TIME_RADIUS = 20;

        private Texture2D LifePresentTexture;
        private Texture2D LifeAbsentTexture;
        private MainGame game;

        internal UserInterface(MainGame game)
            : base(game)
        {
            this.game = game;
        }

        protected override void LoadContent()
        {
            this.LifePresentTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.LifePresentTexture.SetData<Color>(new Color[] { Color.Pink });

            this.LifeAbsentTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.LifeAbsentTexture.SetData<Color>(new Color[] { Color.Gray });

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (((MainGame)Game).UIState == MainGame.InterfaceState.PlayingLevel)
            {
                drawCompletionTimes(gameTime, this.game.CurrentLevel);
            }

            base.Draw(gameTime);
        }

        // TODO Play around with particle effects.
        private void drawCompletionTimes(GameTime gameTime, Level level)
        {
            Texture2D goldTexture, silverTexture, bronzeTexture; 
            float bronzeCompletionDegrees = (float) ((level.TimePlayed / level.CompletionTimes.Bronze) * 360);
            float silverCompletionDegrees = (float) ((level.TimePlayed / level.CompletionTimes.Silver) * 360);
            float goldCompletionDegrees   = (float) ((level.TimePlayed / level.CompletionTimes.Gold)   * 360);

            if (goldCompletionDegrees > 360f)
            {
                goldTexture = Drawing.GetCircleTexture(MEDAL_TIME_RADIUS, Color.DarkRed, GraphicsDevice);
            }
            else
            {
                goldTexture = Drawing.GetCircleTextureMissingChunk(MEDAL_TIME_RADIUS, new Color(Color.Gold, 50), 0, goldCompletionDegrees, GraphicsDevice);
            }

            if (silverCompletionDegrees > 360f)
            {
                silverTexture = Drawing.GetCircleTexture(MEDAL_TIME_RADIUS, Color.DarkRed, GraphicsDevice);
            }
            else
            {
                silverTexture = Drawing.GetCircleTextureMissingChunk(MEDAL_TIME_RADIUS, new Color(Color.Silver, 50), 0, silverCompletionDegrees, GraphicsDevice);
            }

            if (bronzeCompletionDegrees > 360f)
            {
                bronzeTexture = Drawing.GetCircleTexture(MEDAL_TIME_RADIUS, Color.DarkRed, GraphicsDevice);
            }
            else
            {
                bronzeTexture = Drawing.GetCircleTextureMissingChunk(MEDAL_TIME_RADIUS, new Color(Color.OrangeRed, 50), 0, bronzeCompletionDegrees, GraphicsDevice);
            }

            this.game.spriteBatch.Draw(goldTexture, new Vector2(10, 10), Color.White);
            this.game.spriteBatch.Draw(silverTexture, new Vector2(60, 10), Color.White);
            this.game.spriteBatch.Draw(bronzeTexture, new Vector2(110, 10), Color.White);
        }
    }
}