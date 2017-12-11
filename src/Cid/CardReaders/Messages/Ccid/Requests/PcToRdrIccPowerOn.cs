namespace Cid.CardReaders.Messages.Ccid.Requests
{
    internal class PcToRdrIccPowerOn : ICcidRequest
    {
        private readonly int _slotNumber;

        public PcToRdrIccPowerOn(int slotNumber)
        {
            _slotNumber = slotNumber;
        }

        public byte[] ToBytes(int sequence)
        {
            return new byte[]
            {
                0x62, // message type
                0x0, 0x0, 0x0, 0x0, // data length
                (byte)_slotNumber,
                (byte)sequence, // sequence number
                0x00, // voltage selection
                0x00, 0x00 // reserved for future use
            };
        }
    }
}
