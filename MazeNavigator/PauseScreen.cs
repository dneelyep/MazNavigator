using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeNavigator
{
    internal class PauseScreen
        : DrawableGameComponent
    {
        private const int BACKGROUND_INLAY = 20;

        private Texture2D backgroundTexture;
        private Rectangle backgroundTextureBounds;
        private SpriteFont uiFont;

        private Rectangle levelSelectBounds;
        private Rectangle resumeGameBounds;
        private Rectangle quitBounds;

        private bool levelSelectCollision;
        private bool resumeCollision;
        private bool quitCollision;
        private bool mouseOverPlaying = false;

        private MouseState currentMouseState;
        private MouseState previousMouseState;

        internal PauseScreen(MainGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            this.backgroundTextureBounds = new Rectangle(BACKGROUND_INLAY,
                                                         BACKGROUND_INLAY,
                                                         GraphicsDevice.Viewport.Width - (BACKGROUND_INLAY * 2),
                                                         GraphicsDevice.Viewport.Height - (BACKGROUND_INLAY * 2));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.backgroundTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.backgroundTexture.SetData<Color>(new Color[] { Color.DarkOrange });

            this.uiFont = Game.Content.Load<SpriteFont>("uiFont");

            resumeGameBounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - (int)(uiFont.MeasureString("Resume game").X / 2),
                                             GraphicsDevice.Viewport.Height / 2,
                                             (int)uiFont.MeasureString("Resume game").X,
                                             (int)uiFont.MeasureString("Resume game").Y);

            levelSelectBounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - (int)(uiFont.MeasureString("Return to Level Select").X / 2),
                                              GraphicsDevice.Viewport.Height / 2 + (int)(uiFont.MeasureString("Return to Level Select").Y),
                                              (int)uiFont.MeasureString("Return to Level Select").X,
                                              (int)uiFont.MeasureString("Return to Level Select").Y);

            quitBounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - (int)(uiFont.MeasureString("Quit").X / 2),
                                       GraphicsDevice.Viewport.Height / 2 + (int)(2 * (uiFont.MeasureString("Quit").Y)),
                                       (int)uiFont.MeasureString("Quit").X,
                                       (int)uiFont.MeasureString("Quit").Y);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.previousMouseState = this.currentMouseState;
            this.currentMouseState = Mouse.GetState();

            checkForButtonCollision();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ((MainGame)Game).spriteBatch.Draw(backgroundTexture, backgroundTextureBounds, Color.OliveDrab);

            drawLevelSelectButton();
            drawResumeButton();
            drawQuitButton();
            drawCursor();

            base.Draw(gameTime);
        }

        private void drawLevelSelectButton()
        {
            Color textColor = (levelSelectCollision ? Color.Yellow : Color.White);
            if (levelSelectBounds.Contains(((MainGame)Game).Utils.CursorBounds) && !mouseOverPlaying)
            {
                MediaPlayer.Play(((MainGame)Game).Utils.MouseOverSound);
                mouseOverPlaying = true;
            }
            else
            {
                mouseOverPlaying = false;
            }

            ((MainGame)Game).spriteBatch.DrawString(uiFont,
                                                    "Return to Level Select",
                                                    new Vector2(levelSelectBounds.X, levelSelectBounds.Y),
                                                    textColor);
        }
        private void drawResumeButton()
        {
            if (resumeGameBounds.Contains(((MainGame)Game).Utils.CursorBounds) && !mouseOverPlaying)
            {
                MediaPlayer.Play(((MainGame)Game).Utils.MouseOverSound);
                mouseOverPlaying = true;
            }
            else
            {
                mouseOverPlaying = false;
            }

            Color textColor = (resumeCollision ? Color.Yellow : Color.White);
            ((MainGame)Game).spriteBatch.DrawString(uiFont,
                                                    "Resume game",
                                                    new Vector2(resumeGameBounds.X, resumeGameBounds.Y),
                                                    textColor);
        }
        private void drawQuitButton()
        {
            if (quitBounds.Contains(((MainGame)Game).Utils.CursorBounds) && !mouseOverPlaying)
            {
                MediaPlayer.Play(((MainGame)Game).Utils.MouseOverSound);
                mouseOverPlaying = true;
            }
            else
            {
                mouseOverPlaying = false;
            }

            Color textColor = (quitCollision ? Color.Yellow : Color.White);
            ((MainGame)Game).spriteBatch.DrawString(uiFont,
                                                    "Quit",
                                                    new Vector2(quitBounds.X, quitBounds.Y),
                                                    textColor);
        }
        private void drawCursor()
        {
            ((MainGame)Game).spriteBatch.Draw(((MainGame)Game).Utils.CursorTexture,
                                              ((MainGame)Game).Utils.CursorBounds,
                                              Color.White);
        }

        /// <summary>
        /// Checks to see if the cursor is currently moused-over
        /// any of the buttons on this pause screen.
        /// </summary>
        private void checkForButtonCollision()
        {
            // Check the level select button.
            levelSelectCollision = (levelSelectBounds.Contains(((MainGame)Game).Utils.CursorBounds));

            // Check the resume play button.
            resumeCollision = (resumeGameBounds.Contains(((MainGame)Game).Utils.CursorBounds));

            // Check the quit button.
            quitCollision = (quitBounds.Contains(((MainGame)Game).Utils.CursorBounds));

            // Check for the player clicking a menu item.
            if (this.currentMouseState.LeftButton == ButtonState.Released && 
                this.previousMouseState.LeftButton == ButtonState.Pressed)
            {
                if (levelSelectCollision)
                {
                    ((MainGame)Game).UIState = MainGame.InterfaceState.SelectingLevel;
                }
                else if (resumeCollision)
                {
                    ((MainGame)Game).UIState = MainGame.InterfaceState.PlayingLevel;
                }
                else if (quitCollision)
                {
                    ((MainGame)Game).Quit();
                }
            }
        }
    }
}
