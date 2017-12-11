using System;

namespace Cid.CardReaders.Messages.Ccid.Requests
{
    internal class PcToRdrXfrBlock : ICcidRequest
    {
        private readonly int _slotNumber;
        private readonly IApdu _apdu;

        public PcToRdrXfrBlock(int slotNumber, IApdu apdu)
        {
            _slotNumber = slotNumber;
            _apdu = apdu;
        }

        public byte[] ToBytes(int sequence)
        {
            var data = _apdu.GetBytes();

            var message = new byte[10 + data.Length];

            message[0] = 0x6F; // message type
            var lengthBytes=BitConverter.GetBytes(data.Length);
            Buffer.BlockCopy(lengthBytes, 0, message, 1, 4);
            message[5] = (byte) _slotNumber;
            message[6] = (byte) sequence; // sequence number
            message[7] = 0x01;

            Buffer.BlockCopy(data, 0, message, 10, data.Length);

            return message;
        }
    }
}
