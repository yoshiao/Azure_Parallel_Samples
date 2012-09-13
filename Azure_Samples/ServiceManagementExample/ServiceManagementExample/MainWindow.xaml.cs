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

namespace ServiceManagementExample
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

        private void CreateDeployment_Click(object sender, RoutedEventArgs e)
        {
            CreateDeploymentExample.UseCreateDeploymentExample();
        }

        private void CreateService_Click(object sender, RoutedEventArgs e)
        {
            CreateHostedServiceExample.UseCreateHostedServiceExample();
        }

        private void GetProperties_Click(object sender, RoutedEventArgs e)
        {
            GetHostedServicePropertiesExample.UseGetHostedServicePropertiesExample();
        }

        private void UpgradeDeployment_Click(object sender, RoutedEventArgs e)
        {
            UpgradeDeploymentExample.UseUpgradeDeploymentExample();
        }
    }
}
