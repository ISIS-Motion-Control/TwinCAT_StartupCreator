using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwinCAT___Startup_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        readonly ObservableCollection<string> ListOfTerminals = new ObservableCollection<string>() { "EL7041", "Technosoft 8020" };
        string selectedTerminal;
        public XmlDocument xmlDoc;
        const string quoteMark = "\"";
        private terminalParameter[] terminalParameterList1 = new terminalParameter[] { new terminalParameter(false, "Enc settings - Disable filter (0:False 1:True)", "PS", "0", "8000", "08") };
        private terminalParameter[] terminalParameterList2 = new terminalParameter[] { new terminalParameter(false, "Enc settings - Disable filter (0:False 1:True)", "PS", "0", "8000", "08"), new terminalParameter(false, "Enc settings - Disable filter (0:False 1:True)", "PS", "0", "8000", "08") };
        private GenericTerminal genericTerminal1;
        private GenericTerminal genericTerminal2;
        TerminalEL7041 testEL7041 = new TerminalEL7041();
        Page techno8020;
        Page el7041;

        Windows.Storage.StorageFolder folder;

        public MainPage()
        {
            this.InitializeComponent();
            genericTerminal1 = new GenericTerminal(terminalParameterList1);
            genericTerminal2 = new GenericTerminal(terminalParameterList2);
            techno8020 = new EL7041(genericTerminal1);
            el7041 = new EL7041(genericTerminal2);
        }

        private async void messageBoxPopup(string text2show)
        {
            var messageDialog = new MessageDialog(text2show);
            await messageDialog.ShowAsync();
        }

        private async void generateStartupButton_Click(object sender, RoutedEventArgs e)
        {
            if (folder==null)
            {
                messageBoxPopup("Select a save directory before generating file");
                return;
            }
            List<string> startupString = new List<string>(); 
            selectedTerminal = terminalSelectionComboBox.SelectedItem.ToString();
            GenericTerminal terminal4Startup;
            
            switch (selectedTerminal)
            {
                case "EL7041":
                    EL7041 el7041Page = (EL7041)terminalFrame.Content;
                    TerminalEL7041 terminalEL7041 = el7041Page.TerminalEL7041;

                    terminal4Startup = ((EL7041)el7041).GenericTerminal;
                    terminal4Startup.Reset();
                    
                    startupString = beckhoffBoilerPlateStart(startupString);
                    terminalEL7041.Reset();
                    //foreach (terminalParameter param in terminalEL7041)
                    foreach (terminalParameter param in terminal4Startup)   //Now we loop through the generic
                    {
                        if( param.Include)
                        {
                            startupString = beckhoffInitCmd(startupString, param);
                        }
                    }
                    startupString = beckhoffBoilerPlateEnd(startupString);
                    Windows.Storage.StorageFile testFile = await folder.CreateFileAsync(fileName.Text + @".xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteLinesAsync(testFile, startupString);
                    break;
            }
        }

        public List<string> beckhoffBoilerPlateStart(List<string> startupString)
        {
            startupString.Add(@"<?xml version=" + quoteMark + "1.0" + quoteMark + " encoding=" + quoteMark + "utf-8" + quoteMark + "?>");
            startupString.Add(@"<EtherCATMailbox>");
            startupString.Add("\t<CoE>");
            startupString.Add("\t\t<InitCmds>");
            return startupString;
        }
        public List<string> beckhoffBoilerPlateEnd(List<string> startupString)
        {
            startupString.Add("\t\t</InitCmds>");
            startupString.Add("\t</CoE>");
            startupString.Add(@"</EtherCATMailbox>");
            return startupString;
        }
        public List<string> beckhoffInitCmd(List<string> startupString, terminalParameter parameter)
        {
            //Index conversion here
            string decIndex = (Convert.ToInt64(parameter.Index, 16)).ToString();
            //SubIndex conversion here
            string decSubIndex = (Convert.ToInt64(parameter.SubIndex, 16)).ToString();
            //Data conversion here
            string dataConversion = Convert.ToInt64(parameter.Data, 10).ToString("x4");

            int dataLength = dataConversion.Length;
            char[] flippedData = new char[dataLength];
            for(int i=0; i<dataLength; i++)
            {
                flippedData[i] = dataConversion[dataLength -2- i];
                flippedData[i + 1] = dataConversion[dataLength - 1 - i];
                i++;
            }
            string chars2Str = new string(flippedData);
            char c = dataConversion[1];
            

            startupString.Add("\t\t\t<InitCmd>");
            startupString.Add("\t\t\t\t<Transition>" + parameter.Transition + "</Transition>");
            startupString.Add("\t\t\t\t<Timeout>" + parameter.Timeout + "</Timeout>");
            startupString.Add("\t\t\t\t<Ccs>" + parameter.CCS + "</Ccs>");
            startupString.Add("\t\t\t\t<Comment>" + parameter.Name + "</Comment>");
            startupString.Add("\t\t\t\t<Index>" + decIndex+ "</Index>"); //this needs to be converted from HEX to DEC
            startupString.Add("\t\t\t\t<SubIndex>" + decSubIndex + "</SubIndex>"); //this needs to be converted from hex to DEC
            startupString.Add("\t\t\t\t<Data>" + chars2Str + "</Data>"); //this needs to be converted from DEC to LSB HEX
            startupString.Add("\t\t\t</InitCmd>");
            return startupString;
        }




/////////////////////////////////////////////////////////////

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTerminal = terminalSelectionComboBox.SelectedItem.ToString();
            switch (selectedTerminal)
            {
                case "EL7041":
                    terminalFrame.Content = el7041;
                    //terminalFrame.Navigate(typeof(EL7041));
                    break;
                case "Technosoft 8020":
                    terminalFrame.Content = techno8020;
                    //terminalFrame.Navigate(typeof(technosoft8020));
                    break;
            }
        }

        


        private async void folderSelectButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FolderPicker folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");
            folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            }
        }
    }

    


    public class GenericTerminal : IEnumerator, IEnumerable
    {
        private terminalParameter[] _terminalParameterList;
        int position = -1;

        public GenericTerminal(terminalParameter[] terminalParameterList)
        {
            _terminalParameterList = terminalParameterList;
        }
        //IEnumerator and IEnumerable require these methods.
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        //IEnumerator
        public bool MoveNext()
        {
            position++;
            return (position < _terminalParameterList.Length);
        }
        //IEnumerable
        public void Reset()
        {
            position = -1;
        }
        public object Current
        {
            get { return _terminalParameterList[position]; }
        }
        public object parameter(int paramIndex)
        {
            return _terminalParameterList[paramIndex];
        }
    }



    public class TerminalEL7041 : IEnumerator,IEnumerable
    {
        private terminalParameter[] terminalParameterList;
        int position = -1;

        public TerminalEL7041()
        {
            terminalParameterList = new terminalParameter[]
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
                new terminalParameter(false,"Controller Settings - Inner window (###)","PS","0","8011","03"),
                new terminalParameter(false,"Controller Settings - Outer window (###)","PS","0","8011","05"),
                new terminalParameter(false,"Controller Settings - Filter cut off frequency (###)","PS","0","8011","06"),
                new terminalParameter(false,"Controller Settings - Ka factor (curr) (###)","PS","100","8011","07"),
                new terminalParameter(false,"Controller Settings - Kd factor (curr) (###)","PS","100","8011","08"),
                new terminalParameter(true,"Features - Speed Range (0:1k, 1:2k, 2:4k, 3:8k, 4:16k, 5:32k)","PS","1","8012","05"),
                new terminalParameter(true,"Feedback type (0: encoder, 1: internal counter)","PS","1","8012","08"),
                new terminalParameter(true,"Invert motor polarity (0:False 1:True)","PS","0","8012","09")
            };
        }
        //IEnumerator and IEnumerable require these methods.
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        //IEnumerator
        public bool MoveNext()
        {
            position++;
            return (position < terminalParameterList.Length);
        }
        //IEnumerable
        public void Reset()
        {
            position = -1;
        }
        public object Current
        {
            get { return terminalParameterList[position]; }
        }
        public object parameter(int paramIndex)
        {
            return terminalParameterList[paramIndex];
        }
    }

    public class terminalParameter
    {
        private bool _include = false;
        private string _name;
        private string _transition;
        private string _data;
        private string _index;
        private string _subindex;
        private string _ccs;
        private string _timeout;
        public terminalParameter(bool Include, string Name, string Transition, string Data, string Index, string SubIndex, string CCS = "1", string Timeout = "0")
        {
            _include = Include;
            _name = Name;
            _transition = Transition;
            _data = Data;
            _index = Index;
            _subindex = SubIndex;
            _ccs = CCS;
            _timeout = Timeout;
        }

        public bool Include
        {
            get { return _include; }
            set { _include = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Transition
        {
            get { return _transition; }
            set { _transition = value; }
        }
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }
        public string Index
        {
            get { return _index; }
            set { _index = value; }
        }
        public string SubIndex
        {
            get { return _subindex; }
            set { _subindex = value; }
        }
        public string CCS
        {
            get { return _ccs; }
            set { _ccs = value; }
        }
        public string Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }       
    }

}
