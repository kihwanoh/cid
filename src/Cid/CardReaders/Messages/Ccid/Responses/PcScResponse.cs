using System;

namespace Cid.CardReaders.Messages.Ccid.Responses
{
    public class PcScResponse
    {
        public Sw1 Sw1 { get; }
        public byte Sw2 { get; }

        public byte[] Data { get; }

        public PcScResponse(byte[] message)
        {
            if(message.Length < 2)
                throw new Exception($"PC/SC response is not long enough. Was '{message.Length}' expected Length >= 2");

            Sw1 = (Sw1)message[message.Length - 2];
            Sw2 = message[message.Length - 1];

            Data = new byte[message.Length -2];
            Buffer.BlockCopy(message, 0, Data, 0, Data.Length);
        }
    }

    public enum Sw1
    {
        ErrorFunctionNotSupported = 0x68,
        ErrorP3Incorrect = 0x6C,
        Normal = 0x90
    }
}
