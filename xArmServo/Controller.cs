// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Runtime.InteropServices;

namespace xArmServo
{
    public class Controller
    {
        // Constants
        private const int ProductId = 0x5750;
        private const int VendorId = 0x0483;
        private const int ReadWriteTimeout = 1000;
        private const int ReceiveTimeout = 2000;

        private const byte SIGNATURE = 0x55;

        // This needs to be static to keep the context otherwise, the app will close it
        private static UsbContext context = null;

        // Class variables
        private IUsbDevice _controller;
        private UsbEndpointReader _endpointReader;
        private UsbEndpointWriter _endpointWriter;

        /// <summary>
        /// Gets the first of default Controller.
        /// </summary>
        /// <returns>A Controller</returns>
        public static Controller GetFirstPortal()
        {
            var portals = GetControllers();

            if (portals.Length == 0)
            {
                throw new Exception("No Controller found.");
            }

            return new Controller(portals[0]);
        }

        /// <summary>
        /// Gets all the available USB device that matches the Controller.
        /// </summary>
        /// <returns>Array of USB devices.</returns>
        public static IUsbDevice[] GetControllers()
        {
            context ??= new UsbContext();
#if DEBUG
            context.SetDebugLevel(LogLevel.Info);
#else
            context.SetDebugLevel(LogLevel.Error);
#endif
            //Get a list of all connected devices
            var usbDeviceCollection = context.List();

            //Narrow down the device by vendor and pid
            var selectedDevice = usbDeviceCollection.Where(d => d.ProductId == ProductId && d.VendorId == VendorId);

            return selectedDevice.ToArray();
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Creates a new instance of a Controller.
        /// </summary>
        /// <param name="device">A valid Lego Dimensions instance.</param>
        /// <param name="id">An ID for this device, can be handy if you manage multiple ones.</param>
        public Controller(IUsbDevice device, int id = 0)
        {
            _controller = device;
            Id = id;
            // Open the device
            _controller.Open();

            // Non Windows OS need to detach the kernel driver
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Imports.SetAutoDetachKernelDriver(_controller.DeviceHandle, 1);
            }

            //Get the first config number of the interface
            _controller.ClaimInterface(_controller.Configs[0].Interfaces[0].Number);

            //Open up the endpoints
            _endpointWriter = _controller.OpenEndpointWriter(WriteEndpointID.Ep01);
            _endpointReader = _controller.OpenEndpointReader(ReadEndpointID.Ep01);

            // Read the first 64 bytes to clean the buffer
            var readBuffer = new byte[64];
            _endpointReader.Read(readBuffer, ReadWriteTimeout, out var bytesRead);
        }

        /// <summary>
        /// Stops a servo.
        /// </summary>
        /// <param name="servoId">The servo ID should be between 1 and 6.</param>
        public void StopServo(byte servoId)
        {
            var buffer = new byte[2];
            buffer[0] = 1;
            buffer[1] = servoId;
            Send(Command.ServoStop, buffer);
        }

        /// <summary>
        /// Sets the position of a specific servo.
        /// </summary>
        /// <param name="servoId">The servo ID should be between 1 and 6.</param>
        /// <param name="position">The position, depending on the servo, should be between 0 and 1000.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="wait">If true, the method will wait for the duration before returning.</param>
        public void SetPosition(byte servoId, ushort position, ushort duration = 1000, bool wait = false)
        {
            var buffer = new byte[6];
            int idx = 0;
            buffer[idx++] = 1;
            buffer[idx++] = (byte)(duration & 0xff);
            buffer[idx++] = (byte)(duration >> 8);
            buffer[idx++] = servoId;
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
        public void SetPositions(byte[] servoIds, ushort[] positions, ushort duration = 1000, bool wait = false)
        {
            var buffer = new byte[3 + servoIds.Length * 3];
            int idx = 0;
            buffer[idx++] = (byte)servoIds.Length;
            buffer[idx++] = (byte)(duration & 0xff);
            buffer[idx++] = (byte)(duration >> 8);
            for (int i = 0; i < servoIds.Length; i++)
            {
                buffer[idx++] = servoIds[i];
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
        public short GetPosition(byte servoId)
        {
            var buffer = new byte[2];
            buffer[0] = 1;
            buffer[1] = servoId;
            Send(Command.GetServoPosition, buffer);
            var readBuffer = new byte[64];
            _endpointReader.Read(readBuffer, ReadWriteTimeout, out var bytesRead);
            if ((bytesRead > 0) && (readBuffer[0] == SIGNATURE)
                && (readBuffer[1] == SIGNATURE) && (readBuffer[3] == (byte)Command.GetServoPosition)
                && (readBuffer[4] == 1) && (readBuffer[5] == servoId))
            {
                return (short)(readBuffer[7] * 256 + readBuffer[6]);
            }

            return -1;
        }

        /// <summary>
        /// Gets the position of multiple servos.
        /// </summary>
        /// <param name="servoIds">>The servo ID array, each servo should be between 1 and 6.</param>
        /// <returns>An array containing the positions of the servos.</returns>
        public short[] GetPositions(byte[] servoIds)
        {
            var buffer = new byte[servoIds.Length + 1];
            buffer[0] = (byte)servoIds.Length;
            servoIds.CopyTo(buffer, 1);
            Send(Command.GetServoPosition, buffer);
            var readBuffer = new byte[64];
            _endpointReader.Read(readBuffer, ReadWriteTimeout, out var bytesRead);
            short[] resp = new short[servoIds.Length];
            for (int i = 0; i < resp.Length; i++)
            {
                resp[i] = -1;
            }

            if ((bytesRead > 0) && (readBuffer[0] == SIGNATURE) && (readBuffer[1] == SIGNATURE) && (readBuffer[3] == (byte)Command.GetServoPosition))
            {
                for (int i = 0; i < servoIds.Length; i++)
                {
                    resp[i] = (short)(readBuffer[7 + i * 3] * 256 + readBuffer[6 + i * 3]);
                }
            }

            return resp;
        }

        /// <summary>
        /// Gets the battery level in volts.
        /// </summary>
        /// <returns>The battery voltage.</returns>
        public double GetBatteryLevel()
        {
            Send(Command.BatteryVoltage, new byte[0]);
            var readBuffer = new byte[64];
            _endpointReader.Read(readBuffer, ReadWriteTimeout, out var bytesRead);
            if ((bytesRead > 0) && (readBuffer[0] == SIGNATURE) && (readBuffer[1] == SIGNATURE) && (readBuffer[3] == (byte)Command.BatteryVoltage))
            {
                return (readBuffer[5] * 256 + readBuffer[4]) / 1000.0;
            }

            return -1;
        }

        private void Send(Command cmd, byte[] data)
        {
            //var buffer = new byte[data.Length + 4];
            var buffer = new byte[64];
            int idx = 0;
            buffer[idx++] = SIGNATURE;
            buffer[idx++] = SIGNATURE;
            buffer[idx++] = (byte)(data.Length + 2);
            buffer[idx++] = (byte)cmd;
            Array.Copy(data, 0, buffer, idx, data.Length);
            _endpointWriter.Write(buffer, ReadWriteTimeout, out var bytesWritten);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _controller.ReleaseInterface(_controller.Configs[0].Interfaces[0].Number);
            _controller.Close();
            _controller.Dispose();
        }
    }
}