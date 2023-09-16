// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo.LewanSoul
{
    /// <summary>
    /// Represents different types of alarms that can occur.
    /// </summary>
    public enum Alarm
    {
        /// <summary>
        /// No alarm is triggered.
        /// </summary>
        NoAlarm = 0,

        /// <summary>
        /// Alarm triggered due to over-temperature condition.
        /// </summary>
        OverTemperature,

        /// <summary>
        /// Alarm triggered due to over-voltage condition.
        /// </summary>
        OverVoltage,

        /// <summary>
        /// Alarm triggered due to both over-temperature and over-voltage conditions.
        /// </summary>
        OverTemperatureAndOverVoltage,

        /// <summary>
        /// Alarm triggered due to locked rotor condition.
        /// </summary>
        LockedRotor,

        /// <summary>
        /// Alarm triggered due to both over-temperature and stalled conditions.
        /// </summary>
        OverTemperatureAndStalled,

        /// <summary>
        /// Alarm triggered due to both over-voltage and stalled conditions.
        /// </summary>
        OverVoltageAndStalled,
    }
}
