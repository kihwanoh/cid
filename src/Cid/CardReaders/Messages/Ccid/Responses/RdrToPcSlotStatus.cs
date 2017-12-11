namespace Cid.CardReaders.Messages.Ccid.Responses
{
    public class RdrToPcSlotStatus : ICcidResponse
    {
        public int SlotIndex { get; private set; }
        public SlotStatus SlotStatus { get; private set; }
        public CommandStatus CommandStatus { get; private set; }

        public void Parse(byte[] message)
        {
            SlotIndex = message[5];
            SlotStatus = (SlotStatus)message[7];
            CommandStatus = (CommandStatus)message[8];
        }
    }
}
