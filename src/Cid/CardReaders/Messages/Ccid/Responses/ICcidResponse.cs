namespace Cid.CardReaders.Messages.Ccid.Responses
{
    public interface ICcidResponse
    {
        void Parse(byte[] message);
    }
}
