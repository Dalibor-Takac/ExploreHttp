﻿using ExploreHttp.Models;
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

namespace ExploreHttp;
/// <summary>
/// Interaction logic for LogWindow.xaml
/// </summary>
public partial class LogWindow : Window
{
    public LogWindow()
    {
        InitializeComponent();
    }

    public static void OpenDialog(Window parent, RequestModel requestModel)
    {
        var dlg = new LogWindow();
        dlg.DataContext = requestModel;
        dlg.Owner = parent;
        dlg.ShowDialog();
    }

    private void Close_Click(object sender, ContextMenuEventArgs e)
    {
        this.DialogResult = true;
        Close();
    }
}
