// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo
{
    public interface IConnection
    {
        void Close();
        void Open();
        void Write(byte[] data);
        int Read(byte[] data);

        void Dispose();
    }
}
