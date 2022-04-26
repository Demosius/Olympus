﻿using Styx;
using Uranus;

namespace Prometheus.View.Pages.Users;

/// <summary>
/// Interaction logic for UserActivatePage.xaml
/// </summary>
public partial class UserActivatePage
{
    public UserActivatePage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}