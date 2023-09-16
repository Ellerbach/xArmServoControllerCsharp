// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using UnitsNet;

namespace xArmServo.LewanSoul
{
    /// <summary>
    /// Represents a controller for servo devices for the LewanSoul servo motors.
    /// </summary>
    public class ServoController
    {
        /// <summary>
        /// Some commands allow to broadcast to all servo, use this specific ID for that.
        /// </summary>
        public const byte AllServo = 0xFE;

        private const byte SIGNATURE = 0x55;

        private IConnection _controller;

        public ServoController(IConnection controller)
        {
            _controller = controller;
        }

        /// <summary>
        /// Moves a specific servo to a position over a specified time.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="position">The target position (0 to 1000).</param>
        /// <param name="time">The time to reach the target position (0 to 30000).</param>
        public void Move(byte servoId, ushort position, ushort time)
        {
            if (position > 1000)
            {
                throw new ArgumentOutOfRangeException("Position should be between 0 and 1000");
            }

            if (time > 30000)
            {
                throw new ArgumentOutOfRangeException("Time should be between 0 and 30000");
            }

            var buffer = new byte[4];
            int idx = 0;
            buffer[idx++] = (byte)(position & 0xff);
            buffer[idx++] = (byte)(position >> 8);
            buffer[idx++] = (byte)(time & 0xff);
            buffer[idx++] = (byte)(time >> 8);
            var bytes = CreateMessage(ServoCommand.MOVE_TIME_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Moves all servos to a position over a specified time.
        /// </summary>
        /// <param name="position">The target position (0 to 1000).</param>
        /// <param name="time">The time to reach the target position (0 to 30000).</param>
        public void MoveAll(ushort position, ushort time) => Move(AllServo, position, time);

        /// <summary>
        /// Sets the move parameters for a specific servo. This method does not move the servo. Use StartMove to start the movement.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="position">The target position (0 to 1000).</param>
        /// <param name="time">The time to reach the target position (0 to 30000).</param>
        public void SetMove(byte servoId, ushort position, ushort time)
        {
            if (position > 1000)
            {
                throw new ArgumentOutOfRangeException("Position should be between 0 and 1000");
            }

            if (time > 30000)
            {
                throw new ArgumentOutOfRangeException("Time should be between 0 and 30000");
            }

            var buffer = new byte[4];
            int idx = 0;
            buffer[idx++] = (byte)(position & 0xff);
            buffer[idx++] = (byte)(position >> 8);
            buffer[idx++] = (byte)(time & 0xff);
            buffer[idx++] = (byte)(time >> 8);
            var bytes = CreateMessage(ServoCommand.MOVE_TIME_WAIT_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Sets the move parameters for all servos.
        /// </summary>
        /// <param name="position">The target position (0 to 1000).</param>
        /// <param name="time">The time to reach the target position (0 to 30000).</param>
        public void SetMoveAll(ushort position, ushort time) => SetMove(AllServo, position, time);

        /// <summary>
        /// Starts moving a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        public void StartMove(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.MOVE_START, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Stops moving a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        public void StopMove(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.MOVE_STOP, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Starts moving all servos.
        /// </summary>
        public void StartMoveAll() => StartMove(AllServo);

        /// <summary>
        /// Stops moving all servos.
        /// </summary>
        public void StopMoveAll() => StopMove(AllServo);

        /// <summary>
        /// Reads the position of a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>The position of the servo.</returns>
        public ushort ReadPosition(byte servoId)
        {
            var buffer = new byte[1];
            buffer[0] = servoId;
            var bytes = CreateMessage(ServoCommand.POS_READ, servoId, buffer);
            _controller.Write(bytes);
            // Same packet format but it's in the packet!
            byte[] buff = new byte[5 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.POS_READ, servoId, buff);
            return (ushort)(response[0] + (response[1] << 8));
        }

        /// <summary>
        /// Writes a new ID for a specific servo. Use carefully! You may lose track of your servos if you use this method.
        /// Only have 1 servo connected at a time when using this method.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        public void WriteServoId(byte servoId)
        {
            var buffer = new byte[1];
            buffer[0] = servoId;
            var bytes = CreateMessage(ServoCommand.ID_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Reads the ID of a specific servo. This will only work if you have 1 servo connected at a time.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>The ID of the servo.</returns>
        public byte ReadServoId()
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.ID_READ, AllServo, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.POS_READ, AllServo, buff);
            return response[5];
        }

        /// <summary>
        /// Writes an angle offset adjustment for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="offset">The angle offset.</param>
        public void WriteAngleOffset(byte servoId, sbyte offset)
        {
            var buffer = new byte[1];
            buffer[0] = (byte)(offset);
            var bytes = CreateMessage(ServoCommand.ANGLE_OFFSET_ADJUST, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Writes an angle offset adjustment and stores it in EEPROM for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="offset">The angle offset.</param>
        public void WriteAngleOffsetAndStore(byte servoId, sbyte offset)
        {
            var buffer = new byte[1];
            buffer[0] = (byte)(offset);
            var bytes = CreateMessage(ServoCommand.ANGLE_OFFSET_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        public sbyte ReadAngleOffset(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.ANGLE_OFFSET_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.ANGLE_OFFSET_READ, servoId, buff);
            return (sbyte)response[5];
        }

        /// <summary>
        /// Writes angle limits for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="minAngle">The minimum angle.</param>
        /// <param name="maxAngle">The maximum angle.</param>
        public void WriteAngleLimit(byte servoId, ushort minAngle, ushort maxAngle)
        {
            if (minAngle > maxAngle)
            {
                throw new ArgumentOutOfRangeException("Min angle should be less than max angle");
            }

            if (minAngle > 1000)
            {
                throw new ArgumentOutOfRangeException("Min angle should be less than 1000");
            }

            if (maxAngle > 1000)
            {
                throw new ArgumentOutOfRangeException("Max angle should be less than 1000");
            }

            var buffer = new byte[4];
            int idx = 0;
            buffer[idx++] = (byte)(minAngle & 0xff);
            buffer[idx++] = (byte)(minAngle >> 8);
            buffer[idx++] = (byte)(maxAngle & 0xff);
            buffer[idx++] = (byte)(maxAngle >> 8);
            var bytes = CreateMessage(ServoCommand.ANGLE_LIMIT_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Reads angle limits for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>An array containing the minimum and maximum angles.</returns>
        public ushort[] ReadAngleLimit(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.ANGLE_LIMIT_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[7 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.ANGLE_LIMIT_READ, servoId, buff);
            ushort[] minMax = new ushort[2];
            minMax[0] = (ushort)(response[0] + (response[1] << 8));
            minMax[1] = (ushort)(response[2] + (response[3] << 8));
            return minMax;
        }

        /// <summary>
        /// Writes voltage input limits for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="minVoltage">The minimum voltage limit. Voltage should be between 4500 and 12000 millivolt.</param>
        /// <param name="maxVoltage">The maximum voltage limit. Voltage should be between 4500 and 12000 millivolt.</param>
        public void WriteVInLimit(byte servoId, ElectricPotential minVoltage, ElectricPotential maxVoltage)
        {
            if (minVoltage > maxVoltage)
            {
                throw new ArgumentOutOfRangeException("Min voltage should be less than max voltage.");
            }

            // range 4500~12000mv
            if ((minVoltage.Millivolts > 12000) || (minVoltage.Millivolts < 4500))
            {
                throw new ArgumentOutOfRangeException("Min voltage should be between 4500 and 12000 millivolt.");
            }

            // range 4500~12000mv
            if ((maxVoltage.Millivolts > 12000) || (maxVoltage.Millivolts < 4500))
            {
                throw new ArgumentOutOfRangeException("Max voltage should be between 4500 and 12000millivolt.");
            }

            var buffer = new byte[4];
            int idx = 0;
            buffer[idx++] = (byte)((ushort)minVoltage.Millivolts & 0xff);
            buffer[idx++] = (byte)((ushort)minVoltage.Millivolts >> 8);
            buffer[idx++] = (byte)((ushort)maxVoltage.Millivolts & 0xff);
            buffer[idx++] = (byte)((ushort)maxVoltage.Millivolts >> 8);
            var bytes = CreateMessage(ServoCommand.VIN_LIMIT_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Reads voltage input limits for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>An array containing the minimum and maximum voltage limits.</returns>
        public ElectricPotential[] ReadVInLimit(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.VIN_LIMIT_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[7 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.VIN_LIMIT_READ, servoId, buff);
            ElectricPotential[] minMax = new ElectricPotential[2];
            minMax[0] = ElectricPotential.FromMillivolts((ushort)(response[0] + (response[1] << 8)));
            minMax[1] = ElectricPotential.FromMillivolts((ushort)(response[2] + (response[3] << 8)));
            return minMax;
        }

        /// <summary>
        /// Sets the maximum temperature limit for a servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="maxTemperature">The maximum temperature limit (50 to 100 Celsius).</param>
        public void SetMaximumTemperatureLimit(byte servoId, Temperature maxTemperature)
        {
            if ((maxTemperature.DegreesCelsius > 100) || (maxTemperature.DegreesCelsius < 50))
            {
                throw new ArgumentOutOfRangeException("Max temperature should be between 50 and 100 Celsius.");
            }

            var buffer = new byte[1];
            buffer[0] = (byte)maxTemperature.DegreesCelsius;
            var bytes = CreateMessage(ServoCommand.TEMP_MAX_LIMIT_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Gets the maximum temperature limit for a servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>The maximum temperature limit.</returns>
        public Temperature GetMaximumTemperatureLimit(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.TEMP_MAX_LIMIT_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.TEMP_MAX_LIMIT_READ, servoId, buff);
            return Temperature.FromDegreesCelsius(response[0]);
        }

        /// <summary>
        /// Gets the current temperature of a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>The current temperature.</returns>
        public Temperature GetTemperature(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.TEMP_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.TEMP_READ, servoId, buff);
            return Temperature.FromDegreesCelsius(response[0]);
        }

        /// <summary>
        /// Gets the input voltage of a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>The input voltage.</returns>
        public ElectricPotential GetVIn(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.VIN_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.VIN_READ, servoId, buff);
            return ElectricPotential.FromMillivolts((ushort)(response[0] + (response[1] << 8)));
        }

        /// <summary>
        /// Loads or unloads a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="load">True to load, false to unload.</param>
        public void LoadServo(byte servoId, bool load)
        {
            var buffer = new byte[1];
            buffer[0] = (byte)(load ? 1 : 0);
            var bytes = CreateMessage(ServoCommand.LOAD_OR_UNLOAD_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Checks if a specific servo is loaded.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>True if the servo is loaded, otherwise false.</returns>
        public bool IsLoadedServo(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.LOAD_OR_UNLOAD_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.LOAD_OR_UNLOAD_READ, servoId, buff);
            return response[0] == 1;
        }

        /// <summary>
        /// Sets the LED state for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="ledOn">True to turn the LED on, false to turn it off.</param>
        public void SetLed(byte servoId, bool ledOn)
        {
            var buffer = new byte[1];
            buffer[0] = (byte)(ledOn ? 1 : 0);
            var bytes = CreateMessage(ServoCommand.LED_CTRL_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Gets the current LED state for a specific servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>True if the LED is on, otherwise false.</returns>
        public bool GetLed(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.LED_CTRL_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.LED_CTRL_READ, servoId, buff);
            return response[0] == 1;
        }

        /// <summary>
        /// Sets the alarm state for a servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <param name="alarm">The alarm state to set.</param>
        public void SetAlarm(byte servoId, Alarm alarm)
        {
            var buffer = new byte[1];
            buffer[0] = (byte)alarm;
            var bytes = CreateMessage(ServoCommand.LED_ERROR_WRITE, servoId, buffer);
            _controller.Write(bytes);
        }

        /// <summary>
        /// Gets the current alarm state for a servo.
        /// </summary>
        /// <param name="servoId">The ID of the servo.</param>
        /// <returns>The current alarm state.</returns>
        public Alarm GetAlarm(byte servoId)
        {
            var buffer = new byte[0];
            var bytes = CreateMessage(ServoCommand.LED_ERROR_READ, servoId, buffer);
            _controller.Write(bytes);
            byte[] buff = new byte[4 + 3];
            _controller.Read(buff);
            var response = GetData(ServoCommand.LED_ERROR_READ, servoId, buff);
            return (Alarm)response[0];
        }

        private byte[] CreateMessage(ServoCommand cmd, byte servoId, byte[] data)
        {
            // Header ID number Data Length Command Parameter Checksum
            // 0x55 0x55 ID Length Cmd Prm 1... Prm N Checksum
            // ID: ID number, 1 byte, value range: 0-253
            // Length: Length of the data package, 1 byte, value range: 3-255
            // Cmd: Command, 1 byte, value range: 0-255
            // Prm 1...Prm N: Parameters, value range: 0-255
            // Checksum: Checksum, 1 byte, value range: 0-255
            // Checksum=~(ID+ Length+Cmd+ Prm1+...PrmN)If the numbers in the
            // brackets are calculated and exceeded 255,Then take the lowest one byte, "~" means Negation
            byte[] bytes = new byte[data.Length + 6];
            int idx = 0;
            bytes[idx++] = SIGNATURE;
            bytes[idx++] = SIGNATURE;
            bytes[idx++] = servoId;
            bytes[idx++] = (byte)(data.Length + 3);
            bytes[idx++] = (byte)cmd;
            data.CopyTo(bytes, idx);
            idx += data.Length;
            byte checksum = 0;
            for (int i = 2; i < bytes.Length - 1; i++)
            {
                checksum += bytes[i];
            }
            bytes[idx++] = (byte)(~checksum);

            return bytes;
        }

        private byte[] GetData(ServoCommand cmd, byte servoId, byte[] data)
        {
            // Header ID number Data Length Command Parameter Checksum
            // 0x55 0x55 ID Length Cmd Prm 1... Prm N Checksum
            // ID: ID number, 1 byte, value range: 0-253
            // Length: Length of the data package, 1 byte, value range: 3-255
            // Cmd: Command, 1 byte, value range: 0-255
            // Prm 1...Prm N: Parameters, value range: 0-255
            // Checksum: Checksum, 1 byte, value range: 0-255
            // Checksum=~(ID+ Length+Cmd+ Prm1+...PrmN)If the numbers in the
            // brackets are calculated and exceeded 255,Then take the lowest one byte, "~" means Negation
            if (data[0] != SIGNATURE || data[1] != SIGNATURE)
            {
                throw new Exception("Invalid data");
            }

            // In the case of ID_READ, there is no servo ID to check, it was a broadcast one
            if ((cmd != ServoCommand.ID_READ) && (data[2] != servoId))
            {
                throw new Exception("Invalid servo id");
            }

            if (data[3] != data.Length + 3)
            {
                throw new Exception("Invalid data length");
            }

            if (data[4] != (byte)cmd)
            {
                throw new Exception("Invalid command");
            }

            byte checksum = 0;
            for (int i = 2; i < data.Length - 1; i++)
            {
                checksum += data[i];
            }

            if (data[data.Length - 1] != (byte)(~checksum))
            {
                throw new Exception("Invalid checksum");
            }

            byte[] resp = new byte[data.Length - 6];
            Array.Copy(data, 5, resp, 0, resp.Length);
            return resp;
        }
    }
}
