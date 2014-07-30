#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
using MazeNavigator;
using XNAUtils.Input;
#endregion

namespace LevelDesigner
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LevelBuilder : Game
    {
        private const string LEVEL_PATH = "C:\\Users\\DANIEL!\\Documents\\Visual Studio 2013\\Projects\\MazeNavigator\\MazeNavigator\\Content\\Levels\\";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private bool mouseHeld = false;
        private Point mouseStartPosition = Point.Zero;
        private Point mouseCurrentPosition = Point.Zero;
        private Texture2D selectionTexture;
        private Texture2D cursorTexture;
        private Texture2D rectangleTexture;
        private Texture2D saveButtonTexture;
        private Texture2D transparentOverlayTexture;
        private Rectangle saveButtonRectangle;
        internal SpriteFont uiFont;
        private InterfaceState UIState;
        private EndPoint endPoint;
        private StartPoint startPoint;
        private KeyboardRecorder kbRecorder;
        private Utils.Medal currentMedalBeingRetrieved = Utils.Medal.Bronze;
        private Utils.CompletionTimes MedalTimes;

        private enum InterfaceState
        {
            CreatingLevel,
            GettingMedalTimes,
            SavingLayout
        }

        /// <summary>
        /// A list of each rectangle the user has drawn in the UI.
        /// </summary>
        private List<Rectangle> Rectangles = new List<Rectangle>();

        public LevelBuilder()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.UIState = InterfaceState.CreatingLevel;

            this.kbRecorder = new KeyboardRecorder(this);
            this.kbRecorder.Initialize();

            selectionTexture = new Texture2D(this.GraphicsDevice, 1, 1);
            selectionTexture.SetData<Color>(new Color[] { Color.Gray });

            cursorTexture = new Texture2D(GraphicsDevice, 1, 1);
            cursorTexture.SetData<Color>(new Color[] { Color.Yellow });

            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData<Color>(new Color[] { Color.Purple });

            saveButtonTexture = new Texture2D(GraphicsDevice, 1, 1);
            saveButtonTexture.SetData<Color>(new Color[] { Color.Blue });

            transparentOverlayTexture = new Texture2D(GraphicsDevice, 1, 1);
            transparentOverlayTexture.SetData<Color>(new Color[] { Color.Gray });

            saveButtonRectangle = new Rectangle(GraphicsDevice.Viewport.Width - 100,
                                                10,
                                                80,
                                                80);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.uiFont = Content.Load<SpriteFont>("uiFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.kbRecorder.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (this.UIState == InterfaceState.SavingLayout)
            {
                this.UIState = InterfaceState.CreatingLevel;
            }
            else if (this.UIState == InterfaceState.GettingMedalTimes)
            {
                Console.Out.WriteLine("Current medal being retrieved: " + currentMedalBeingRetrieved.ToString());
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    string recordedText = this.kbRecorder.GetRecordedText();
                    if (recordedText != String.Empty)
                    {
                        recordedText = recordedText.Substring(0, (recordedText.Length - "Enter".Length));

                        if (currentMedalBeingRetrieved == Utils.Medal.Bronze)
                        {
                            if (int.TryParse(recordedText, out this.MedalTimes.Bronze))
                            {
                                this.currentMedalBeingRetrieved = Utils.Medal.Silver;
                            }
                            else
                            {
                                throw new InvalidDataException();
                            }
                        }
                        else if (currentMedalBeingRetrieved == Utils.Medal.Silver)
                        {
                            if (int.TryParse(recordedText, out this.MedalTimes.Silver))
                            {
                                this.currentMedalBeingRetrieved = Utils.Medal.Gold;
                            }
                            else
                            {
                                throw new InvalidDataException();
                            }
                        }
                        else if (currentMedalBeingRetrieved == Utils.Medal.Gold)
                        {
                            if (!int.TryParse(recordedText, out this.MedalTimes.Gold))
                            {
                                throw new InvalidDataException();
                            }
                            // TODO Need to allow specifying an input delay to the keyboard
                            //      recorder, so that input's only recorded every so often..
                            saveLevelToFile();
                            this.UIState = InterfaceState.CreatingLevel;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }

                        this.kbRecorder.StopRecording();
                        this.kbRecorder.ClearRecordedText();
                        this.kbRecorder.StartRecording();
                    }
                }
            }
            else if (this.UIState == InterfaceState.CreatingLevel)
            {
                mouseCurrentPosition = Mouse.GetState().Position;

                if (Mouse.GetState().LeftButton == ButtonState.Pressed
                    && !mouseHeld)
                {
                    if (saveButtonRectangle.Contains(mouseCurrentPosition))
                    {
                        this.UIState = InterfaceState.GettingMedalTimes;

                        this.kbRecorder.StopRecording();
                        this.kbRecorder.ClearRecordedText();
                        this.kbRecorder.StartRecording();
                    }
                    else
                    {
                        mouseHeld = true;
                        mouseStartPosition = Mouse.GetState().Position;
                    }
                }

                if (Mouse.GetState().LeftButton != ButtonState.Pressed && mouseHeld)
                {
                    mouseHeld = false;
                    this.Rectangles.Add(Extensions.CreateRectangle(mouseStartPosition, mouseCurrentPosition));
                }

                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    // TODO Need to add a gradually growing and then shrinking effect for EndPoints. Would be cool.
                    // TODO An EndPoint should be associated with a level, not with a game as a whole.
                    this.endPoint = new EndPoint(this, new Vector2(mouseCurrentPosition.X, mouseCurrentPosition.Y));
                    this.endPoint.Initialize();
                }

                if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
                {
                    // TODO A StartPoint should be associated with a level, not with a game as a whole.
                    this.startPoint = new StartPoint(this, new Vector2(mouseCurrentPosition.X, mouseCurrentPosition.Y));
                    startPoint.Initialize();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (this.UIState == LevelBuilder.InterfaceState.CreatingLevel)
            {
                // TODO This is smelly - I'm handling input logic in the Draw function...
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    drawSelectionRectangle();
                }

                // TODO Make a method or a cleaner way of handling this on-mouseover effect.
                if (this.saveButtonRectangle.Contains(mouseCurrentPosition))
                {
                    this.saveButtonTexture.SetData<Color>(new Color[] { Color.LightBlue });
                }
                else
                {
                    this.saveButtonTexture.SetData<Color>(new Color[] { Color.Blue });
                }

                drawRectangles();
                drawStartPoint();
                drawEndPoint();
                drawCursor();
                drawSaveButton();
            }
            else if (this.UIState == LevelBuilder.InterfaceState.SavingLayout)
            {
                // Draw background.
                spriteBatch.Draw(transparentOverlayTexture,
                                 new Rectangle(10, 
                                               10,
                                               GraphicsDevice.Viewport.Width - 20,
                                               GraphicsDevice.Viewport.Height - 20),
                                 Color.White);

                spriteBatch.DrawString(uiFont,
                                       "Level saved!",
                                       new Vector2((GraphicsDevice.Viewport.Width / 2) - 200,
                                                   GraphicsDevice.Viewport.Height / 2),
                                       Color.Yellow);
            }
            else if (this.UIState == LevelBuilder.InterfaceState.GettingMedalTimes)
            {
                // Draw background.
                spriteBatch.Draw(transparentOverlayTexture,
                                 new Rectangle(10,
                                               10,
                                               GraphicsDevice.Viewport.Width - 20,
                                               GraphicsDevice.Viewport.Height - 20),
                                 Color.White);

                spriteBatch.DrawString(uiFont,
                                       "Medal time for " + this.currentMedalBeingRetrieved.ToString() + ": " +
                                           this.kbRecorder.GetRecordedText() + " ms",
                                       new Vector2((GraphicsDevice.Viewport.Width / 2) - 400,
                                                   GraphicsDevice.Viewport.Height / 2),
                                       Color.Yellow);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawSelectionRectangle()
        {
            Rectangle selectionRectangle = Extensions.CreateRectangle(mouseStartPosition, mouseCurrentPosition);
            spriteBatch.Draw(selectionTexture,
                             selectionRectangle,
                             Color.White);
        }

        private void drawCursor()
        {
            spriteBatch.Draw(cursorTexture,
                             new Rectangle(mouseCurrentPosition.X, mouseCurrentPosition.Y, 10, 10),
                             Color.White);
        }
        private void drawEndPoint()
        {
            if (this.endPoint != null)
            {
                spriteBatch.Draw(this.endPoint.PowerUpTexture,
                                 new Rectangle((int)this.endPoint.Position.X,
                                               (int)this.endPoint.Position.Y,
                                               EndPoint.WIDTH,
                                               EndPoint.HEIGHT),
                                Color.White);
            }
        }

        private void drawStartPoint()
        {
            if (this.startPoint != null)
            {
                spriteBatch.Draw(this.startPoint.PowerUpTexture,
                                 new Rectangle((int)this.startPoint.Position.X,
                                               (int)this.startPoint.Position.Y,
                                               StartPoint.WIDTH,
                                               StartPoint.HEIGHT),
                                 Color.White);

            }
        }

        // TODO It would be great if the KeyboardRecorder had a facility for
        //      discarding certain input. So I could say kbRecorder.RecordOnly(List<Keys> keys);
        //      And then provide public Numeric key types/etc.
        // TODO Ideally, I would get rid of the drawRectangles,
        //      drawStartPoint, and drawEndPoint calls, and replace them
        //      with a call to Level.Draw().
        private void drawRectangles()
        {
            foreach (Rectangle rect in this.Rectangles)
            {
                spriteBatch.Draw(rectangleTexture,
                                 rect,
                                 Color.White);
            }
        }

        private void drawSaveButton()
        {
            spriteBatch.Draw(saveButtonTexture,
                             saveButtonRectangle,
                             Color.White);

            spriteBatch.DrawString(uiFont,
                                   "Save",
                                   new Vector2(saveButtonRectangle.X + 15, saveButtonRectangle.Y + 15),
                                   Color.Red);
        }

        // TODO Need to add in reflection when the player goes outside the screen, so that they "bounce" off
        //      of the screen when they hit the edge.
        private void saveLevelToFile()
        {
            string outputFilePath = LEVEL_PATH +
                                    DateTime.Now.Hour + "." +
                                    DateTime.Now.Minute + "." +
                                    DateTime.Now.Second + "." +
                                    DateTime.Now.Millisecond + ".lvl";

            // Medal times in ms.
            File.AppendAllLines(outputFilePath, new string[] { this.MedalTimes.Bronze + "|" + this.MedalTimes.Silver + "|" + this.MedalTimes.Gold });

            // Start/end point coordinates.
            File.AppendAllLines(outputFilePath, new string[] { this.startPoint.Position.X + "|" + this.startPoint.Position.Y });
            File.AppendAllLines(outputFilePath, new string[] { this.endPoint.Position.X + "|" + this.endPoint.Position.Y });

            // Walls.
            foreach (Rectangle rect in this.Rectangles)
            {
                File.AppendAllLines(outputFilePath, new string[]{ rect.X + "|" + rect.Y + "|" + rect.Width + "|" + rect.Height });
            }
        }
    }
}