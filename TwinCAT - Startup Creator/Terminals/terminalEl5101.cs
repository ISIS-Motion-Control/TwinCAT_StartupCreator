using Windows.UI.Xaml.Controls;

namespace TwinCAT___Startup_Creator
{
    public partial class MainPage : Page
    {
        private GenericTerminal terminalEL5101;
        private terminalParameter[] el5101ParameterList = new terminalParameter[]
        {
            new terminalParameter(false, "Enc settings - Disable filter (0:False 1:True)", "PS", "0", "8000","08"),
                new terminalParameter(false, "Enc settings - Enable microincrements (0:False 1:True)","PS","0","8000","0A"),
                new terminalParameter(false, "Enc settings - Reversion of rotation (0:False 1:True)","PS","0","8000","0E"),
                new terminalParameter(true,"Motor Settings - Max current (mA)","PS","5000","8010","01"),
                new terminalParameter(true,"Motor Settings - Reduced current (mA)","PS","2500","8010","02"),
                new terminalParameter(true,"Motor Settings - Nominal voltage (mV)","PS","50000","8010","03"),
                new terminalParameter(true,"Motor Settings - Coil resistance (0.01Ohm)","PS","100","8010","04")
        };
    }
}