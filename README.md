The project is about encoding and decoding messages for a protocol.

Initially, it was to confirm that COM ports could be opened and used. 

RealTerm serial terminal was used to test the COM ports as well as VSPE which created virtual ports to mimic communication between PC and microcontroller.

To start off, the Protocol class is the structure of the protocol.

```C#
public class ProtocolMessage
    {
        public byte MessageID { get; set; }
        public int MessageLength { get; set; }
        public int MessageTypeID { get; set; }
        public byte[] Data { get; set; }
        public byte Checksum { get; set; }
        public bool IsValidChecksum { get; set; }

        public override string ToString()
        {
            return $"Message ID: {MessageID}\n" +
                   $"Message Length: {MessageLength}\n" +
                   $"MessageType ID: {MessageTypeID}\n" +
                   $"Data: {BitConverter.ToString(Data).Replace("-", " ")}\n" +
                   $"Checksum: 0x{Checksum:X2}\n" +
                   $"Is Checksum Valid: {IsValidChecksum}";
        }
    }
```
The message that is being sent and received have the following members and functions:
MessageID which identifies the message.
MessageLength which tells us the length of the message.
MessageTypeID which identifies what type of message is being sent or received.
Data which is the actual data being sent.
Checksum which is the messageID byte that is added to all the bytes of the messages and then modded by 255.
IsValidChecksum which is a boolean to check if the Checksum is correct.
