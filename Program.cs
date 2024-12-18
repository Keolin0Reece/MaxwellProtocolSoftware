using ConsoleApp1;
using ConsoleApp1.Models;
using System;
using System.IO.Ports;

namespace WMBUSProtocol
{
    public class Program
    {
        public static void Main()
        {
            string comPort = "COM1"; // Replace with your COM port
            int baudRate = 9600;
            byte[] buffer = new byte[256]; // Buffer to read raw bytes

      
            MessageType[] messageTypes = MessageTypeLoader.LoadMessageTypesFromJson("C:\\Users\\impul\\Documents\\ConsoleApp1\\TOMessage.json");
            // Display the loaded message types
            foreach (var messageType in messageTypes)
            {
                Console.WriteLine($"Name: {messageType.Name}");
                Console.WriteLine($"MessageID: {messageType.MessageTypeID   }");
                Console.WriteLine($"DataType: {messageType.DataType}");
                Console.WriteLine($"Description: {messageType.Description}");
                Console.WriteLine();
            }

            using (SerialPort serialPort = new SerialPort(comPort, baudRate))
            {
                try
                {
                    serialPort.ReadTimeout = 1000; // 1-second timeout
                    serialPort.Open();
                    Console.WriteLine($"Connected to {comPort} at {baudRate} baud.");

                    // Encode and send a WMBUS message
                    Message message1 = new Message();
                    message1.MessageData = "He";
                    MessageType message1type = new MessageType();
                    message1.TypeofData = message1type;
                    message1.TypeofData.Name = "A";
                    message1.TypeofData.DataType = "Int";
                    message1.TypeofData.Description = "B";
                    message1.TypeofData.MessageTypeID = 0;
                    byte[] encodedMessage = WMBUSProtocol.EncodeWMBUSMessage(0x01, message1);
                    serialPort.Write(encodedMessage, 0, encodedMessage.Length);
                    Console.WriteLine("Sent encoded WMBUS message.");

                    // Read and decode WMBUS message
                    while (true)
                    {
                        try
                        {
                            int bytesRead = serialPort.Read(buffer, 0, buffer.Length);

                            if (bytesRead > 0)
                            {
                                ProtocolMessage message = WMBUSProtocol.DecodeWMBUSMessage(buffer, bytesRead);

                                if (message != null)
                                {
                                    Console.WriteLine("Received WMBUS message:");
                                    Console.WriteLine(message.ToString());
                                }
                                else
                                {
                                    Console.WriteLine("Failed to decode WMBUS message.");
                                }
                            }
                        }
                        catch (TimeoutException)
                        {
                            Console.WriteLine("No data received within the timeout period.");
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
    }
}
