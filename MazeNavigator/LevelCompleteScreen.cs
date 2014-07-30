using MazeNavigator.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using XNAUtils.GameComponents;

namespace MazeNavigator
{
    // TODO Need to make other relevant classes derive from ScreenWithInputState.
    /// <summary>
    /// Displays which, if any, medals the player won for the current
    /// level, and lets the player retry or continue to the next level.
    /// </summary>
    internal class LevelCompleteScreen
        : ScreenWithInputState
    {
        private SpriteFont uiFont;
        private Texture2D buttonBackground;
        private Texture2D cursorBackground;

        private Texture2D bronzeMedalBackground;
        private Texture2D silverMedalBackground;
        private Texture2D goldMedalBackground;

        private Texture2D loseMedalBackground;
        private Texture2D winMedalBackground;

        private const int BUTTON_OFFSET_Y = 60;
        private const int BUTTON_PADDING = 20;
        private const int CURSOR_WIDTH = 10;
        private const int CURSOR_HEIGHT = 10;
        private const int MEDAL_WIDTH = 200;
        private const int MEDAL_HEIGHT = 50;
        private const int MEDAL_PADDING_X = 10;
        private const int MEDAL_PADDING_Y = 10;
        private const int BORDER_PADDING = 5;
        private Rectangle CursorBounds
        {
            get
            {
                return new Rectangle(currentMouseState.Position.X,
                                     currentMouseState.Position.Y,
                                     CURSOR_WIDTH,
                                     CURSOR_HEIGHT);
            }
        }

        private Rectangle bronzeMedalBounds
        {
            get
            {
                return new Rectangle(GraphicsDevice.Viewport.Width / 2 - ((int)(MEDAL_WIDTH * 1.5)) - MEDAL_PADDING_X - (2 * BORDER_PADDING),
                                     GraphicsDevice.Viewport.Height / 2 - (GraphicsDevice.Viewport.Height / 3),
                                     MEDAL_WIDTH + MEDAL_PADDING_X,
                                     MEDAL_HEIGHT + MEDAL_PADDING_Y);

            }
        }

        private Rectangle silverMedalBounds
        {
            get
            {
                return new Rectangle(GraphicsDevice.Viewport.Width / 2 - (MEDAL_WIDTH / 2),
                                     GraphicsDevice.Viewport.Height / 2 - (GraphicsDevice.Viewport.Height / 3),
                                     MEDAL_WIDTH + MEDAL_PADDING_X,
                                     MEDAL_HEIGHT + MEDAL_PADDING_Y);
            }
        }

        private Rectangle goldMedalBounds
        {
            get
            {
                return new Rectangle(GraphicsDevice.Viewport.Width / 2 + (MEDAL_WIDTH / 2) + MEDAL_PADDING_X + (2 * BORDER_PADDING),
                                     GraphicsDevice.Viewport.Height / 2 - (GraphicsDevice.Viewport.Height / 3),
                                     MEDAL_WIDTH + MEDAL_PADDING_X,
                                     MEDAL_HEIGHT + MEDAL_PADDING_Y);
            }
        }

        private Rectangle ContinueButtonBounds
        {
            get
            {
                int textWidth = (int)uiFont.MeasureString("Continue").X;
                int textHeight = (int)uiFont.MeasureString("Continue").Y;
                Rectangle continueButtonBounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - (textWidth / 2),
                                                               GraphicsDevice.Viewport.Height / 2 - (textHeight / 2),
                                                               textWidth + BUTTON_PADDING,
                                                               textHeight + BUTTON_PADDING);
                return continueButtonBounds;
            }
        }

