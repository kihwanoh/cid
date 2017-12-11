using Cid.CardReaders.Messages.Ccid.Requests;

namespace Cid.CardReaders.Messages.ISO14443
{
    public class GetData : IApdu
    {
        public byte[] GetBytes()
        {
            return new byte[] {0xff, 0xca, 0x00, 0x00, 0x00};
        }
    }
}
