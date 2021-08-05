using Windows.UI.Xaml.Controls;

namespace TwinCAT___Startup_Creator
{
    public partial class MainPage : Page
    {
        private GenericTerminal terminalEL7047;
        private terminalParameter[] el7047ParameterList = new terminalParameter[]
        {
            new terminalParameter(false, "Enc settings - Disable filter (0:False 1:True)", "PS", "0", "8000","08"),
                new terminalParameter(false, "Enc settings - Enable microincrements (0:False 1:True)","PS","0","8000","0A"),
                new terminalParameter(false, "Enc settings - Reversion of rotation (0:False 1:True)","PS","0","8000","0E"),
                new terminalParameter(true,"Motor Settings - Max current (mA)","PS","5000","8010","01"),
                new terminalParameter(true,"Motor Settings - Reduced current (mA)","PS","2500","8010","02"),
                new terminalParameter(true,"Motor Settings - Nominal voltage (mV)","PS","50000","8010","03"),
                new terminalParameter(true,"Motor Settings - Coil resistance (0.01Ohm)","PS","100","8010","04"),
                new terminalParameter(false,"Motor Settings - Motor EMF (###)","PS","0","8010","05"),
                new terminalParameter(true,"Motor Settings - Fullsteps","PS","200","8010","06"),
                new terminalParameter(false,"Motor Settings - Encoder Increments (4-fold)","PS","0","8010","07"),
                new terminalParameter(false,"Controller Settings - Kp factor (curr) (###)","PS","400","8011","01"),
                new terminalParameter(false,"Controller Settings - Ki factor (curr) (###)","PS","4","8011","02"),
                new terminalParameter(true,"Features - Speed Range (0:1k, 1:2k, 2:4k, 3:8k, 4:16k, 5:32k)","PS","1","8012","05"),
                new terminalParameter(true,"Feedback type (0: encoder, 1: internal counter)","PS","1","8012","08"),
                new terminalParameter(true,"Invert motor polarity (0:False 1:True)","PS","0","8012","09"),
                new terminalParameter(false, "Error on step lost (0:False 1:True)","PS", "0","8012","0A"),
                new terminalParameter(false, "Fan cartridge present (0:False 1:True)","PS","0","8012","0B")
        };
    }
}