
sadfsd
sdfsad

```C#

        public static byte[] EncodeWMBUSMessage(byte messageID, Message data)
        {
            byte[] payload = Encoding.ASCII.GetBytes(data.MessageData);
            byte checksum = CalculateChecksum(messageID, payload);

            // Construct WMBUS packet: [Message ID] [Message Length] [Payload] [Checksum]
            byte[] wmbusPacket = new byte[2 + payload.Length + 1];
            wmbusPacket[0] = messageID;               // Message ID
            wmbusPacket[1] = (byte)payload.Length;    // Message Length
            wmbusPacket[2] = (byte)data.TypeofData.MessageTypeID;
            Array.Copy(payload, 0, wmbusPacket, 2, payload.Length); // Payload
            wmbusPacket[wmbusPacket.Length - 1] = checksum;         // Checksum

            return wmbusPacket;
        }
```
The Message class contains the basic protocol message type details:
```C#
public class Message
    {
        public string MessageData { get; set; }
        public MessageType TypeofData { get; set; }

    }
```
