using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Widget;
using Cid.CardReaders;
using Cid.CardReaders.Messages.Ccid.Responses;
using Cid.CardReaders.Messages.ISO14443;
using Cid.CardReaders.Messages.Wiegand;

namespace Cid.Example
{
    [Activity(Label = "Cid.Example", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private CardReader _reader;
        private TextView _statusText;
        private TextView _cardIdText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _statusText = FindViewById<TextView>(Resource.Id.status);
            _cardIdText = FindViewById<TextView>(Resource.Id.cardId);
        }

        protected override void OnResume()
        {
            GetCardReader();
            base.OnResume();
        }

        protected override void OnStop()
        {
            _reader?.Dispose();
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            _reader?.Dispose();
            base.OnDestroy();
        }

        private void GetCardReader()
        {
            UsbManager manager = (UsbManager) GetSystemService(UsbService);
            var cardReaders = CardReaderProbe.ProbeForCardReaders(manager);

            var device = cardReaders.First();
            if (!manager.HasPermission(device))
            {
                var usbPermission = "Cid.Example.USB_PERMISSION";

                var permissionIntent = PendingIntent.GetBroadcast(this, 0, new Intent(usbPermission), 0);

                var filter = new IntentFilter(usbPermission);

                var receiver = new UsbPermissionReceiver
                {
                    Callback = () => WireUpCardReader(device)
                };

                RegisterReceiver(receiver, filter);

                manager.RequestPermission(device, permissionIntent);
            }
            else
            {
                WireUpCardReader(device);
            }
        }

        private void WireUpCardReader(UsbDevice device)
        {
            _reader?.Dispose();
            _reader = new CardReader(device);
            _reader.OnSlotStateChange += (_, change) =>
            {
                if (change.IccPresent)
                {
                    _reader.ActivateSlot(change.SlotNumber);

                    var status = _reader.GetSlotStatus(change.SlotNumber);
                    if (status == SlotStatus.IccPresentAndActive)
                    {
                        var response = _reader.SendApdu(change.SlotNumber, new GetData());
                        var info = Parser.GetPacInfo(Format.Corporate1000, response.Data);

                        RunOnUiThread(() =>
                        {
                            _statusText.Text = "Card Present";
                            _cardIdText.Text = info.CardId.ToString();
                        });
                    }
                }
                else
                {
                    _reader.DeactivateSlot(change.SlotNumber);

                    RunOnUiThread(() =>
                    {
                        _statusText.Text = "Card Not Present";
                        _cardIdText.Text = "";
                    });
                }
            };
            _reader.Start();
        }
    }

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "Cid.Example.USB_PERMISSION" })]
    public class UsbPermissionReceiver : BroadcastReceiver
    {
        public Action Callback { get; set; }

        public override void OnReceive(Context context, Intent intent)
        {
            Callback?.Invoke();
        }
    }
}
