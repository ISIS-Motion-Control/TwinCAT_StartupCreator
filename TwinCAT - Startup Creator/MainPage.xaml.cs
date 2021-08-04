using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwinCAT___Startup_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        readonly ObservableCollection<string> ListOfTerminals = new ObservableCollection<string>();
        string selectedTerminal;
        public XmlDocument xmlDoc;
        const string quoteMark = "\"";

        Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
        Windows.Storage.StorageFolder folder;

        public MainPage()
        {
            this.InitializeComponent();
            GenerateTerminalList();
            
        }





/// ////////////////////////////////////////////////////////
        private async void generateStartupButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> startupString = new List<string>();
       
            selectedTerminal = terminalSelectionComboBox.SelectedItem.ToString();
            switch (selectedTerminal)
            {
                case "EL7041":
                    
                    EL7041 el7041Page = (EL7041)terminalFrame.Content;
                    var x = el7041Page.TerminalEL7041;
                    
                    startupString = beckhoffBoilerPlateStart(startupString);

                    x.Reset();
                    //foreach(terminalParameter parameter in terminalEL7041)
                    foreach (terminalParameter param in x)
                    {
                        startupString.Add(param.Index);
                        startupString.Add(param.Include.ToString());
                        if( param.Include)
                        {
                            startupString = beckhoffInitCmd(startupString, param);
                            startupString.Add("Testing");
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


            startupString.Add("\t\t\t<InitCmd>");
            startupString.Add("\t\t\t\t<Transition>" + parameter.Transition + "</Transition>");
            startupString.Add("\t\t\t\t<Timeout>" + parameter.Timeout + "</Timeout>");
            startupString.Add("\t\t\t\t<Ccs>" + parameter.CCS + "</Ccs>");
            startupString.Add("\t\t\t\t<Comment>" + parameter.Name + "</Comment>");
            startupString.Add("\t\t\t\t<Index>" + decIndex+ "</Index>"); //this needs to be converted from HEX to DEC
            startupString.Add("\t\t\t\t<SubIndex>" + decSubIndex + "</SubIndex>"); //this needs to be converted from hex to DEC
            startupString.Add("\t\t\t\t<Data>" + parameter.Data + "</Data>"); //this needs to be converted from DEC to LSB HEX
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
                    terminalFrame.Navigate(typeof(EL7041));
                    break;
                case "Technosoft 8020":
                    terminalFrame.Navigate(typeof(technosoft8020));
                    break;
            }          
        }

        private void GenerateTerminalList()
        {
            ListOfTerminals.Add("EL7041");
            ListOfTerminals.Add("Technosoft 8020");
        }


        private async void folderSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");
            folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            }
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
                new terminalParameter(true, "Test", "PS", "5000", "8011", "01"),
                new terminalParameter(false, "Test2", "IP", "420", "8015", "0A")
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

