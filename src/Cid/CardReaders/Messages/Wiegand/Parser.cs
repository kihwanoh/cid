using System;
using System.Linq;
using Cid.CardReaders.Messages.ISO14443;

namespace Cid.CardReaders.Messages.Wiegand
{
    public static class Parser
    {
        public static PacInfo GetPacInfo(Format format, byte[] data)
        {
            var info = new PacInfo();
            switch (format)
            {
                case Format.Corporate1000:
                    if (data.Length != 5)
                        throw new Exception($"Incorrect data length for Corporate1000. Was {data.Length} expected 5");

                    var fullLongBytes = new byte[8];
                    Buffer.BlockCopy(data, 0, fullLongBytes, 3, 5);
                    var fullLong = BitConverter.ToUInt64(fullLongBytes.Reverse().ToArray(), 0);
                    info.CardId = (fullLong & 0x3FFFFc0) >> 6;
                    return info;
                default:
                    throw new Exception($"Format {format} not supported yet");
            }
        }
    }
}
