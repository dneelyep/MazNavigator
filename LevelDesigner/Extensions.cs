using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelDesigner
{
    public static class Extensions
    {
        public static Rectangle CreateRectangle(Point startingPoint, Point endingPoint)
        {
            int rectWidth = Math.Abs(endingPoint.X - startingPoint.X);
            int rectHeight = Math.Abs(endingPoint.Y - startingPoint.Y);
            Point startPositionOffset = Point.Zero;

            // Quadrant I.
            if (endingPoint.X > startingPoint.X &&
                    endingPoint.Y < startingPoint.Y)
            {
                startPositionOffset.Y -= rectHeight;
            }
            // Quadrant II.
            if (endingPoint.X < startingPoint.X &&
                endingPoint.Y < startingPoint.Y)
            {
                startPositionOffset.X -= rectWidth;
                startPositionOffset.Y -= rectHeight;
            }
            // Quadrant III.
            if (endingPoint.X < startingPoint.X &&
                endingPoint.Y > startingPoint.Y)
            {
                startPositionOffset.X -= rectWidth;
            }
            // Quadrant IV.
            if (endingPoint.X > startingPoint.X &&
                endingPoint.Y > startingPoint.Y)
            {
                // No offset needed.
            }

            return new Rectangle(startingPoint.X + startPositionOffset.X,
                                 startingPoint.Y + startPositionOffset.Y,
                                 rectWidth,
                                 rectHeight);
        }
    }
}