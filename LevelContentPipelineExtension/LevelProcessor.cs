using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = System.String;
using TOutput = ContentBuilder.Level;

namespace LevelContentPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentProcessor(DisplayName = "Level Processor")]
    public class LevelProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            List<Rectangle> walls = new List<Rectangle>();

            string[] lines = input.Split(new char[] { '\n' });

            foreach (string strLine in lines)
            {
                string[] fields = strLine.Split(new char[] { '|' });

                walls.Add(new Rectangle(int.Parse(fields[0]),
                                        int.Parse(fields[1]),
                                        int.Parse(fields[2]),
                                        int.Parse(fields[3])));
            }

            return new ContentBuilder.Level(walls);
        }
    }
}