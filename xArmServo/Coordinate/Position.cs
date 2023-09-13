// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using UnitsNet;

namespace xArmServo.Coordinate
{
    /// <summary>
    /// Represents a position in three-dimensional space with X, Y, and Z coordinates.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Gets or sets the X-coordinate of the position.
        /// </summary>
        public Length X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate of the position.
        /// </summary>
        public Length Y { get; set; }

        /// <summary>
        /// Gets or sets the Z-coordinate of the position.
        /// </summary>
        public Length Z { get; set; }
    }
}
