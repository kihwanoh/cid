namespace Cid.CardReaders.Messages.Ccid.Requests
{
    internal class PcToRdrGetSlotStatus : ICcidRequest
    {
        private readonly int _slotNumber;

        public PcToRdrGetSlotStatus(int slotNumber)
        {
            _slotNumber = slotNumber;
        }

        public byte[] ToBytes(int sequence)
        {
            return new byte[]
            {
                0x65, // message type
                0x0, 0x0, 0x0, 0x0, // data length
                (byte)_slotNumber,
                (byte)sequence,
                0x0, 0x0, 0x0 // reserved for future use
            };
        }
    }
}
