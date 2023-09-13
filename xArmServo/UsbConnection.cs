// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Runtime.InteropServices;

namespace xArmServo
{
    /// <summary>
    /// Defines the basic operations for handling a USB connection.
    /// </summary>
    public class UsbConnection : IConnection, IDisposable
    {

        // Constants
        private const int ProductId = 0x5750;
        private const int VendorId = 0x0483;
        private const int ReadWriteTimeout = 1000;
        private const int ReceiveTimeout = 2000;
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

            return new Controller(new UsbConnection(portals[0]));
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
        /// Initializes a new instance of the UsbConnection class.
        /// </summary>
        /// <param name="device">The USB device used for the connection.</param>
        public UsbConnection(IUsbDevice device)
        {
            _controller = device;
        }

        /// <inheritdoc/>
        public void Close()
        {
            _controller.Close();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _controller.ReleaseInterface(_controller.Configs[0].Interfaces[0].Number);
            _controller.Close();
            _controller.Dispose();
        }

        /// <inheritdoc/>
        public void Open()
        {
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

        /// <inheritdoc/>
        public int Read(byte[] data)
        {
            var readBuffer = new byte[64];
            _endpointReader.Read(readBuffer, ReadWriteTimeout, out var bytesRead);
            if (bytesRead > 0)
            {
                Array.Copy(readBuffer, data, bytesRead <= data.Length ? bytesRead : data.Length);
            }

            return bytesRead;
        }

        /// <inheritdoc/>
        public void Write(byte[] data)
        {
            if (data.Length > 64)
            {
                throw new Exception("Data length cannot be more than 64 bytes");
            }

            var buffer = new byte[64];
            Array.Copy(data, buffer, data.Length);
            _endpointWriter.Write(buffer, ReadWriteTimeout, out var bytesWritten);
        }
    }
}
