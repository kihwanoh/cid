namespace Cid.CardReaders.Messages.Ccid.Requests
{
    internal interface ICcidRequest
    {
        byte[] ToBytes(int sequence);
    }
}
