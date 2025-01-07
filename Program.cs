using ConsoleApp1;
using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace WMBUSProtocol
{
    public class Program
    {
        private const int MaxBufferSize = 1024; // Maximum allowed buffer size to prevent unbounded growth
        

        public static void Main()
        {
            string comPort = "COM6"; // Replace with your COM port
            int baudRate = 9600;

            using (SerialPort serialPort = new SerialPort(comPort, baudRate))
            {
                try
                {
                    serialPort.ReadTimeout = 1000; // 1-second timeout
                    serialPort.Open();
                    Console.WriteLine($"Connected to {comPort} at {baudRate} baud.");

                    // Buffer to store incoming data dynamically
                    List<byte> dynamicBuffer = new List<byte>();

                    MessageType[] messageTypes = MessageTypeLoader.LoadMessageTypesFromJson("C:\\Users\\impul\\Documents\\MaxwellProtocolSoftware\\TOMessage.json");
                    // Encode and send a WMBUS message (example message)
                    Message message1 = new Message
                    {
                        MessageData = "3",
                        TypeofData = messageTypes[0]
                    };

                    byte[] encodedMessage = WMBUSProtocol.EncodeWMBUSMessage(0x01, message1);
                    serialPort.Write(encodedMessage, 0, encodedMessage.Length);
                    Console.WriteLine("Sent encoded WMBUS message.");

                    // Read and decode WMBUS messages
                    while (true)
                    {
                        try
                        {
                            // Read incoming bytes into a temporary buffer
                            byte[] tempBuffer = new byte[256];
                            int bytesRead = serialPort.Read(tempBuffer, 0, tempBuffer.Length);

                            if (bytesRead > 0)
                            {
                                // Add only the valid portion of tempBuffer to dynamicBuffer
                                for (int i = 0; i < bytesRead; i++)
                                {
                                    dynamicBuffer.Add(tempBuffer[i]);
                                }

                                // Process messages from the buffer
                                ProcessBuffer(dynamicBuffer);

                                serialPort.Write(encodedMessage, 0, encodedMessage.Length);
                                Console.WriteLine("Sent encoded WMBUS message.");

                                // Ensure buffer does not grow uncontrollably
                                if (dynamicBuffer.Count > MaxBufferSize)
                                {
                                    Console.WriteLine("Warning: Buffer exceeded maximum size. Clearing buffer to prevent memory issues.");
                                    dynamicBuffer.Clear();
                                }
                            }
                        }
                        catch (TimeoutException)
                        {
                            // Timeout: no data received within the specified period
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Unexpected error: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                        Console.WriteLine("Serial port closed.");
                    }
                }
            }
        }

        private static void ProcessBuffer(List<byte> buffer)
        {
            while (true)
            {
                // Look for the newline character as the delimiter
                int newlineIndex = buffer.IndexOf((byte)'\n');
                if (newlineIndex == -1)
                {
                    // No complete message in the buffer yet
                    break;
                }

                // Extract the full message (up to the newline character)
                byte[] messageBytes = buffer.GetRange(0, newlineIndex).ToArray();

                // Remove the processed message (including the newline character) from the buffer
                buffer.RemoveRange(0, newlineIndex + 1);

                // Decode the WMBUS message
                ProtocolMessage message = WMBUSProtocol.DecodeWMBUSMessage(messageBytes, messageBytes.Length);

                if (message != null)
                {
                    //Console.WriteLine("Received WMBUS message:");
                    Console.WriteLine(message.ToString());
                }
                else
                {
                    Console.WriteLine("Failed to decode WMBUS message.");
                }
            }
        }
    }
}
