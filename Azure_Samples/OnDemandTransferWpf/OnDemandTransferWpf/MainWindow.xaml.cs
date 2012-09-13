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

namespace OnDemandTransferWpf
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

        private void RequestTransfer_Click(object sender, RoutedEventArgs e)
        {
            String deploymentId = DeploymentId.Text;
            String roleName = RoleName.Text;
            String roleInstanceId = InstanceId.Text;

            OnDemandTransferExample example = new OnDemandTransferExample();
            example.RequestOnDemandTransfer(deploymentId, roleName, roleInstanceId);
        }

        private void CleanupTransfers_Click(object sender, RoutedEventArgs e)
        {
            OnDemandTransferExample example = new OnDemandTransferExample();
            example.CleanupOnDemandTransfers();
        }
    }
}
