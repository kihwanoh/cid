namespace Cid.CardReaders.Messages.Ccid.Requests
{
    internal class PcToRdrIccPowerOff : ICcidRequest
    {
        private readonly int _slotNumber;

        public PcToRdrIccPowerOff(int slotNumber)
        {
            _slotNumber = slotNumber;
        }

        public byte[] ToBytes(int sequence)
        {
            return new byte[]
            {
                0x63, // message type
                0x0, 0x0, 0x0, 0x0, // data length
                (byte)_slotNumber,
                (byte)sequence, // sequence number
                0x0, 0x0, 0x0 // reserved for future use
            };
        }
    }
}
