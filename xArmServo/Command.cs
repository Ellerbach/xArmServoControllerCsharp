// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo
{
    internal enum Command : byte
    {
        ServoRaw = 0x00,
        ServoMove = 0x03,
        ActionGroupRun = 0x06,
        ActionGroupStop = 0x07,
        ActionGroupSpeed = 0x0b,
        BatteryVoltage = 0x0f,
        ServoStop = 0x14,
        GetServoPosition = 0x15,
    }
}
