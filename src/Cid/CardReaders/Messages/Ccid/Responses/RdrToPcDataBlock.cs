using System;

namespace Cid.CardReaders.Messages.Ccid.Responses
{
    public class RdrToPcDataBlock : ICcidResponse
    {
        public int SlotIndex { get; private set; }
        public SlotStatus SlotStatus { get; private set; }
        public CommandStatus CommandStatus { get; private set; }

        public byte[] Data { get; private set; }

        public void Parse(byte[] message)
        {
            SlotIndex = message[5];
            SlotStatus = (SlotStatus)message[7];
            CommandStatus = (CommandStatus)message[8];

            var dataLength = BitConverter.ToInt32(message, 1);
            Data = new byte[dataLength];
            Buffer.BlockCopy(message, 10, Data, 0, dataLength);
        }
    }
}