/*
        public terminalParameter i8000_08 = new terminalParameter
        {
            Include = false,
            Name = "Enc settings - Disable fitler",
            Transition = "PS",
            Data = "0",
            Index = "8000",
            SubIndex = "08"
        };
        public terminalParameter i8000_0A = new terminalParameter
        {
            Include = false,
            Name = "Enc settings - Enable microincrements",
            Transition = "PS",
            Data = "0",
            Index = "8000",
            SubIndex = "0A"
        };
        public terminalParameter i8000_0E = new terminalParameter
        {
            Include = false,
            Name = "Enc settings - Reversion of rotation",
            Transition = "PS",
            Data = "0",
            Index = "8000",
            SubIndex = "0E"
        };
        public terminalParameter i8010_01 = new terminalParameter
        {
            Include = true,
            Name = "Motor Settings - Max current (mA)",
            Transition = "PS",
            Data = "5000",
            Index = "8010",
            SubIndex = "01"
        };
        public terminalParameter i8010_02 = new terminalParameter
        {
            Include = true,
            Name = "Motor Settings - Reduced current (mA)",
            Transition = "PS",
            Data = "2500",
            Index = "8010",
            SubIndex = "02"
        };
        public terminalParameter i8010_03 = new terminalParameter
        {
            Include = true,
            Name = "Motor Settings - Nominal voltage (mV)",
            Transition = "PS",
            Data = "50000",
            Index = "8010",
            SubIndex = "03"
        };
        public terminalParameter i8010_04 = new terminalParameter
        {
            Include = true,
            Name = "Motor Settings - Coil resistance (0.01Ohm)",
            Transition = "PS",
            Data = "100",
            Index = "8010",
            SubIndex = "04"
        };
        public terminalParameter i8010_05 = new terminalParameter
        {
            Include = false,
            Name = "Motor Settings - Motor EMF (###)",
            Transition = "PS",
            Data = "0",
            Index = "8010",
            SubIndex = "05"
        };
        public terminalParameter i8010_06 = new terminalParameter
        {
            Include = true,
            Name = "Motor Settings - Fullsteps",
            Transition = "PS",
            Data = "200",
            Index = "8010",
            SubIndex = "06"
        };
        public terminalParameter i8010_07 = new terminalParameter
        {
            Include = false,
            Name = "Motor Settings - Encoder Increments (4-fold)",
            Transition = "PS",
            Data = "0",
            Index = "8010",
            SubIndex = "07"
        };
        public terminalParameter i90 = new terminalParameter(false, 
            "Test", 
            "PS", 
            "5000", 
            "8011", 
            "01");
        public terminalParameter i8011_01 = new terminalParameter
        (
            Include = false,
            Name = "Controller Settings - Kp factor (curr) (###)",
            Transition = "PS",
            Data = "400",
            Index = "8011",
            SubIndex = "01"
        );
        public terminalParameter i8011_02 = new terminalParameter
        {
            Include = false,
            Name = "Controller Settings - Ki factor (curr) (###)",
            Transition = "PS",
            Data = "4",
            Index = "8011",
            SubIndex = "02"
        };
        public terminalParameter i8011_03 = new terminalParameter
        {
            Include = false,
            Name = "Controller Settings - Inner window (###)",
            Transition = "PS",
            Data = "0",
            Index = "8011",
            SubIndex = "03"
        };
        public terminalParameter i8011_05 = new terminalParameter
        {
            Include = false,
            Name = "Controller Settings - Outer window (###)",
            Transition = "PS",
            Data = "0",
            Index = "8011",
            SubIndex = "05"
        };
        public terminalParameter i8011_06 = new terminalParameter
        {
            Include = false,
            Name = "Controller Settings - Filter cut off frequency (###)",
            Transition = "PS",
            Data = "0",
            Index = "8011",
            SubIndex = "06"
        };
        public terminalParameter i8011_07 = new terminalParameter
        {
            Include = false,
            Name = "Controller Settings - Ka factor (curr) (###)",
            Transition = "PS",
            Data = "100",
            Index = "8011",
            SubIndex = "07"
        };
        public terminalParameter i8011_08 = new terminalParameter
        {
            Include = false,
            Name = "Controller Settings - Kd factor (curr) (###)",
            Transition = "PS",
            Data = "100",
            Index = "8011",
            SubIndex = "08"
        };
        public terminalParameter i8012_05 = new terminalParameter
        {
            Include = true,
            Name = "Features - Speed Range (0:1k, 1:2k, 2:4k, 3:8k, 4:16k, 5:32k)",
            Transition = "PS",
            Data = "1",
            Index = "8012",
            SubIndex = "05"
        };
        public terminalParameter i8012_08 = new terminalParameter
        {
            Include = true,
            Name = "Feedback type (0: encoder, 1: internal counter)",
            Transition = "PS",
            Data = "1",
            Index = "8012",
            SubIndex = "08"
        };
        public terminalParameter i8012_09 = new terminalParameter
        {
            Include = true,
            Name = "Invert motor polarity",
            Transition = "PS",
            Data = "0",
            Index = "8012",
            SubIndex = "09"
        };
*/
    }

    public class terminalParameter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _include = false;
        private string _name;
        private string _transition;
        private string _data;
        private string _index;
        private string _subindex;
        private string _ccs = "1";
        private string _timeout = "0";
        public terminalParameter(bool Include, string Name, string Transition, string Data, string Index, string SubIndex, string CCS = "1", string timeout = "0")
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
            //set { _include = value; }
            set
            {
                if (value != _include)
                {
                    _include = value;
                    OnPropertyChanged();
                }
              
            }
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
