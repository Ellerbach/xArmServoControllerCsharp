// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using UnitsNet;

namespace xArmServo.Coordinate
{
    /// <summary>
    /// Represents a model of an arm with specified lengths for its segments.
    /// </summary>
    public class ArmModel
    {
        /// <summary>
        /// Gets or sets the length of the first segment (L1). This is the segment from the clamp to the first servo.
        /// </summary>
        public Length L1 { get; internal set; }

        /// <summary>
        /// Gets or sets the length of the second segment (L2). This is the segment from the first servo to the second servo.
        /// </summary>
        public Length L2 { get; internal set; }

        /// <summary>
        /// Gets or sets the length of the third segment (L3). This is the segment from the second servo to the third servo.
        /// </summary>
        public Length L3 { get; internal set; }

        /// <summary>
        /// Gets or sets the length of the fourth segment (L4). This is the segment from the third servo to the base.
        /// </summary>
        public Length L4 { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ArmModel class with specified segment lengths.
        /// </summary>
        /// <param name="l1">The length of the first segment (L1).</param>
        /// <param name="l2">The length of the second segment (L2).</param>
        /// <param name="l3">The length of the third segment (L3).</param>
        /// <param name="l4">The length of the fourth segment (L4).</param>
        public ArmModel(Length l1, Length l2, Length l3, Length l4)
        {
            L1 = l1;
            L2 = l2;
            L3 = l3;
            L4 = l4;
        }
    }
}
