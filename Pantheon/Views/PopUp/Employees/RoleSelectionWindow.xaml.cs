﻿using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for RoleSelectionWindow.xaml
/// </summary>
public partial class RoleSelectionWindow
{
    public RoleSelectionVM VM { get; set; }

    public RoleSelectionWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new RoleSelectionVM(helios, charon);
        DataContext = VM;
    }
}