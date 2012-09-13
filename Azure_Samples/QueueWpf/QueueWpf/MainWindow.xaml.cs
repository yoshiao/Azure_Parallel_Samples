using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QueueWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateQueue_Click(object sender, RoutedEventArgs e)
        {
            CreateQueueExample.UseCreateQueueExample();
        }

        private void AddMessages_Click(object sender, RoutedEventArgs e)
        {
            AddMessagesExample.UseAddMessagesExample();
        }

        private void GetMessages_Click(object sender, RoutedEventArgs e)
        {
            GetMessagesExample.UseGetMessagesExample();
        }

        private void PoisonMessages_Click(object sender, RoutedEventArgs e)
        {
            PoisonMessagesExample.UsePoisonMessagesExample();
        }

        private void Backoff_Click(object sender, RoutedEventArgs e)
        {
            BackoffExample.UseBackoffExample();
        }

        private void LargeData_Click(object sender, RoutedEventArgs e)
        {
            LargeDataExample.UseLargeDataExample();
        }

    }
}
