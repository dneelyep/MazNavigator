#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MazeNavigator.PowerUps;
using MazeNavigator.Content;
using XNAUtils.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
#endregion

namespace MazeNavigator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
#region "Constants, enums"
        internal enum InterfaceState
        {
            SelectingLevel,
            PlayingLevel,
            LevelComplete,
            Paused
        }
#endregion

#region "Variables, properties"
        private GraphicsDeviceManager graphics;
        internal SpriteBatch spriteBatch;

        internal Player player;

        /// <summary>
        /// The Level that is currently being played.
        /// </summary>
        internal Level CurrentLevel;

        /// <summary>
        /// A list of all Levels in this game.
        /// </summary>
        internal List<Level> Levels;

        /// <summary>
        /// A list of each PowerUp that is currently active (visible, not deleted) in the game.
        /// </summary>
        internal List<PowerUps.PowerUp> PowerUps;

        /// <summary>
        /// The UI used to display data to the player.
        /// </summary>
        private UserInterface UI;

        /// <summary>
        /// Which screen/mode is currently being used.
        /// </summary>
        internal InterfaceState UIState;

        private LevelSelectScreen levelSelector;

        private PauseScreen pauseScreen;

        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;

        internal Utils Utils;

        private LevelCompleteScreen levelCompleteScreen;

        private Song PlayerDeathEffect;
#endregion

        public MainGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.player = new Player(this);
            this.Components.Add(this.player);

            this.UI = new UserInterface(this);
            this.Components.Add(this.UI);

            this.PowerUps = new List<PowerUps.PowerUp>();

            this.levelSelector = new LevelSelectScreen(this);
            this.Components.Add(this.levelSelector);

            this.levelCompleteScreen = new LevelCompleteScreen(this);
            this.Components.Add(this.levelCompleteScreen);

            this.pauseScreen = new PauseScreen(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Levels = new List<Level>();
            foreach (string levelName in Directory.GetFiles(Utils.LEVEL_PATH))
            {
                Level level = Level.Create(this, Path.GetFileNameWithoutExtension(levelName));
                level.Initialize();
                this.Levels.Add(level);
            }

            // TODO Wishlist: Let the player bounce around the menu as their character to select levels, etc. 
            this.UIState = InterfaceState.SelectingLevel;
            LoadLevel(this.Levels[0]);

            this.pauseScreen.Initialize();
            this.currentKeyboardState = Keyboard.GetState();

            this.Utils = new Utils(this);

            base.Initialize();
        }

        // TODO Add a countdown timer to the game.
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            try
            {
                this.PlayerDeathEffect = Content.Load<Song>("Audio/PlayerDeath");
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
            GenerateEntities(gameTime);
            HandleInput(gameTime);
            this.CurrentLevel.Update(gameTime);
            //Console.Out.WriteLine(this.kbRecorder.GetRecordedText());
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();

            if (this.UIState == InterfaceState.SelectingLevel)
            {
                // TODO Replace this with this.LevelSelector.draw().
            }
            else if (this.UIState == InterfaceState.PlayingLevel)
            {
                foreach (PowerUp powerUp in this.PowerUps)
                {
                    powerUp.Draw(gameTime);
                }

                // TODO This should probably be in Update().
                foreach (Rectangle wall in this.CurrentLevel.Walls)
                {
                    if (wall.Contains(this.player.Position))
                    {
                        KillPlayer();
                    }
                }

                this.CurrentLevel.Draw(gameTime);
            }
            else if (this.UIState == InterfaceState.Paused)
            {
                this.pauseScreen.Draw(gameTime);
            }

            base.Draw(gameTime);

            this.spriteBatch.End();
        }

        /// <summary>
        /// Examine key-presses, button presses, etc, and take action accordingly.
        /// </summary>
        private void HandleInput(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (UIState == InterfaceState.PlayingLevel)
            {
                // Pause the game.
                if (!previousKeyboardState.IsKeyDown(Keys.Escape)
                    && currentKeyboardState.IsKeyDown(Keys.Escape))
                {
                    Pause();
                }

                // Move left.
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    this.player.Acceleration.X -= Constants.NUDGE_FACTOR;
                }
                // Move down.
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    this.player.Acceleration.Y += Constants.NUDGE_FACTOR;
                }
                // Move right.
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    this.player.Acceleration.X += Constants.NUDGE_FACTOR;
                }
                // Move up.
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    this.player.Acceleration.Y -= Constants.NUDGE_FACTOR;
                }
            }
            else if (UIState == InterfaceState.Paused)
            {
                this.pauseScreen.Update(gameTime);

                if (!previousKeyboardState.IsKeyDown(Keys.Escape)
                    && currentKeyboardState.IsKeyDown(Keys.Escape))
                {
                    this.UIState = InterfaceState.PlayingLevel;
                }
            }
        }
 
        /// <summary>
        /// Create powerups, etc on the board.
        /// </summary>
        private void GenerateEntities(GameTime gameTime)
        {
            // Generate a new power up every second.
            if (gameTime.TotalGameTime.TotalMilliseconds % 5000 >= 0
                && gameTime.TotalGameTime.TotalMilliseconds % 5000 <= 30)
            {
                Random rng = new Random();
                PowerUps.SpeedUp speedUp = new PowerUps.SpeedUp(this);
                speedUp.Position = new Vector2(rng.Next(0, GraphicsDevice.Viewport.Width),
                                               rng.Next(0, GraphicsDevice.Viewport.Height));
                speedUp.Initialize();
                this.PowerUps.Add(speedUp);
            }
        }

        // TODO Ideally, this would be an Event...
        /// <summary>
        /// Actions to take when the player has come into contact with the
        /// end of the current Level.
        /// </summary>
        internal void CurrentLevelEndReached()
        {
            // Check for any medals the player won.
            // TODO Restore this.
            ProgressManager.CheckMedalCompletionTimes(CurrentLevel);
            ProgressManager.SaveMedalCompletionTimes(this.Levels);

            this.UIState = InterfaceState.LevelComplete;
        }

        internal void LoadNextLevel()
        {
            // Move to the next level.
            if (this.Levels.IndexOf(this.CurrentLevel) + 1 >= this.Levels.Count)
            {
                this.UIState = InterfaceState.SelectingLevel;
            }
            else
            {
                LoadLevel(this.Levels[this.Levels.IndexOf(this.CurrentLevel) + 1]);
            }
        }

        internal void ReloadCurrentLevel()
        {
            LoadLevel(this.CurrentLevel);
        }

        internal void LoadLevel(Level level)
        {
            this.CurrentLevel = level;
            this.CurrentLevel.TimePlayed = 0f;
            this.player.Acceleration = Vector2.Zero;
            this.player.Velocity = Vector2.Zero;
            this.player.Position = this.CurrentLevel.StartPoint.Position;
        }

        private void KillPlayer()
        {
            LoadLevel(this.CurrentLevel);
            MediaPlayer.Play(this.PlayerDeathEffect);
        }

        private void Pause()
        {
            this.UIState = InterfaceState.Paused;
        }

        internal void Quit()
        {
            Exit();
        }
    }
}