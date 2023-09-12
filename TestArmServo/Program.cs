// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using xArmServo;

var controller = Controller.GetFirstPortal();
Console.WriteLine($"Controller found, and open");

var batt = controller.GetBatteryLevel();
Console.WriteLine($"Battery level: {batt}");

Console.WriteLine("Moving all servos to 400");
for (byte i = 1; i <= 6; i++)
{
    controller.SetPosition(i, 400);
    var pos = controller.GetPosition(i);
    Console.WriteLine($"Servo {i} position: {pos}");

}

Console.WriteLine("Moving all servos to 500");
controller.SetPositions(new byte[] { 1, 2, 3, 4, 5, 6 }, new ushort[] { 500, 500, 500, 500, 500, 500 });

var poss = controller.GetPositions(new byte[] { 1, 2, 3, 4, 5, 6 });
for (int i = 0; i < poss.Length; i++)
{
    Console.WriteLine($"Servo {i + 1} position: {poss[i]}");
}

for (byte i = 1; i <= 6; i++)
{
    controller.StopServo(i);
}

