// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Runtime.InteropServices;
using UnitsNet;

namespace xArmServo
{
    public class Controller
    {
        private const byte SIGNATURE = 0x55;

        private IConnection _controller;

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Creates a new instance of a Controller.
        /// </summary>
        /// <param name="device">A valid Lego Dimensions instance.</param>
        /// <param name="id">An ID for this device, can be handy if you manage multiple ones.</param>
        public Controller(IConnection device, int id = 0)
        {
            _controller = device;
            Id = id;
            // Open the device
            _controller.Open();
        }

        /// <summary>
        /// Stops a servo.
        /// </summary>
        /// <param name="servoId">The servo ID should be between 1 and 6.</param>
        public void Stop(Servo servoId)
        {
            var buffer = new byte[2];
            buffer[0] = 1;
            buffer[1] = (byte)servoId;
            Send(Command.ServoStop, buffer);
        }

        /// <summary>
        /// Stops multiple servos.
        /// </summary>
        /// <param name="servoIds">The servo ID array, each servo should be between 1 and 6.</param>
        public void Stop(Servo[] servoIds)
        {
            var buffer = new byte[servoIds.Length + 1];
            buffer[0] = (byte)servoIds.Length;
            servoIds.CopyTo(buffer, 1);
            Send(Command.ServoStop, buffer);
        }

        /// <summary>
        /// Stops all servos.
        /// </summary>
        public void StopAll()
        {
            Send(Command.ServoStop, new byte[] { 6, 1, 2, 3, 4, 5, 6 });
        }

        /// <summary>
        /// Sets the position of a specific servo.
        /// </summary>
        /// <param name="servoId">The servo ID should be between 1 and 6.</param>
        /// <param name="position">The position, depending on the servo, should be between 0 and 1000.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="wait">If true, the method will wait for the duration before returning.</param>
        public void SetPosition(Servo servoId, ushort position, ushort duration = 1000, bool wait = true)
        {
            var buffer = new byte[6];
            int idx = 0;
            buffer[idx++] = 1;
            buffer[idx++] = (byte)(duration & 0xff);
            buffer[idx++] = (byte)(duration >> 8);
            buffer[idx++] = (byte)servoId;
            buffer[idx++] = (byte)(position & 0xff);
            buffer[idx++] = (byte)(position >> 8);
            Send(Command.ServoMove, buffer);
            if (wait)
            {
                Thread.Sleep(duration);
            }
        }

        /// <summary>
        /// Sets the position of multiple servos.
        /// </summary>
        /// <param name="servoIds">The servo ID array, each servo should be between 1 and 6.</param>
        /// <param name="positions">The positions of the servos</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="wait">If true, the method will wait for the duration before returning./param>
        public void SetPositions(Servo[] servoIds, ushort[] positions, ushort duration = 1000, bool wait = true)
        {
            var buffer = new byte[3 + servoIds.Length * 3];
            int idx = 0;
            buffer[idx++] = (byte)servoIds.Length;
            buffer[idx++] = (byte)(duration & 0xff);
            buffer[idx++] = (byte)(duration >> 8);
            for (int i = 0; i < servoIds.Length; i++)
            {
                buffer[idx++] = (byte)servoIds[i];
                buffer[idx++] = (byte)(positions[i] & 0xff);
                buffer[idx++] = (byte)(positions[i] >> 8);
            }

            Send(Command.ServoMove, buffer);
            if (wait)
            {
                Thread.Sleep(duration);
            }
        }

        /// <summary>
        /// Gets the position of a specific servo.
        /// </summary>
        /// <param name="servoId">The servo ID should be between 1 and 6.</param>
        /// <returns>The position of the servo.</returns>
        public short GetPosition(Servo servoId)
        {
            var buffer = new byte[2];
            buffer[0] = 1;
            buffer[1] = (byte)servoId;
            Send(Command.GetServoPosition, buffer);

            var readBuffer = Receive(Command.GetServoPosition, 4);
            if (readBuffer.Length >= 4)
            {
                return (short)(readBuffer[3] * 256 + readBuffer[2]);
            }

            return -1;
        }

        /// <summary>
        /// Gets the position of multiple servos.
        /// </summary>
        /// <param name="servoIds">>The servo ID array, each servo should be between 1 and 6.</param>
        /// <returns>An array containing the positions of the servos.</returns>
        public short[] GetPositions(Servo[] servoIds)
        {
            var buffer = new byte[servoIds.Length + 1];
            buffer[0] = (byte)servoIds.Length;
            servoIds.CopyTo(buffer, 1);
            Send(Command.GetServoPosition, buffer);
            var readBuffer = Receive(Command.GetServoPosition, servoIds.Length * 3 + 1);

            short[] resp = new short[servoIds.Length];
            for (int i = 0; i < resp.Length; i++)
            {
                resp[i] = -1;
            }

            if (readBuffer.Length >= servoIds.Length * 3 + 1)
            {
                for (int i = 0; i < servoIds.Length; i++)
                {
                    resp[i] = (short)(readBuffer[3 + i * 3] * 256 + readBuffer[2 + i * 3]);
                }
            }

            return resp;
        }

        /// <summary>
        /// Gets the battery level in volts.
        /// </summary>
        /// <returns>The battery voltage.</returns>
        public ElectricPotential GetBatteryLevel()
        {
            Send(Command.BatteryVoltage, new byte[0]);
            var readBuffer = Receive(Command.BatteryVoltage, 2);
            if (readBuffer.Length >= 2)
            {
                return ElectricPotential.FromMillivolts(readBuffer[1] * 256 + readBuffer[0]);
            }

            return ElectricPotential.FromVolts(-1);
        }

        private void Send(Command cmd, byte[] data)
        {
            var buffer = new byte[data.Length + 4];
            int idx = 0;
            buffer[idx++] = SIGNATURE;
            buffer[idx++] = SIGNATURE;
            buffer[idx++] = (byte)(data.Length + 2);
            buffer[idx++] = (byte)cmd;
            Array.Copy(data, 0, buffer, idx, data.Length);
            _controller.Write(buffer);
        }

        private byte[] Receive(Command cmd, int size)
        {
            var readBuffer = new byte[size + 4];
            var bytesRead = _controller.Read(readBuffer);
            if ((bytesRead > 0) && (readBuffer[0] == SIGNATURE) && (readBuffer[1] == SIGNATURE) && (readBuffer[3] == (byte)cmd))
            {
                var resp = new byte[readBuffer[2] - 2];
                Array.Copy(readBuffer, 4, resp, 0, resp.Length);
                return resp;
            }

            return new byte[0];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}