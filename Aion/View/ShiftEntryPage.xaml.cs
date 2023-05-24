﻿using Styx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Aion.ViewModels;
using Uranus;

namespace Aion.View;

/// <summary>
/// Interaction logic for EntryEditPage.xaml
/// </summary>
public partial class ShiftEntryPage
{
    public ShiftEntryPageVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ShiftEntryPage(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void ShiftEntryPage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await ShiftEntryPageVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }

    private readonly Style centerStyle = new()
    {
        TargetType = typeof(TextBlock),
        Setters =
        {
            new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
        }
    };

    private void Entries_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var headerName = e.Column.Header.ToString() ?? string.Empty;

        //Cancel the column you don't want to generate
        if (new List<string> { "ID", "ShiftName", "ClockEvents", "Employee" }.Contains(headerName))
        {
            e.Cancel = true;
        }

        switch (headerName)
        {
            //update column details when generating
            case "EmployeeID":
                e.Column.Header = "Associate Number";
                e.Column.IsReadOnly = true;
                break;
            case "EmployeeName":
                e.Column.Header = "Name";
                e.Column.Width = 150;
                e.Column.IsReadOnly = true;
                break;
            case "Location":
                try
                {
                    DataGridComboBoxColumn col = new()
                    {
                        Header = "Location",
                        ItemsSource = VM?.Locations,
                        SelectedValueBinding = new Binding("Location")
                    };

                    e.Column = col;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected Exception in Entry Column Auto-generate:\n\n{ex}",
                        "Unexpected Exception");
                }

                break;
            case "ShiftStartTime":
                e.Column.Header = "In";
                e.Column.Width = 80;
                break;
            case "LunchStartTime":
                e.Column.Header = "Out (Lunch)";
                e.Column.Width = 80;
                break;
            case "LunchEndTime":
                e.Column.Header = "In (Lunch)";
                e.Column.Width = 80;
                break;
            case "ShiftEndTime":
                e.Column.Header = "Out";
                e.Column.Width = 80;
                break;
            case "ShiftType":
                e.Column.Width = 80;
                e.Column.Header = "Shift\n(D/M/A)";
                break;
            case "TimeTotal":
                e.Column.Width = 80;
                e.Column.Header = "Total";
                ((DataGridTextColumn)e.Column).Binding.StringFormat = "HH:mm";
                break;
            case "HoursWorked":
                e.Column.Width = 80;
                e.Column.Header = "Time Worked";
                ((DataGridTextColumn)e.Column).Binding.StringFormat = "0.00";
                break;
            case "Comments":
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                break;
            case "Department":
                e.Column.Width = 80;
                e.Column.IsReadOnly = true;
                break;
            case "Day" or "Date":
                e.Column.Width = 100;
                e.Column.IsReadOnly = true;
                break;
            default:
                e.Column.Width = 80;
                break;
        }

        if (e.Column is DataGridTextColumn column && headerName != "Comments") column.ElementStyle = centerStyle;
    }

    private void Entries_AutoGeneratedColumns(object sender, EventArgs e)
    {
        var grid = (DataGrid)sender;

        grid.Columns.First(c => c.Header.ToString() == "Associate Number").DisplayIndex = 0;
        grid.Columns.First(c => c.Header.ToString() == "Name").DisplayIndex = 1;
        grid.Columns.First(c => c.Header.ToString() == "Location").DisplayIndex = 2;
        grid.Columns.First(c => c.Header.ToString() == "Date").DisplayIndex = 3;
        grid.Columns.First(c => c.Header.ToString() == "Day").DisplayIndex = 4;
        grid.Columns.First(c => c.Header.ToString() == "In").DisplayIndex = 5;
        grid.Columns.First(c => c.Header.ToString() == "Out (Lunch)").DisplayIndex = 6;
        grid.Columns.First(c => c.Header.ToString() == "In (Lunch)").DisplayIndex = 7;
        grid.Columns.First(c => c.Header.ToString() == "Out").DisplayIndex = 8;
        grid.Columns.First(c => c.Header.ToString() == "Shift\n(D/M/A)").DisplayIndex = 9;
        grid.Columns.First(c => c.Header.ToString() == "Total").DisplayIndex = 10;
        grid.Columns.First(c => c.Header.ToString() == "Time Worked").DisplayIndex = 11;
        grid.Columns.First(c => c.Header.ToString() == "Comments").DisplayIndex = 12;
        grid.Columns.First(c => c.Header.ToString() == "Department").DisplayIndex = 13;

    }
}