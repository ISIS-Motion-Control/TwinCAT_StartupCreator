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
    /* HOW TO ADD NEW TERMINALS
     * Add the terminal name to the ListOfTerminals
     * Create a 'Page' instance for the terminal
     * Create a new partial class for the parameter information
     * Under MainPage() constructor setup the variables and create a terminal page instance with the terminal information as an input
     * Write a CASE for the terminal under "generateStartupButton_Click"
     * Write a CASE for the frame population under "terminalComboBox_SelectionChanged"
     */

    /// <summary>
    /// Main display page for the COE startup creation tool
    /// </summary>
    public partial class MainPage : Page
    {       
        readonly ObservableCollection<string> ListOfTerminals = new ObservableCollection<string>() { "EL7041", "EL7047" }; //List of terminals currently supported in the tool that asks as a source fo combobox in UI
        string selectedTerminal; //Stores the value of the combobox (i.e. the user selection for terminal)
        const string quoteMark = "\""; //constant used for easier creation of strings where a quotation mark is required
        Windows.Storage.StorageFolder folder; //User selected directory for storing the XML output

        //Terminal pages - instances created as MainPage initialised      
        Page el7041Page;
        Page el7047Page;

 
        //Need to clean these up or find a better way to declare them - might just move to end or can I create another partial for MainPage and dump them there for ease of access


        /// <summary>
        /// Constructor for main page. Initialises and instances the terminal pages
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            terminalEL7041 = new GenericTerminal(el7041ParameterList);
            el7041Page = new TerminalPage(terminalEL7041);

            terminalEL7047 = new GenericTerminal(el7047ParameterList);
            el7047Page = new TerminalPage(terminalEL7047);
        }

        /// <summary>
        /// Method for displaying a string to the user
        /// </summary>
        /// <param name="text2show"></param>
        private async void messageBoxPopup(string text2show)
        {
            var messageDialog = new MessageDialog(text2show);
            await messageDialog.ShowAsync();
        }

        /// <summary>
        /// Method for creating and exporting the startup list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void generateStartupButton_Click(object sender, RoutedEventArgs e)
        {
            //Check for a folder selection first
            if (folder==null)
            {
                messageBoxPopup("Select a save directory before generating file");
                return;
            }

            List<string> startupString = new List<string>(); //List created for storing the xml startup string
            GenericTerminal terminal4Startup; //Creating an object to hold the GenericTerminal output from the TerminalPage instances
            Windows.Storage.StorageFile startupFile;


            switch (terminalSelectionComboBox.SelectedItem.ToString())
            {
                case "EL7041":
                    terminal4Startup = ((TerminalPage)el7041Page).Terminal; //Recast from Page to TerminalPage
                    terminal4Startup.Reset();   //Reset required as foreach position does not seem to be resetting
                    
                    startupString = beckhoffBoilerPlateStart(startupString);
                    //foreach (terminalParameter param in terminalEL7041)
                    foreach (terminalParameter param in terminal4Startup)   //Now we loop through the generic
                    {
                        if( param.Include)
                        {
                            startupString = beckhoffInitCmd(startupString, param);
                        }
                    }
                    startupString = beckhoffBoilerPlateEnd(startupString);
                    startupFile = await folder.CreateFileAsync(fileName.Text + @".xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteLinesAsync(startupFile, startupString);
                    break;
                
                case "EL7047":
                    terminal4Startup = ((TerminalPage)el7047Page).Terminal;
                    terminal4Startup.Reset();
                    startupString = beckhoffBoilerPlateStart(startupString);
                    foreach (terminalParameter param in terminal4Startup)   //Now we loop through the generic
                    {
                        if (param.Include)
                        {
                            startupString = beckhoffInitCmd(startupString, param);
                        }
                    }
                    startupString = beckhoffBoilerPlateEnd(startupString);
                    startupFile = await folder.CreateFileAsync(fileName.Text + @".xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteLinesAsync(startupFile, startupString);
                    break;
            }
        }

        /// <summary>
        /// Takes in and returns a startupstring list and adds the generic required beckhoff startup information for starting the xml file
        /// </summary>
        /// <param name="startupString"></param>
        /// <returns></returns>
        public List<string> beckhoffBoilerPlateStart(List<string> startupString)
        {
            startupString.Add(@"<?xml version=" + quoteMark + "1.0" + quoteMark + " encoding=" + quoteMark + "utf-8" + quoteMark + "?>");
            startupString.Add(@"<EtherCATMailbox>");
            startupString.Add("\t<CoE>");
            startupString.Add("\t\t<InitCmds>");
            return startupString;
        }

        /// <summary>
        /// Takes in and returns a startupstring list and adds the generic required beckhoff startup information for ending the xml file
        /// </summary>
        /// <param name="startupString"></param>
        /// <returns></returns>
        public List<string> beckhoffBoilerPlateEnd(List<string> startupString)
        {
            startupString.Add("\t\t</InitCmds>");
            startupString.Add("\t</CoE>");
            startupString.Add(@"</EtherCATMailbox>");
            return startupString;
        }
        
        /// <summary>
        /// Takes in and returns the startuplist string. Adds individual COE parameter xml information to the startup.
        /// </summary>
        /// <param name="startupString"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public List<string> beckhoffInitCmd(List<string> startupString, terminalParameter parameter)
        {
            string decIndex = (Convert.ToInt64(parameter.Index, 16)).ToString(); //Address index converted to DEC from HEX
            string decSubIndex = (Convert.ToInt64(parameter.SubIndex, 16)).ToString(); //Address subindex converted to DEC from HEX
            
            //Data needs to be converted from DEC to LSB HEX
            string dataConversion = Convert.ToInt64(parameter.Data, 10).ToString("x4"); //perform initial conversion to HEX in lower case format ("x4" modifier)
            int dataLength = dataConversion.Length; //determine length of data for use in MSB to LSB conversion
            char[] flippedData = new char[dataLength]; //create char array for holding the MSB/LSB converted data
            //iterate over flipping byte positions, double increment of i required as each byte is two chars
            for(int i=0; i<dataLength; i++)
            {
                flippedData[i] = dataConversion[dataLength -2 - i];
                flippedData[i + 1] = dataConversion[dataLength - 1 - i];
                i++;
            }
            string chars2Str = new string(flippedData); //convert char array back to string for entry in to COE startup string

            //Populate the startupstring with parameter data
            startupString.Add("\t\t\t<InitCmd>");
            startupString.Add("\t\t\t\t<Transition>" + parameter.Transition + "</Transition>");
            startupString.Add("\t\t\t\t<Timeout>" + parameter.Timeout + "</Timeout>");
            startupString.Add("\t\t\t\t<Ccs>" + parameter.CCS + "</Ccs>");
            startupString.Add("\t\t\t\t<Comment>" + parameter.Name + "</Comment>");
            startupString.Add("\t\t\t\t<Index>" + decIndex+ "</Index>");
            startupString.Add("\t\t\t\t<SubIndex>" + decSubIndex + "</SubIndex>");
            startupString.Add("\t\t\t\t<Data>" + chars2Str + "</Data>");
            startupString.Add("\t\t\t</InitCmd>");
            return startupString;
        }

        /// <summary>
        /// Method for repopulating the mainpage terminalframe when a new terminal is selected with the dropdown box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void terminalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTerminal = terminalSelectionComboBox.SelectedItem.ToString();
            switch (selectedTerminal)
            {
                case "EL7041":
                    terminalFrame.Content = el7041Page;
                    break;
                case "EL7047":
                    terminalFrame.Content = el7047Page;
                    break;
            }
        }

        

        /// <summary>
        /// Method for selecting a directory to export startup xml to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

    

    /// <summary>
    /// Class for a generic terminal that can hold a list of terminal parameters for iterating over with a foreach
    /// Doesn't work perfectly as the reset is not implemented properly, something about needing to return a new instance of this as opposed to the instance of it.
    /// More googling required. Can be made to work as is just requires a manual reset before iterating over.
    /// </summary>
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
    /// <summary>
    /// Class to define elements required for a COE startup parameter
    /// </summary>
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

    

}
