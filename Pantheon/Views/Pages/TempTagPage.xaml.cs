﻿using System;
using Pantheon.ViewModels.Pages;
using Styx;
using Uranus;

namespace Pantheon.Views.Pages;

/// <summary>
/// Interaction logic for TempTagPage.xaml
/// </summary>
public partial class TempTagPage
{
    public TempTagPageVM VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public TempTagPage(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        VM = TempTagPageVM.CreateEmpty(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }

    private async void TempTagPage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await TempTagPageVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}