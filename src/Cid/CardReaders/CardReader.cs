using System;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Cid.CardReaders.Messages.Ccid.Requests;
using Cid.CardReaders.Messages.Ccid.Responses;
using Thread = System.Threading.Thread;

namespace Cid.CardReaders
{
    public class CardReader : IDisposable
    {
        public event EventHandler<SlotStateChange> OnSlotStateChange;

        public CcidDescriptor Descriptor { get; private set; }

        public bool SupportsInterrupt { get; }

        private readonly UsbDevice _device;
        private UsbDeviceConnection _connection;

        private readonly UsbInterface _interface;

        private readonly UsbEndpoint _bulkOutPipe;
        private readonly UsbEndpoint _bulkInPipe;
        private readonly UsbEndpoint _interruptPipe;

        private Thread _interruptListenerThread;

        private int _sequence;


        private readonly byte[] _receiveBuffer;

        public CardReader(UsbDevice device)
        {
            _device = device;
            _interface = device.GetInterface(0);

            for (var i = 0; i < _interface.EndpointCount; i++)
            {
                var endpoint = _interface.GetEndpoint(i);
                switch (endpoint.Direction)
                {
                    case UsbAddressing.In when endpoint.Type == UsbAddressing.XferInterrupt:
                        _interruptPipe = endpoint;
                        break;
                    case UsbAddressing.In when endpoint.Type == UsbAddressing.XferBulk:
                        _bulkInPipe = endpoint;
                        _receiveBuffer = new byte[_bulkInPipe.MaxPacketSize];
                        break;
                    case UsbAddressing.Out when endpoint.Type == UsbAddressing.XferBulk:
                        _bulkOutPipe = endpoint;
                        break;
                }
            }

            SupportsInterrupt = _interruptPipe != null;
        }

        public void Start()
        {
            var manager = (UsbManager)Application.Context.GetSystemService(Context.UsbService);
            _connection = manager.OpenDevice(_device);
            var claimed = _connection.ClaimInterface(_interface, true);
            if (!claimed)
                throw new Exception("Interface could not be claimed");

            var rawDescriptors = _connection.GetRawDescriptors();
            Descriptor = new CcidDescriptor(rawDescriptors);

            if (SupportsInterrupt)
            {
                _interruptListenerThread = new Thread(StartListeningOnInterruptEndpoint);
                _interruptListenerThread.Start();
            }
        }

        public void ActivateSlot(int slotNumber)
        {
            var powerOnResponse = SendAndReceiveResponse<RdrToPcDataBlock>(new PcToRdrIccPowerOn(slotNumber));
            if (powerOnResponse.CommandStatus == CommandStatus.Failed)
                throw new Exception("Command Failed");
        }

        public void DeactivateSlot(int slotNumber)
        {
            var response = SendAndReceiveResponse<RdrToPcDataBlock>(new PcToRdrIccPowerOff(slotNumber));
            if (response.CommandStatus == CommandStatus.Failed)
                throw new Exception("Command Failed");
        }

        public SlotStatus GetSlotStatus(int slotNumber)
        {
            var response = SendAndReceiveResponse<RdrToPcSlotStatus>(new PcToRdrGetSlotStatus(slotNumber));

            return response.SlotStatus;
        }

        public PcScResponse SendApdu(int slotNumber, IApdu apdu)
        {
            var response = SendAndReceiveResponse<RdrToPcDataBlock>(new PcToRdrXfrBlock(slotNumber, apdu));

            if (response.CommandStatus == CommandStatus.Failed)
                throw new Exception("Command Failed");

            var pcscResponse = new PcScResponse(response.Data);
            if (pcscResponse.Sw1 != Sw1.Normal)
                throw new Exception($"PC/SC response does not reflect success. SW1={pcscResponse.Sw1}");

            return pcscResponse;
        }

        private T SendAndReceiveResponse<T>(ICcidRequest request) where T : ICcidResponse, new()
        {
            var message = request.ToBytes(_sequence++);

            var bytesTransferred = _connection.BulkTransfer(_bulkOutPipe, message, message.Length, 0);
            if (bytesTransferred < 10)
                throw new Exception("Transfer did not send all bytes");

            var bytesReceived = _connection.BulkTransfer(_bulkInPipe, _receiveBuffer, _receiveBuffer.Length, 0);

            if (bytesReceived < 10)
                throw new Exception("Did not receive expected number of bytes");

            var response = new T();
            response.Parse(_receiveBuffer);

            return response;
        }

        public void Dispose()
        {
            _interruptListenerThread?.Abort();
            _connection?.ReleaseInterface(_interface);
            _connection?.Close();
        }

        private void StartListeningOnInterruptEndpoint()
        {
            var buffer = new byte[100];

            while (true)
            {
                var bytesReceived = _connection.BulkTransfer(_interruptPipe, buffer, buffer.Length, 100);
                if (bytesReceived == 2 && buffer[0] == 0x50)
                {
                    ParseSlotChange(buffer[1]);
                }
                else if (bytesReceived == -1)
                {

                }
            }
        }

        private void ParseSlotChange(byte slotChangeMessage)
        {
            for (var i = 0; i < 3; i++)
            {
                var status = 1 << (1 + i * 2) & slotChangeMessage;
                if (status <= 0)
                    continue;

                var state = 1 << (i * 2) & slotChangeMessage;
                var change = new SlotStateChange
                {
                    SlotNumber = i,
                    IccPresent = state > 0
                };

                OnSlotStateChange?.Invoke(this, change);
            }
        }
    }
}
