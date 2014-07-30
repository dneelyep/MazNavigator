using MazeNavigator.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeNavigator
{
    /// <summary>
    /// Keeps track of, and allows updating, various types of progress
    /// the player can make within the game.
    /// </summary>
    internal class ProgressManager
    {
        internal struct MedalCompletionStatus
        {
            internal bool Bronze;
            internal bool Silver;
            internal bool Gold;
        }

        /// <summary>
        /// Checks which, if any, medals the player won,
        /// given that they completed the Level at the TimePlayed time stored in the Level.
        /// </summary>
        internal static void CheckMedalCompletionTimes(Level level)
        {
            MedalCompletionStatus status;

            // If the player had the medal before, don't take it from them.
            // Get [Got medal just now OR had medal before].
            status.Bronze = (level.CompletionStatus.Bronze ||
                             level.TimePlayed <= level.CompletionTimes.Bronze);
            status.Silver = (level.CompletionStatus.Silver ||
                             level.TimePlayed <= level.CompletionTimes.Silver);
            status.Gold   = (level.CompletionStatus.Gold ||
                             level.TimePlayed <= level.CompletionTimes.Gold);

            level.CompletionStatus = status;
        }

        internal static void SaveMedalCompletionTimes(List<Level> levels)
        {
            string fileContent = String.Empty;

            // Convert each Level's completion status into a line in the "progress" file, so
            // that the progress will be saved future games.
            foreach (Level level in levels)
            {
                fileContent += (level.Name + "|" +
                               (level.CompletionStatus.Bronze ? 1 : 0) + "|" +
                               (level.CompletionStatus.Silver ? 1 : 0) + "|" +
                               (level.CompletionStatus.Gold ? 1 : 0) +
                               Environment.NewLine);
            }

//            StorageDevice.ShowStorageDeviceGuide();
            File.WriteAllText("progress.txt", fileContent);
            // TODO Restore all this stuff.
// TODO Use StorageDevice device?
            // Open a storage container.
            //IAsyncResult result; =
            //    device.BeginOpenContainer("StorageDemo", null, null);

            //// Wait for the WaitHandle to become signaled.
            //result.AsyncWaitHandle.WaitOne();

            //StorageContainer container = device.EndOpenContainer(result);

            //// Close the wait handle.
            //result.AsyncWaitHandle.Close();

            //string filename = "currentprogress.sav";

            //// Check to see if the save file exists.
            //if (container.FileExists(filename))
            //{
            //    container.DeleteFile(filename);
            //}
        }
    }
}