        private Rectangle RetryButtonBounds
        {
            get
            {
                int textWidth = (int)uiFont.MeasureString("Retry").X;
                int textHeight = (int)uiFont.MeasureString("Retry").Y;
                Rectangle retryButtonBounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - (textWidth / 2),
                                                            GraphicsDevice.Viewport.Height / 2 - (textHeight / 2) + BUTTON_OFFSET_Y,
                                                            textWidth + BUTTON_PADDING,
                                                            textHeight + BUTTON_PADDING);
                return retryButtonBounds;
            }
        }
        private Rectangle LevelSelectBounds
        {
            get
            {
                int textWidth = (int)uiFont.MeasureString("Level select").X;
                int textHeight = (int)uiFont.MeasureString("Level select").Y;
                Rectangle levelSelectBounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - (textWidth / 2),
                                                            GraphicsDevice.Viewport.Height / 2 - (textHeight / 2) + (2 * BUTTON_OFFSET_Y),
                                                            textWidth + BUTTON_PADDING,
                                                            textHeight + BUTTON_PADDING);

                return levelSelectBounds;
            }
        }

        internal LevelCompleteScreen(MainGame game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            this.uiFont = Game.Content.Load<SpriteFont>("uiFont");

            this.buttonBackground = new Texture2D(GraphicsDevice, 1, 1);
            this.buttonBackground.SetData<Color>(new Color[] { Color.MediumSeaGreen });

            this.cursorBackground = new Texture2D(GraphicsDevice, 1, 1);
            this.cursorBackground.SetData<Color>(new Color[] { Color.LightGreen });

            this.bronzeMedalBackground = new Texture2D(GraphicsDevice, 1, 1);
            this.bronzeMedalBackground.SetData<Color>(new Color[] { Color.DarkRed });

            this.silverMedalBackground = new Texture2D(GraphicsDevice, 1, 1);
            this.silverMedalBackground.SetData<Color>(new Color[] { Color.Silver });

            this.goldMedalBackground = new Texture2D(GraphicsDevice, 1, 1);
            this.goldMedalBackground.SetData<Color>(new Color[] { Color.Gold });

            this.loseMedalBackground = new Texture2D(GraphicsDevice, 1, 1);
            this.loseMedalBackground.SetData<Color>(new Color[] {Color.Red});

            this.winMedalBackground = new Texture2D(GraphicsDevice, 1, 1);
            this.winMedalBackground.SetData<Color>(new Color[] { Color.LightGreen });

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (((MainGame)Game).UIState == MainGame.InterfaceState.LevelComplete)
            {
                if (this.LeftMouseButtonReleased)
                {
                    if (this.ContinueButtonBounds.Contains(this.CursorBounds))
                    {
                        ((MainGame)Game).UIState = MainGame.InterfaceState.PlayingLevel;
                        ((MainGame)Game).LoadNextLevel();
                    }
                    else if (this.RetryButtonBounds.Contains(this.CursorBounds))
                    {
                        ((MainGame)Game).UIState = MainGame.InterfaceState.PlayingLevel;
                        ((MainGame)Game).ReloadCurrentLevel();
                    }
                    else if (this.LevelSelectBounds.Contains(this.CursorBounds))
                    {
                        ((MainGame)Game).UIState = MainGame.InterfaceState.SelectingLevel;
                    }
                }

                // TODO Restore this when appropriate.
                // Move to the next level.
                //LoadLevel(this.Levels[this.Levels.IndexOf(this.CurrentLevel) + 1]);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (((MainGame)Game).UIState == MainGame.InterfaceState.LevelComplete)
            {
                drawMedals(((MainGame)Game).spriteBatch);
                drawContinueButton(((MainGame)Game).spriteBatch);
                drawRetryButton(((MainGame)Game).spriteBatch);
                drawLevelSelectButton(((MainGame)Game).spriteBatch);
                drawCursor(((MainGame)Game).spriteBatch);
            }

            base.Draw(gameTime);
        }

        private void drawMedals(SpriteBatch spriteBatch)
        {
            // TODO Draw win/loss borders.
            // Draw bronze medal.
            spriteBatch.Draw(((((MainGame)Game).CurrentLevel.CompletionStatus.Bronze) ? winMedalBackground : loseMedalBackground),
                             new Rectangle(bronzeMedalBounds.X - BORDER_PADDING,
                                           bronzeMedalBounds.Y - BORDER_PADDING,
                                           bronzeMedalBounds.Width + (2 * BORDER_PADDING),
                                           bronzeMedalBounds.Height + (2 * BORDER_PADDING)),
                             Color.White);
            spriteBatch.Draw(bronzeMedalBackground, bronzeMedalBounds, Color.White);

            // Draw silver medal.
            spriteBatch.Draw(((((MainGame)Game).CurrentLevel.CompletionStatus.Silver) ? winMedalBackground : loseMedalBackground),
                             new Rectangle(silverMedalBounds.X - BORDER_PADDING,
                                           silverMedalBounds.Y - BORDER_PADDING,
                                           silverMedalBounds.Width + (2 * BORDER_PADDING),
                                           silverMedalBounds.Height + (2 * BORDER_PADDING)),
                             Color.White);
            spriteBatch.Draw(silverMedalBackground, silverMedalBounds, Color.White);

            // Draw gold medal.
            spriteBatch.Draw(((((MainGame)Game).CurrentLevel.CompletionStatus.Gold) ? winMedalBackground : loseMedalBackground),
                             new Rectangle(goldMedalBounds.X - BORDER_PADDING,
                                           goldMedalBounds.Y - BORDER_PADDING,
                                           goldMedalBounds.Width + (2 * BORDER_PADDING),
                                           goldMedalBounds.Height + (2 * BORDER_PADDING)),
                             Color.White);
            spriteBatch.Draw(goldMedalBackground, goldMedalBounds, Color.White);
        }

        private void drawContinueButton(SpriteBatch spriteBatch)
        {
            // TODO A button DrawableGameComponent would be nice. Send it your text, background color,
            //      and onMouseOver color.
            spriteBatch.Draw(buttonBackground, ContinueButtonBounds, Color.White);
            spriteBatch.DrawString(uiFont,
                                   "Continue",
                                   new Vector2(ContinueButtonBounds.X + (BUTTON_PADDING / 2),
                                               ContinueButtonBounds.Y + (BUTTON_PADDING / 2)),
                                               (ContinueButtonBounds.Contains(CursorBounds) ? Color.LightBlue : Color.Beige));

            if (ContinueButtonBounds.Contains(CursorBounds))
            {
                MediaPlayer.Play(((MainGame)Game).Utils.MouseOverSound);
            }
        }

        private void drawRetryButton(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonBackground, RetryButtonBounds, Color.White);
            spriteBatch.DrawString(uiFont,
                                   "Retry",
                                   new Vector2(RetryButtonBounds.X + (BUTTON_PADDING / 2),
                                               RetryButtonBounds.Y + (BUTTON_PADDING / 2)),
                                  (RetryButtonBounds.Contains(CursorBounds) ? Color.LightBlue : Color.Beige));

            if (RetryButtonBounds.Contains(CursorBounds))
            {
                MediaPlayer.Play(((MainGame)Game).Utils.MouseOverSound);
            }
        }
        private void drawLevelSelectButton(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonBackground, LevelSelectBounds, Color.White);
            spriteBatch.DrawString(uiFont,
                                   "Level select",
                                   new Vector2(LevelSelectBounds.X + (BUTTON_PADDING / 2),
                                               LevelSelectBounds.Y + (BUTTON_PADDING / 2)),
                                  (LevelSelectBounds.Contains(CursorBounds) ? Color.LightBlue : Color.Beige));

            if (LevelSelectBounds.Contains(CursorBounds))
            {
                MediaPlayer.Play(((MainGame)Game).Utils.MouseOverSound);
            }
        }

        private void drawCursor(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(cursorBackground,
                             this.CursorBounds,
                             Color.White);
        }
    }
}