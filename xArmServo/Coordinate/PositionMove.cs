// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo.Coordinate
{
    public class PositionMove
    {
        public ArmModel ArmModel { get; internal set; }

        public Controller Controller { get; internal set; }

        public PositionMove(ArmModel armModel, Controller controller)
        {
            ArmModel = armModel;
            Controller = controller;
        }

        public void MoveTo(Position position)
        {
            // TODO
            //Controller.SetPositions(new Servo[] { Servo.S3, Servo.S4, Servo.S5, Servo.S6 }, new ushort[] { (ushort)(1000 - alphaRaw), alphaRaw, alphaRaw, tetaRaw });
        }

        private ushort MapAngleToRaw(double angle)
        {
            // raw is 100 to 900 mapping angle from -90 to +90
            // so we need to map angle to raw
            angle = Math.Clamp(angle, -Math.PI / 2, Math.PI / 2);
            var raw = (ushort)(((double)angle + Math.PI / 2) / (2 * Math.PI) * 800 + 100);
            return raw;
        }
    }
}
