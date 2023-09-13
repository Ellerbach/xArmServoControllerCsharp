// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using System.Diagnostics;
using UnitsNet;

namespace xArmServo.Coordinate
{
    public class PositionMove
    {
        private Length[] Pz = new Length[128];

        public ArmModel ArmModel { get; internal set; }

        public Controller Controller { get; internal set; }

        public PositionMove(ArmModel armModel, Controller controller)
        {
            ArmModel = armModel;
            Controller = controller;

            double alpha;
            for (int i = 0; i < 128; i++)
            {
                alpha = i / 128.0 * Math.PI / 2.0;
                Pz[i] = ArmModel.L4 + ArmModel.L3 * Math.Sin(alpha) - ArmModel.L2 * Math.Cos(2 * alpha) - ArmModel.L1 * Math.Sin(3 * alpha);
                Debug.WriteLine($"Pz[{i}]={Pz[i]}");
            }
        }

        public void MoveTo(Position position)
        {
            // we need to find the closest Pz with the position.Z value
            int index = 0;
            for (int i = 0; i < 128; i++)
            {
                index = i;
                if (position.Z <= Pz[i])
                {
                    break;
                }
            }

            Console.WriteLine($"Closest index is {index} with Pz={Pz[index]}, real Pz={position.Z}");
            var alpha = index / 128.0 * Math.PI / 2.0;
            double teta;
            if (Math.Abs(position.X.Value) > Math.Abs(position.Y.Value))
            {
                teta = Math.Atan(position.Y / position.X);
            }
            else
            {
                teta = Math.PI / 2 - Math.Atan(position.X / position.Y);
            }

            var alphaRaw = MapAngleToRaw(alpha);
            if (double.IsNaN(teta))
            {
                Controller.SetPositions(new Servo[] { Servo.S3, Servo.S4, Servo.S5 }, new ushort[] { alphaRaw, (ushort)(1000 - alphaRaw), alphaRaw });
            }
            else
            {
                // FIXME! This is not correct, we need to find the right teta
                ushort tetaRaw = (ushort)(teta / (Math.PI / 2) * 1000);
                Controller.SetPositions(new Servo[] { Servo.S3, Servo.S4, Servo.S5, Servo.S6 }, new ushort[] { alphaRaw, (ushort)(1000 - alphaRaw), alphaRaw, tetaRaw });
            }
        }

        private ushort MapAngleToRaw(double angle)
        {
            // so we need to map angle to raw
            angle = Math.Clamp(angle, 0, Math.PI / 2);
            // when angle is 0, raw is 500
            // When angle is PI / 2, raw is 900
            ushort raw = (ushort)(900 - angle / (Math.PI / 2) * 400);
            return raw;
        }
    }
}
