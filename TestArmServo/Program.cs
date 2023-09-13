// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using xArmServo;

var controller = UsbConnection.GetFirstPortal();
Console.WriteLine($"Controller found, and open");

var batt = controller.GetBatteryLevel();
Console.WriteLine($"Battery level: {batt.Volts} V");

Console.WriteLine("Moving all servos to 400");
for (byte i = 1; i <= 6; i++)
{
    controller.SetPosition((Servo)i, 400);
    var pos = controller.GetPosition((Servo)i);
    Console.WriteLine($"Servo {i} position: {pos}");

}

Console.WriteLine("Moving all servos to 500");
controller.SetPositions(new Servo[] { Servo.S1, Servo.S2, Servo.S3, Servo.S4, Servo.S5, Servo.S6 }, new ushort[] { 500, 500, 500, 500, 500, 500 });

var poss = controller.GetPositions(new Servo[] { Servo.S1, Servo.S2, Servo.S3, Servo.S4, Servo.S5, Servo.S6 });
for (int i = 0; i < poss.Length; i++)
{
    Console.WriteLine($"Servo {i + 1} position: {poss[i]}");
}

//for (byte i = 1; i <= 6; i++)
//{
//    controller.Stop((Servo)i);
//}

Console.WriteLine("Moving servo 6 from 0 to 1000 very quickly");
controller.SetPosition(Servo.S6, 0, 300);
Thread.Sleep(500);
controller.SetPosition(Servo.S6, 1000, 300);

controller.StopAll();

