// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo
{
    /// <summary>
    /// Represents a definition for a servo motor, including its minimum and maximum angles.
    /// </summary>
    public class ServoDefinition
    {
        /// <summary>
        /// Gets or sets the minimum angle of the servo motor.
        /// </summary>
        public int MinAngle { get; internal set; }

        /// <summary>
        /// Gets or sets the maximum angle of the servo motor.
        /// </summary>
        public int MaxAngle { get; internal set; }

        /// <summary>
        /// Gets or sets the servo motor associated with this definition.
        /// </summary>
        public Servo Servo { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ServoDefinition class with the specified servo, minimum angle, and maximum angle.
        /// </summary>
        /// <param name="servo">The servo motor associated with this definition.</param>
        /// <param name="minAngle">The minimum angle of the servo motor.</param>
        /// <param name="maxAngle">The maximum angle of the servo motor.</param>
        public ServoDefinition(Servo servo, int minAngle, int maxAngle)
        {
            Servo = servo;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }
    }

}
