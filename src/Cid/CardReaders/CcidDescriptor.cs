using System;

namespace Cid.CardReaders
{
    public class CcidDescriptor
    {
        public int MaxSlotIndex { get; }
        public CcidFeatures Features { get; }

        public CcidDescriptor(byte[] rawDescriptor)
        {
            var slice = new byte[rawDescriptor[36]];

            Buffer.BlockCopy(rawDescriptor, 36, slice, 0, slice.Length);

            MaxSlotIndex = slice[4];

            var protocols = BitConverter.ToInt32(slice, 6);
            var features = BitConverter.ToInt32(slice, 40);

            Features = new CcidFeatures(features);
        }
    }
}
