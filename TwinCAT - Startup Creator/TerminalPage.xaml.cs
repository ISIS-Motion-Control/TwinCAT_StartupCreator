using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TwinCAT___Startup_Creator
{
    /// <summary>
    /// A generic page for generating a parameter list and user interface for a given terminal
    /// </summary>
    public partial class TerminalPage : Page
    {
        //List of transitions created as source for combobox elements in display
        readonly ObservableCollection<string> ListOfTransitions = new ObservableCollection<string>() { "IP", "PS", "SP", "SO", "OS" };
        
        private GenericTerminal _terminal;
        /// <summary>
        /// Method for accessing the terminal parameter data set by the user
        /// </summary>
        public GenericTerminal Terminal
        {
            get { return _terminal; }
            set { _terminal = value; }
        }

        /// <summary>
        /// Class constructor. Requires an input of the terminal type to display parameters for
        /// </summary>
        /// <param name="inputTerminal"></param>
        public TerminalPage(GenericTerminal inputTerminal)
        {
            this.InitializeComponent();
            Terminal = inputTerminal; 
            populatePage();
        }

        
        /// <summary>
        /// Method for populating the page with XAML elements for user interaction.
        /// Uses the local Terminal value
        /// </summary>
        public void populatePage()
        {
            int iRow = 1;
            foreach (terminalParameter parameter in Terminal)
            {
                Binding includeBind = new Binding();
                Binding commentBind = new Binding();
                Binding dataBind = new Binding();
                Binding transitionBind = new Binding();

                includeBind.Mode = BindingMode.TwoWay;
                includeBind.Source = parameter;
                includeBind.Path = new PropertyPath("Include");
                includeBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                commentBind.Mode = BindingMode.OneWay;
                commentBind.Source = parameter;
                commentBind.Path = new PropertyPath("Name");

                dataBind.Mode = BindingMode.TwoWay;
                dataBind.Source = parameter;
                dataBind.Path = new PropertyPath("Data");
                dataBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                transitionBind.Mode = BindingMode.TwoWay;
                transitionBind.Source = parameter;
                transitionBind.Path = new PropertyPath("Transition");
                transitionBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                //create Include parameter toggles
                ToggleSwitch toggleSwitch = new ToggleSwitch();
                toggleSwitch.SetValue(Grid.ColumnProperty, 0);
                toggleSwitch.SetValue(Grid.RowProperty, iRow);
                toggleSwitch.Margin = new Thickness(5, 0, 0, 0);
                toggleSwitch.OffContent = string.Empty;
                toggleSwitch.OnContent = string.Empty;

                //Create textblock for parameter name/comment
                TextBlock textBlock = new TextBlock();
                textBlock.SetValue(Grid.ColumnProperty, 1);
                textBlock.SetValue(Grid.RowProperty, iRow);
                textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Margin = new Thickness(25, 0, 0, 0);

                //Create textbox for data manipulation
                TextBox textBox = new TextBox();
                textBox.SetValue(Grid.ColumnProperty, 2);
                textBox.SetValue(Grid.RowProperty, iRow);
                textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBox.VerticalAlignment = VerticalAlignment.Center;

                //Create combobox for transition
                ComboBox comboBox = new ComboBox();
                comboBox.SetValue(Grid.ColumnProperty, 3);
                comboBox.SetValue(Grid.RowProperty, iRow);
                comboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                comboBox.VerticalAlignment = VerticalAlignment.Center;
                comboBox.Margin = new Thickness(5, 5, 5, 5);
                comboBox.ItemsSource = ListOfTransitions;

                //Set data binds
                BindingOperations.SetBinding(toggleSwitch, ToggleSwitch.IsOnProperty, includeBind);
                BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, commentBind);
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, dataBind);
                BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, transitionBind);

                iRow++;
                grid.Children.Add(toggleSwitch);
                grid.Children.Add(textBlock);
                grid.Children.Add(textBox);
                grid.Children.Add(comboBox);
            }
        }

    }
}
