using MazeNavigator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MazeNavigator.Content
{
    public class Level
        : DrawableGameComponent
    {
        /// <summary>
        /// The player's position when they start this Level.
        /// </summary>
        public StartPoint StartPoint { get; set; }

        /// <summary>
        /// Where the player has to reach to end this level.
        /// </summary>
        public EndPoint EndPoint { get; set; }

        public List<Rectangle> Walls { get; set; }

        /// <summary>
        /// How long it should take the player to complete this level
        /// for various medal types.
        /// </summary>
        internal Utils.CompletionTimes CompletionTimes;

        private Texture2D wallTexture;

        /// <summary>
        /// How many milliseconds the player has played this level.
        /// </summary>
        internal float TimePlayed = 0f;

        public string Name;

        internal ProgressManager.MedalCompletionStatus CompletionStatus;

        public Level(Game game) : base(game)
        {
        }

        public static Level Create(Game game, string levelName)
        {
            Level generatedLevel = new Level(game);

            generatedLevel.Walls = new List<Rectangle>();
            generatedLevel.Name = levelName;

            string[] lines = File.ReadAllLines(Utils.LEVEL_PATH + levelName + Utils.LEVEL_EXT);

            string completionTimesLine = lines[0];
            string[] completionTimesFields = completionTimesLine.Split(new char[] { '|' });
            generatedLevel.CompletionTimes.Bronze = int.Parse(completionTimesFields[0]);
            generatedLevel.CompletionTimes.Silver = int.Parse(completionTimesFields[1]);
            generatedLevel.CompletionTimes.Gold = int.Parse(completionTimesFields[2]);

            string startPointLine = lines[1];
            string[] startPointFields = startPointLine.Split(new char[] { '|' });
            generatedLevel.StartPoint = new StartPoint(game, new Vector2(int.Parse(startPointFields[0]),
                                                                         int.Parse(startPointFields[1])));
            generatedLevel.StartPoint.Initialize();

            string endPointLine = lines[2];
            string[] endPointFields = endPointLine.Split(new char[] { '|' });
            generatedLevel.EndPoint = new EndPoint(game, new Vector2(int.Parse(endPointFields[0]),
                                                                     int.Parse(endPointFields[1])));
            generatedLevel.EndPoint.Initialize();

            for (int line = 3; line < lines.Count(); line++)
            {
                string[] fields = lines[line].Split(new char[] { '|' });
                generatedLevel.Walls.Add(new Rectangle(int.Parse(fields[0]),
                                                       int.Parse(fields[1]),
                                                       int.Parse(fields[2]),
                                                       int.Parse(fields[3])));
            }

            // Load completion time status from progress file.
            if (!File.Exists(Utils.MEDAL_PROGRESS_PATH))
            {
                File.Create(Utils.MEDAL_PROGRESS_PATH);
            }
            else
            {
                string[] progressLines = File.ReadAllLines(Utils.MEDAL_PROGRESS_PATH);
                foreach (string level in progressLines)
                {
                    string[] fields = level.Split(new char[] { '|' });
                    if (fields[0] == generatedLevel.Name)
                    {
                        generatedLevel.CompletionStatus.Bronze = (fields[1] == "1");
                        generatedLevel.CompletionStatus.Silver = (fields[2] == "1");
                        generatedLevel.CompletionStatus.Gold = (fields[3] == "1");
                    }
                }
            }

            generatedLevel.TimePlayed = 0f;

            return generatedLevel;
        }

        protected override void LoadContent()
        {
            this.wallTexture = new Texture2D(GraphicsDevice, 1, 1);
            this.wallTexture.SetData<Color>(new Color[] { Color.Purple });

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.StartPoint != null)
            {
                this.StartPoint.Draw(gameTime);
            }

            if (this.EndPoint != null)
            {
                this.EndPoint.Draw(gameTime);
            }
            drawWalls();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePlayedTime(gameTime);
            base.Update(gameTime);
        }

        private void drawWalls()
        {
            if (this.Walls != null)
            {
                foreach (Rectangle wall in this.Walls)
                {
                    ((MainGame)Game).spriteBatch.Draw(wallTexture,
                                                      wall,
                                                      Color.White);
                }
            }
        }

        private void UpdatePlayedTime(GameTime gameTime)
        {
            // Update the TimePaused counter.
            if (((MainGame)Game).UIState == MainGame.InterfaceState.PlayingLevel)
            {
                this.TimePlayed += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
    }
}
