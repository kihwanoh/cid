using System.Collections.Generic;
using System.Linq;
using Android.Hardware.Usb;

namespace Cid.CardReaders
{
    public static class CardReaderProbe
    {
        public static Dictionary<int, List<int>> SupportedProducts = new Dictionary<int, List<int>>
        {
            {0x076B, new List<int> { 0x502A } }
        };

        public static List<UsbDevice> ProbeForCardReaders(UsbManager usbManager)
        {
            return usbManager.DeviceList.Values
                .Where(IsDeviceSupported)
                .ToList();
        }

        private static bool IsDeviceSupported(UsbDevice device)
        {
            return SupportedProducts.ContainsKey(device.VendorId) && SupportedProducts[device.VendorId].Contains(device.ProductId);
        }
    }
}
