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
    public class Utils
    {
        #region "Constants"
        /// <summary>
        /// The location where Levels are stored.
        /// </summary>
        internal const string LEVEL_PATH = "C:\\Users\\DANIEL!\\Documents\\Visual Studio 2013\\Projects\\MazeNavigator\\MazeNavigator\\Content\\Levels\\";

        /// <summary>
        /// The file extension for Levels.
        /// </summary>
        internal const string LEVEL_EXT = ".lvl";

        internal const string MEDAL_PROGRESS_PATH = "progress.txt";

        private const int CURSOR_WIDTH = 10;
        private const int CURSOR_HEIGHT = 10;
        #endregion

        public enum Medal
        {
            Bronze,
            Silver,
            Gold
        }
        
        /// <remarks>
        /// Each integer represents milliseconds.
        /// </remarks>
        public struct CompletionTimes
        {
            public int Bronze;
            public int Silver;
            public int Gold;
        }

        public Texture2D CursorTexture
        {
            get;
            private set;
        }

        public Rectangle CursorBounds
        {
            get
            {
                return new Rectangle(Mouse.GetState().Position.X,
                                     Mouse.GetState().Position.Y,
                                     CURSOR_WIDTH,
                                     CURSOR_HEIGHT);
            }
        }

        public Song MouseOverSound;

        public Utils(MainGame game)
        {
            this.CursorTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            this.CursorTexture.SetData<Color>(new Color[] { Color.White });

            this.MouseOverSound = game.Content.Load<Song>("Audio/MouseOver");
        }
    }
}
