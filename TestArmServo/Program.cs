// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

using xArmServo;

var controller = UsbConnection.GetFirstPortal();
// var controller = SerialCommunication.Create("COM3");
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

Console.WriteLine("Moving servo 6 from 0 to 1000 very quickly");
controller.SetPosition(Servo.S6, 0, 300);
Thread.Sleep(500);
controller.SetPosition(Servo.S6, 1000, 300);

// Now create ServoMotor abstraction and test this
ServoDefinition servoDefinition1 = new ServoDefinition(Servo.S1, 0, 180);
ServoDefinition servoDefinition2 = new ServoDefinition(Servo.S2, -180, 180);
ServoDefinition servoDefinition3 = new ServoDefinition(Servo.S3, -180, 180);
ServoDefinition servoDefinition4 = new ServoDefinition(Servo.S4, -180, 180);
ServoDefinition servoDefinition5 = new ServoDefinition(Servo.S5, -180, 180);
ServoDefinition servoDefinition6 = new ServoDefinition(Servo.S6, -180, 180);
ServoMotor servo1 = new ServoMotor(servoDefinition1, controller);
ServoMotor servo2 = new ServoMotor(servoDefinition2, controller);
ServoMotor servo3 = new ServoMotor(servoDefinition3, controller);
ServoMotor servo4 = new ServoMotor(servoDefinition4, controller);
ServoMotor servo5 = new ServoMotor(servoDefinition5, controller);
ServoMotor servo6 = new ServoMotor(servoDefinition6, controller);

servo1.SetPosition(0);
Console.WriteLine($"Servo {servo1.ServoDefinition.Servo} position: {servo1.GetPosition()}");
servo1.SetPosition(180);
Console.WriteLine($"Servo {servo1.ServoDefinition.Servo} position: {servo1.GetPosition()}");
servo1.SetPosition(90);
Console.WriteLine($"Servo {servo1.ServoDefinition.Servo} position: {servo1.GetPosition()}");

servo2.SetPosition(-180);
Console.WriteLine($"Servo {servo2.ServoDefinition.Servo} position: {servo2.GetPosition()}");
servo2.SetPosition(180);
Console.WriteLine($"Servo {servo2.ServoDefinition.Servo} position: {servo2.GetPosition()}");
servo2.SetPosition(0);
Console.WriteLine($"Servo {servo2.ServoDefinition.Servo} position: {servo2.GetPosition()}");

servo3.SetPosition(-180);
Console.WriteLine($"Servo {servo3.ServoDefinition.Servo} position: {servo3.GetPosition()}");
servo3.SetPosition(180);
Console.WriteLine($"Servo {servo3.ServoDefinition.Servo} position: {servo3.GetPosition()}");
servo3.SetPosition(0);
Console.WriteLine($"Servo {servo3.ServoDefinition.Servo} position: {servo3.GetPosition()}");

servo4.SetPosition(-180);
Console.WriteLine($"Servo {servo4.ServoDefinition.Servo} position: {servo4.GetPosition()}");
servo4.SetPosition(180);
Console.WriteLine($"Servo {servo4.ServoDefinition.Servo} position: {servo4.GetPosition()}");
servo4.SetPosition(0);
Console.WriteLine($"Servo {servo4.ServoDefinition.Servo} position: {servo4.GetPosition()}");

servo5.SetPosition(-180);
Console.WriteLine($"Servo {servo5.ServoDefinition.Servo} position: {servo5.GetPosition()}");
servo5.SetPosition(180);
Console.WriteLine($"Servo {servo5.ServoDefinition.Servo} position: {servo5.GetPosition()}");
servo5.SetPosition(0);
Console.WriteLine($"Servo {servo5.ServoDefinition.Servo} position: {servo5.GetPosition()}");

servo6.SetPosition(-180);
Console.WriteLine($"Servo {servo6.ServoDefinition.Servo} position: {servo6.GetPosition()}");
servo6.SetPosition(180);
Console.WriteLine($"Servo {servo6.ServoDefinition.Servo} position: {servo6.GetPosition()}");
servo6.SetPosition(0);
Console.WriteLine($"Servo {servo6.ServoDefinition.Servo} position: {servo6.GetPosition()}");

controller.StopAll();

