namespace Cid.CardReaders
{
    public class CcidFeatures
    {
        public bool AutoParamConfigViaAtr { get; }
        public bool AutoActivationOnInsert { get; }
        public bool AutoVoltageSelection { get; }
        public bool AutoClockChange { get; }
        public bool AutoDataRateChange { get; }
        public bool AutoParamNegotiation { get; }
        public bool AutoPps { get; }
        public bool CanStopClock { get; }
        public bool NadaAccepted { get; }
        public bool AutoIfsdExchange { get; }
        public bool TPDU { get; }
        public bool ShortAPDU { get; }
        public bool ShortAndExtendedAPDU { get; }
        public bool WakeOnCardAction { get; }

        public CcidFeatures(int features)
        {
            AutoParamConfigViaAtr = (0x02 & features) > 0;
            AutoActivationOnInsert = (0x04 & features) > 0;
            AutoVoltageSelection = (0x08 & features) > 0;
            AutoClockChange = (0x10 & features) > 0;
            AutoDataRateChange = (0x20 & features) > 0;
            AutoParamNegotiation = (0x40 & features) > 0;
            AutoPps = (0x80 & features) > 0;
            CanStopClock = (0x100 & features) > 0;
            NadaAccepted = (0x200 & features) > 0;
            AutoIfsdExchange = (0x400 & features) > 0;
            TPDU = (0x10000 & features) > 0;
            ShortAPDU = (0x20000 & features) > 0;
            ShortAndExtendedAPDU = (0x40000 & features) > 0;
            WakeOnCardAction = (0x00100000 & features) > 0;
        }
    }
}
