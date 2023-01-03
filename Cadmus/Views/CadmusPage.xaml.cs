using System;
using System.Windows;
using System.Windows.Controls;
using Cadmus.ViewModels;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Cadmus.Views;

/// <summary>
/// Interaction logic for CadmusPage.xaml
/// </summary>
public partial class CadmusPage : IProject
{
    public CadmusPage()
    {
        InitializeComponent();
        DataContext = new CadmusVM(App.Helios);
    }

    public EProject Project => EProject.Cadmus;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var printDialog = new PrintDialog();

        if (printDialog.ShowDialog() != true) return;

        string str = "\nN\nq812Q1218,20\n";
        str += Printing.SendImageToPrinter(50, 50, System.IO.Path.GetDirectoryName
            (System.Reflection.Assembly.GetEntryAssembly()?.Location) + "./printer.png");
        str += "B200,150,0,K,3,7,150,N,\"A123456789A\"\n";
        str += "A340,310,0,4,1,1,N,\"123456789\"\n";
        str += "X100,400,1,730,1100\n";
        str += "A110,560,0,3,1,1,N,\"Contact\"\n";
        str += "A260,560,0,3,1,1,N,\"YOUR NAME\"\n";
        str += "A110,590,0,3,1,1,N,\"Department\"\n";
        str += "A260,590,0,3,1,1,N,\"YOUR DEPARTMENT\"\n";
        str += "A110,620,0,3,1,1,N,\"Company\"\n";
        str += "A260,620,0,3,1,1,N,\"YOUR COMPANY\"\n";
        str += "A110,650,0,3,1,1,N,\"Address\"\n";
        str += "A260,650,0,3,1,1,N,\"YOUR ADDRESS1\"\n";
        str += "A260,680,0,3,1,1,N,\"YOUR ADDRESS2\"\n";
        str += "A260,710,0,3,1,1,N,\"YOUR ADDRESS3\"\n";
        str += "A110,740,0,3,1,1,N,\"City\"\n";
        str += "A260,740,0,3,1,1,N,\"YOUR CITY\"\n";
        str += "A110,770,0,3,1,1,N,\"State\"\n";
        str += "A260,770,0,3,1,1,N,\"YOUR STATE\"\n";
        str += "A110,800,0,3,1,1,N,\"Country\"\n";
        str += "A260,800,0,3,1,1,N,\"YOUR COUNTRY\"\n";
        str += "A110,830,0,3,1,1,N,\"Post code\"\n";
        str += "A260,830,0,3,1,1,N,\"YOUR POSTCODE\"\n";
        str += "A110,860,0,3,1,1,N,\"Phone No\"\n";
        str += "A260,860,0,3,1,1,N,\"YOUR PHONE\"\n";
        str += "A110,890,0,3,1,1,N,\"Email\"\n";
        str += "A260,890,0,3,1,1,N,\"YOUR EMAIL\"\n";
        str += "P1\n";
        RawPrinterHelper.SendStringToPrinter(printDialog.PrintQueue.FullName, str);
    }
}