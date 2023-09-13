using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xArmServo
{
    public class SerialCommunication : IConnection, IDisposable
    {
        private SerialPort _serialPort;

        /// <summary>
        /// Creates a new instance of the Controller class with the specified serial port.
        /// </summary>
        /// <param name="portName">A valid port name</param>
        /// <returns>A Controller.</returns>
        public static Controller Create(string portName)
        {
            var serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 1000;

            var connection = new SerialCommunication(serialPort);
            return new Controller(connection);
        }

        /// <summary>
        /// Initializes a new instance of the SerialCommunication class with the specified serial port.
        /// </summary>
        /// <param name="serialPort">The serial port</param>
        public SerialCommunication(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        /// <inheritdoc/>
        public void Close()
        {
            _serialPort.Close();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _serialPort.Dispose();
        }

        /// <inheritdoc/>
        public void Open()
        {
            _serialPort.Open();
        }

        /// <inheritdoc/>
        public int Read(byte[] data)
        {
            CancellationTokenSource cs = new CancellationTokenSource(_serialPort.ReadTimeout);
            while ((_serialPort.BytesToRead <= data.Length) && (!cs.Token.IsCancellationRequested))
            {
                cs.Token.WaitHandle.WaitOne(20);
            }

            return _serialPort.Read(data, 0, data.Length);
        }

        /// <inheritdoc/>
        public void Write(byte[] data)
        {
            _serialPort.Write(data, 0, data.Length);
        }
    }
}
