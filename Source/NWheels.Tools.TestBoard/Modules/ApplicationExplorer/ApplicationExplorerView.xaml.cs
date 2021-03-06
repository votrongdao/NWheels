﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NWheels.Tools.TestBoard.Extensions;

namespace NWheels.Tools.TestBoard.Modules.ApplicationExplorer
{
    /// <summary>
    /// Interaction logic for ApplicationExplorerView.xaml
    /// </summary>
    public partial class ApplicationExplorerView : UserControl
    {
        public ApplicationExplorerView()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = ((DependencyObject)e.OriginalSource).VisualUpwardSearch<TreeViewItem>();

            if ( treeViewItem != null )
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
    }
}
