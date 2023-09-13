// Licensed to Laurent Ellerbach and contributors under one or more agreements.
// Laurent Ellerbach and contributors license this file to you under the MIT license.

namespace xArmServo
{
    /// <summary>
    /// Defines the basic operations for handling a connection.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Opens the connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Writes the specified data to the connection.
        /// </summary>
        /// <param name="data">The data to be written as an array of bytes.</param>
        void Write(byte[] data);

        /// <summary>
        /// Reads data from the connection into the provided byte array.
        /// </summary>
        /// <param name="data">The byte array to store the read data.</param>
        /// <returns>The number of bytes read.</returns>
        int Read(byte[] data);

        /// <summary>
        /// Disposes of any resources associated with the connection.
        /// </summary>
        void Dispose();
    }
}
