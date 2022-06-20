using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestP
{
    /// <summary>
    /// Interaction logic for ViewPalletPatern.xaml
    /// </summary>
    public partial class ViewPalletPatern : Window
    {
        public ViewPalletPatern()
        {
            InitializeComponent();
        }
        private void Button_Add_Box_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Handling Add box click
            ViewAddNewBox viewAddNewBox = new ViewAddNewBox();
            viewAddNewBox.Show();


        }

        private void Button_Add_Pallet_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Handling Add pallet click
        }



        

        private void Button_Calculate_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Handling calculate stuff
            MessageBox.Show("The button calculate has been clicked", "Information Message");
        }
    }
}
