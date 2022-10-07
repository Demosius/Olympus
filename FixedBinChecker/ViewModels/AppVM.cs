﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FixedBinChecker.Views;
using Uranus;
using Uranus.Annotations;

namespace FixedBinChecker.ViewModels;

internal class AppVM : INotifyPropertyChanged
{
    public Helios? Helios { get; set; }

    private FixedBinCheckerPage? fixedBinCheckerPage;
    public FixedBinCheckerPage? FixedBinCheckerPage
    {
        get => fixedBinCheckerPage;
        set
        {
            fixedBinCheckerPage = value;
            OnPropertyChanged(nameof(FixedBinCheckerPage));
        }
    }

    public AppVM()
    {
        FixedBinCheckerPage = null;
    }

    public void SetDataSources(Helios helios)
    {
        Helios = helios;
        FixedBinCheckerPage = new FixedBinCheckerPage(helios);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}