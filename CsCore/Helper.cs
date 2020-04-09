using CSCore.CoreAudioAPI;
using System.Collections.Generic;

namespace Orpheus.CsCore
{
    public static class Helper
    {
        public static IEnumerable<MMDevice> GetOutputDevice()
        {
            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        yield return device;
                    }
                }
            }
        }
    }
}
