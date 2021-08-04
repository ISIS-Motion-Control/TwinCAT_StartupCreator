using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TwinCAT___Startup_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class EL7041 : Page, INotifyPropertyChanged
    {
        private async void messageBoxPopup(string text2show)
        {
            var messageDialog = new MessageDialog(text2show);
            await messageDialog.ShowAsync();
        }

        private TerminalEL7041 _terminalEL7041 = new TerminalEL7041();
        public TerminalEL7041 TerminalEL7041
        {
            get { return _terminalEL7041; }
            set { _terminalEL7041 = value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }


        public bool TestBool { get; set; }

        
        
        
        
        string testString;
        Binding testBind = new Binding();
        Binding testBind2 = new Binding();

        
        public EL7041()
        {
            InitializeComponent();
            this.DataContext = this;
            int iRow = 4;

            testString = ((terminalParameter)TerminalEL7041.parameter(0)).Data;
            //testBind.Source = ((terminalParameter)(((EL7041)DataContext).TerminalEL7041.parameter(0))).Include;
            testBind.Source = this;
            testBind.Path = new PropertyPath("TestBool");
            testBind.Mode = BindingMode.TwoWay;
            testBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //include1.SetBinding(ToggleSwitch.IsOnProperty, testBind);


            testBind2.Source = TerminalEL7041.parameter(0);
            //testBind2.Path = new PropertyPath("((terminalParameter)TerminalEL7041.parameter[0]).Include");
            testBind2.Path = new PropertyPath("Include");
            testBind2.Mode = BindingMode.TwoWay;
            testBind2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            include1.SetBinding(ToggleSwitch.IsOnProperty, testBind2);
            
            //testBool = ((terminalParameter)TerminalEL7041.parameter(0)).Include;
            
            
            List<ToggleSwitch> toggleSwitches = new List<ToggleSwitch>();
            
            foreach (terminalParameter parameter in TerminalEL7041)
            {
                ToggleSwitch toggleSwitch = new ToggleSwitch();
                toggleSwitch.SetValue(Grid.ColumnProperty, 0);
                toggleSwitch.SetValue(Grid.RowProperty, iRow);
                //toggleSwitch.Toggled += new RoutedEventHandler(includeToggle1_Toggled);               
               
                Binding includeBind = new Binding();
                includeBind.Mode = BindingMode.TwoWay;
                includeBind.Source = parameter;
                includeBind.Path = new PropertyPath("Include");
                includeBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(toggleSwitch, ToggleSwitch.IsOnProperty, includeBind);
                
                iRow++;
                grid.Children.Add(toggleSwitch);
                toggleSwitches.Add(toggleSwitch);
            }
            
        }

        private void includeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
            }
        }
        private void includeToggle1_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                messageBoxPopup(((terminalParameter)TerminalEL7041.parameter(0)).Include.ToString());
                //messageBoxPopup(((terminalParameter)TerminalEL7041.parameter(0)).Include.ToString());
            }
        }


        private void include1_Toggled(object sender, RoutedEventArgs e)
        {
            //terminalEL7041.i8000_08.Include = include1.IsOn;
        }

        private void data1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //terminalEL7041.i8000_08.Data = data1.Text;
        }
        private void data2_TextChanged(object sender, TextChangedEventArgs e)
        {
            //terminalEL7041.i8000_0A.Data = data2.Text;
        }
        private void data3_TextChanged(object sender, TextChangedEventArgs e)
        {
            //terminalEL7041.i8000_0E.Data = data3.Text;
        }
    }

}
