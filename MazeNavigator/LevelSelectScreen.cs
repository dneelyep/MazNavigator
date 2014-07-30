using MazeNavigator.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MazeNavigator
{
    /// <summary>
    /// Provides a UI, allowing the user to switch between 
    /// and play levels in the game.
    /// </summary>
    internal class LevelSelectScreen
        : DrawableGameComponent
    {
        private const int HEADER_HEIGHT = 50;
        private const int SCROLLBAR_WIDTH = 40;
        private const int SCROLLBAR_BUTTON_HEIGHT = 40;
        private const int INLAY_INSET = 5;
        private readonly Color INLAY_DEFAULT_COLOR = Color.Purple;

        private ButtonState previousMouseState;
        private ButtonState currentMouseState;

        private int LevelGridWidth
        {
            get
            {
                return ((GraphicsDevice.Viewport.Width - SCROLLBAR_WIDTH) / 3);
            }
        }

        private int LevelGridHeight
        {
            get
            {
                return ((GraphicsDevice.Viewport.Height - HEADER_HEIGHT) / 3);
            }
        }

#region "Textures, SpriteFonts"
        private SpriteFont uiFont;
        private Texture2D headerBackground;
        private Texture2D bodyBackground;
        private Texture2D scrollbarBackground;
        private Texture2D cursorTexture;
        private Texture2D levelBackground;
        private Texture2D levelInlay;
        private Texture2D levelBronzeInlay;
        private Texture2D levelSilverInlay;
        private Texture2D levelGoldInlay;
        private Texture2D scrollbarButtonBackground;
        private Texture2D bronzeMedalBackground;
        private Texture2D silverMedalBackground;
        private Texture2D goldMedalBackground;
#endregion
        private bool buttonReleased
        {
            get 
            {
                return (previousMouseState == ButtonState.Pressed &&
                        currentMouseState == ButtonState.Released);
            }
        }

        /// <summary>
        /// An index that specifies which Levels appear at the
        /// top of the screen on this LevelSelector.
        /// </summary>
        private int TopLevelIndex;

        private int MedalHeight
        {
            get
            {
                return LevelGridHeight / 3;
            }
        }

        private int MedalWidth
        {
            get
            {
                return (LevelGridWidth / 3) - ((2 * INLAY_INSET) / 3);
            }
        }

        /// <summary>
        /// How much a medal should be offset from its Level's top edge to appear correctly.
        /// </summary>
        private int MedalOffset
        {
            get
            {
                return HEADER_HEIGHT - INLAY_INSET + LevelGridHeight - MedalHeight;
            }
        }

        internal LevelSelectScreen(MainGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            TopLevelIndex = 0;
            currentMouseState = Mouse.GetState().LeftButton;
            previousMouseState = currentMouseState;
            base.Initialize();
        }

        // TODO I need to handle updating the TopLevelIndex in the Update method depending on button clicks/etc.
        protected override void LoadContent()
        {
            this.uiFont = Game.Content.Load<SpriteFont>("uiFont");

            // TODO A utility method, GetColoredTexture might be useful,
            //      which does this work here that's repeated quite a bit.
            this.headerBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.headerBackground.SetData<Color>(new Color[] { Color.Red });

            this.bodyBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.bodyBackground.SetData<Color>(new Color[] { Color.Brown });

            this.scrollbarBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.scrollbarBackground.SetData<Color>(new Color[] { Color.Gray });

            this.cursorTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.cursorTexture.SetData<Color>(new Color[] { Color.Yellow });

            this.levelBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.levelBackground.SetData<Color>(new Color[] { Color.Black });

            this.levelInlay = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.levelInlay.SetData<Color>(new Color[] { INLAY_DEFAULT_COLOR });

            this.levelBronzeInlay = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.levelBronzeInlay.SetData<Color>(new Color[] { Color.DarkRed });

            this.levelSilverInlay = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.levelSilverInlay.SetData<Color>(new Color[] { Color.Silver });

            this.levelGoldInlay = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.levelGoldInlay.SetData<Color>(new Color[] { Color.Gold });

            this.scrollbarButtonBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.scrollbarButtonBackground.SetData<Color>(new Color[] { Color.DarkGray });

            this.bronzeMedalBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.bronzeMedalBackground.SetData<Color>(new Color[] { Color.DarkRed });

            this.silverMedalBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.silverMedalBackground.SetData<Color>(new Color[] { Color.Silver });

            this.goldMedalBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            this.goldMedalBackground.SetData<Color>(new Color[] { Color.Gold });

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (((MainGame)Game).UIState == MainGame.InterfaceState.SelectingLevel)
            {
                Rectangle topButton = new Rectangle(GraphicsDevice.Viewport.Width - SCROLLBAR_WIDTH,
                                                    HEADER_HEIGHT,
                                                    SCROLLBAR_WIDTH,
                                                    SCROLLBAR_BUTTON_HEIGHT);

                Rectangle bottomButton = new Rectangle(GraphicsDevice.Viewport.Width - SCROLLBAR_WIDTH,
                                                        GraphicsDevice.Viewport.Height - SCROLLBAR_BUTTON_HEIGHT,
                                                        SCROLLBAR_WIDTH,
                                                        SCROLLBAR_BUTTON_HEIGHT);

                // TODO Add total counts of each medal received.
                // TODO Need to make a drawRectangle() utility function that I can use 
                //      to easily draw cursors. Takes in a rectangle and a color.
                Rectangle cursor = ((MainGame)Game).Utils.CursorBounds;

                this.previousMouseState = this.currentMouseState;
                this.currentMouseState = Mouse.GetState().LeftButton;


                if (topButton.Contains(cursor))
                {
                    if (buttonReleased && TopLevelIndex > 0)
                    {
                        TopLevelIndex--;
                    }
                }

                if (bottomButton.Contains(cursor))
                {
                    if (buttonReleased && TopLevelIndex < ((((MainGame)Game).Levels.Count / 3)-1))
                    {
                        TopLevelIndex++;
                    }
                }

                for (int i = 0; i < ((MainGame)Game).Levels.Count - TopLevelIndex * 3; i++)
                {
                    Vector2 basePosition = new Vector2((i % 3) * LevelGridWidth,
                                                       (i / 3) * LevelGridHeight);

                    Rectangle inlayRectangle = new Rectangle((int)basePosition.X + INLAY_INSET,
                                                             (int)basePosition.Y + INLAY_INSET + HEADER_HEIGHT,
                                                             LevelGridWidth - (2 * INLAY_INSET),
                                                             LevelGridHeight - (2 * INLAY_INSET));

                    if (inlayRectangle.Contains(cursor))
                    {
                        if (buttonReleased && TopLevelIndex < ((((MainGame)Game).Levels.Count / 3) - 1))
                        {
                            Console.WriteLine(((MainGame)Game).Levels[i + (TopLevelIndex * 3)].Walls.Count);
                            ((MainGame)Game).LoadLevel(((MainGame)Game).Levels[i + (TopLevelIndex * 3)]);
                            ((MainGame)Game).UIState = MainGame.InterfaceState.PlayingLevel;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (((MainGame)Game).UIState == MainGame.InterfaceState.SelectingLevel)
            {
                ((MainGame)Game).spriteBatch.DrawString(this.uiFont, "Selecting a level!" + ((MainGame)Game).Levels.Count, new Vector2(250, 250), Color.Orange);

                drawHeader(((MainGame)Game).spriteBatch);
                drawBody(((MainGame)Game).spriteBatch);
                drawScrollbar(((MainGame)Game).spriteBatch);
                drawCursor(((MainGame)Game).spriteBatch);
            }

            base.Draw(gameTime);
        }

        private void drawHeader(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(headerBackground, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, HEADER_HEIGHT), Color.White);
        }

        private void drawBody(SpriteBatch spriteBatch)
        {
            // Draw background.
            spriteBatch.Draw(bodyBackground,
                             new Rectangle(0,
                                           HEADER_HEIGHT,
                                           GraphicsDevice.Viewport.Width - SCROLLBAR_WIDTH,
                                           GraphicsDevice.Viewport.Height - HEADER_HEIGHT),
                             Color.White);

            // Figure out the Levels to display based on the top index.
            int firstLevelIndex = TopLevelIndex * 3;
            Level drawnLevel = ((MainGame)Game).Levels[firstLevelIndex];

            for (int i = firstLevelIndex; i < ((MainGame)Game).Levels.Count; i++)
            {
                DrawLevel(((MainGame)Game).Levels[i], i, new Vector2(i % 3, (i / 3) - TopLevelIndex), spriteBatch);
            }
        }

        private void drawScrollbar(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(scrollbarBackground, new Rectangle(GraphicsDevice.Viewport.Width - SCROLLBAR_WIDTH,
                                                                HEADER_HEIGHT,
                                                                SCROLLBAR_WIDTH,
                                                                GraphicsDevice.Viewport.Height - HEADER_HEIGHT),
                             Color.White);

            // Draw "Up arrow" button.
            spriteBatch.Draw(scrollbarButtonBackground, new Rectangle(GraphicsDevice.Viewport.Width - SCROLLBAR_WIDTH,
                                                                      HEADER_HEIGHT,
                                                                      SCROLLBAR_WIDTH,
                                                                      SCROLLBAR_BUTTON_HEIGHT),
                             Color.White);

            // Draw "Down arrow" button.
            spriteBatch.Draw(scrollbarButtonBackground, new Rectangle(GraphicsDevice.Viewport.Width - SCROLLBAR_WIDTH,
                                                                      GraphicsDevice.Viewport.Height - SCROLLBAR_BUTTON_HEIGHT,
                                                                      SCROLLBAR_WIDTH,
                                                                      SCROLLBAR_BUTTON_HEIGHT),
                             Color.White);
        }

        private void drawCursor(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(((MainGame)Game).Utils.CursorTexture,
                             ((MainGame)Game).Utils.CursorBounds,
                             Color.Yellow);

        }

        private void DrawLevel(Level level, int levelIndex, Vector2 gridPosition, SpriteBatch spriteBatch)
        {
            Vector2 basePosition = new Vector2(gridPosition.X * LevelGridWidth, gridPosition.Y * LevelGridHeight);

            Rectangle backgroundRectangle = new Rectangle((int)basePosition.X,
                                                          (int)basePosition.Y + HEADER_HEIGHT,
                                                          LevelGridWidth,
                                                          LevelGridHeight);
            Rectangle inlayRectangle = new Rectangle((int)basePosition.X + INLAY_INSET,
                                                     (int)basePosition.Y + INLAY_INSET + HEADER_HEIGHT,
                                                     LevelGridWidth - (2*INLAY_INSET),
                                                     LevelGridHeight - (2*INLAY_INSET));
            Vector2 stringPosition = new Vector2((int)basePosition.X + (LevelGridWidth / 2) - (uiFont.MeasureString(levelIndex.ToString()).X / 2),
                                                 (int)basePosition.Y + (LevelGridHeight / 2) + HEADER_HEIGHT - (uiFont.MeasureString(levelIndex.ToString()).Y / 2));

            Rectangle bronzeRectangle = new Rectangle((int)basePosition.X + INLAY_INSET,
                                                      (int)basePosition.Y + MedalOffset,
                                                      MedalWidth,
                                                      MedalHeight);

            Rectangle silverRectangle = new Rectangle((int)basePosition.X + (LevelGridWidth / 3),
                                                      (int)basePosition.Y + MedalOffset,
                                                      MedalWidth,
                                                      MedalHeight);

            Rectangle goldRectangle = new Rectangle((int)basePosition.X + (2 * (LevelGridWidth / 3)) - (INLAY_INSET/2),
                                                    (int)basePosition.Y + MedalOffset,
                                                    MedalWidth,
                                                    MedalHeight);

            int bronzeCompletionTime = level.CompletionTimes.Bronze / 1000;
            int silverCompletionTime = level.CompletionTimes.Silver / 1000;
            int goldCompletionTime = level.CompletionTimes.Gold / 1000;

            spriteBatch.Draw(levelBackground, backgroundRectangle, Color.White);

            // Set inlay color based on highest medal achieved.
            Texture2D drawnTexture = this.levelInlay;

            if (level.CompletionStatus.Gold)
            {
                drawnTexture = this.levelGoldInlay;
            }
            else if (level.CompletionStatus.Silver)
            {
                drawnTexture = this.levelSilverInlay;
            }
            else if (level.CompletionStatus.Bronze)
            {
                drawnTexture = this.levelBronzeInlay;
            }
            else
            {
                drawnTexture = this.levelInlay;
            }

            if (inlayRectangle.Contains(((MainGame)Game).Utils.CursorBounds))
            {
                MediaPlayer.Play(((MainGame)Game).Utils.MouseOverSound);
                spriteBatch.Draw(drawnTexture, inlayRectangle, Color.LightBlue);
            }
            else
            {
                spriteBatch.Draw(drawnTexture, inlayRectangle, Color.White);
            }

            spriteBatch.DrawString(this.uiFont, levelIndex.ToString(), stringPosition, Color.Black);

            // Draw medals
            spriteBatch.Draw(bronzeMedalBackground, bronzeRectangle, Color.White);
            spriteBatch.Draw(silverMedalBackground, silverRectangle, Color.White);
            spriteBatch.Draw(goldMedalBackground, goldRectangle, Color.White);

            // Draw medal times.
            spriteBatch.DrawString(uiFont,
                                   bronzeCompletionTime.ToString(),
                                   new Vector2((float)(bronzeRectangle.X + (bronzeRectangle.Width / 2) - (uiFont.MeasureString(bronzeCompletionTime.ToString()).X / 2)),
                                               (float)(bronzeRectangle.Y + (bronzeRectangle.Height / 2) - (uiFont.MeasureString(bronzeCompletionTime.ToString()).Y / 2))),
                                   Color.Black);

            spriteBatch.DrawString(uiFont,
                                   silverCompletionTime.ToString(),
                                   new Vector2((float)(silverRectangle.X + (silverRectangle.Width / 2) - (uiFont.MeasureString(silverCompletionTime.ToString()).X / 2)),
                                               (float)(silverRectangle.Y + (silverRectangle.Height / 2) - (uiFont.MeasureString(silverCompletionTime.ToString()).Y / 2))),
                                   Color.Black);


                        spriteBatch.DrawString(uiFont,
                                   goldCompletionTime.ToString(),
                                   new Vector2((float)(goldRectangle.X + (goldRectangle.Width / 2) - (uiFont.MeasureString(goldCompletionTime.ToString()).X / 2)),
                                               (float)(goldRectangle.Y + (goldRectangle.Height / 2) - (uiFont.MeasureString(goldCompletionTime.ToString()).Y / 2))),
                                   Color.Black);

        }
    }
}