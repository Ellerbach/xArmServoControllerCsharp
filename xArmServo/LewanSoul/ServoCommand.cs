// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo.LewanSoul
{
    internal enum ServoCommand
    {
        MOVE_TIME_WRITE = 1,
        MOVE_TIME_READ = 2,
        MOVE_TIME_WAIT_WRITE = 7,
        MOVE_TIME_WAIT_READ = 8,
        MOVE_START = 11,
        MOVE_STOP = 12,
        ID_WRITE = 13,
        ID_READ = 14,
        ANGLE_OFFSET_ADJUST = 17,
        ANGLE_OFFSET_WRITE = 18,
        ANGLE_OFFSET_READ = 19,
        ANGLE_LIMIT_WRITE = 20,
        ANGLE_LIMIT_READ = 21,
        VIN_LIMIT_WRITE = 22,
        VIN_LIMIT_READ = 23,
        TEMP_MAX_LIMIT_WRITE = 24,
        TEMP_MAX_LIMIT_READ = 25,
        TEMP_READ = 26,
        VIN_READ = 27,
        POS_READ = 28,
        OR_MOTOR_MODE_WRITE = 29,
        OR_MOTOR_MODE_READ = 30,
        LOAD_OR_UNLOAD_WRITE = 31,
        LOAD_OR_UNLOAD_READ = 32,
        LED_CTRL_WRITE = 33,
        LED_CTRL_READ = 34,
        LED_ERROR_WRITE = 35,
        LED_ERROR_READ = 36,
    }
}
