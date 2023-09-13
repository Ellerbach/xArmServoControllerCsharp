// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo
{
    /// <summary>
    /// Represents a servo motor controlled by a specified controller and defined with a specific servo definition.
    /// </summary>
    public class ServoMotor
    {
        private Controller _controller;

        /// <summary>
        /// Gets or sets the definition of the servo motor.
        /// </summary>
        public ServoDefinition ServoDefinition { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ServoMotor class with the specified servo definition and controller.
        /// </summary>
        /// <param name="servoDefinition">The definition of the servo motor.</param>
        /// <param name="controller">The controller that manages the servo motor.</param>
        public ServoMotor(ServoDefinition servoDefinition, Controller controller)
        {
            ServoDefinition = servoDefinition;
            _controller = controller;
        }

        /// <summary>
        /// Gets the current position of the servo motor.
        /// </summary>
        /// <returns>The current position in degrees or <see cref="int.MinValue"/> if an error occurred.</returns>
        public int GetPosition()
        {
            var raw = _controller.GetPosition(ServoDefinition.Servo);
            if (raw == -1)
            {
                return int.MinValue;
            }

            return MapRawToAngle(raw);
        }

        /// <summary>
        /// Sets the position of the servo motor.
        /// </summary>
        /// <param name="angle">The target position in degrees.</param>
        public void SetPosition(int angle)
        {
            var raw = MapAngleToRaw(angle);
            _controller.SetPosition(ServoDefinition.Servo, raw);
        }

        private ushort MapAngleToRaw(int angle)
        {
            // raw is 0 to 1000
            // angle is from ServoDefinition.MinAngle to ServoDefinition.MaxAngle
            // so we need to map angle to raw
            angle = Math.Clamp(angle, ServoDefinition.MinAngle, ServoDefinition.MaxAngle);
            ushort raw = (ushort)(((double)angle - ServoDefinition.MinAngle) / (ServoDefinition.MaxAngle - ServoDefinition.MinAngle) * 1000);
            return raw;
        }

        private int MapRawToAngle(int raw)
        {
            // raw is 0 to 1000
            // angle is from ServoDefinition.MinAngle to ServoDefinition.MaxAngle
            // so we need to map raw to angle
            raw = Math.Clamp(raw, 0, 1000);
            int angle = (int)((raw / 1000.0) * (ServoDefinition.MaxAngle - ServoDefinition.MinAngle) + ServoDefinition.MinAngle);
            return angle;
        }
    }

}
