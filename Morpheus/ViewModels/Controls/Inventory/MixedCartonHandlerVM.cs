﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Morpheus.ViewModels.Commands;
using Morpheus.Views.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Morpheus.ViewModels.Controls.Inventory;

public class MixedCartonHandlerVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    public List<MixedCartonItem> MixedCartonItems { get; set; }
    public Dictionary<int, NAVItem> Items { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<MixedCarton> MixedCartons { get; set; }

    public ObservableCollection<MixedCartonItem> MCItems { get; set; }

    private MixedCarton? selectedMixedCarton;
    public MixedCarton? SelectedMixedCarton
    {
        get => selectedMixedCarton;
        set
        {
            selectedMixedCarton = value;
            OnPropertyChanged();
            SetItems();
        }
    }

    private MixedCartonItem? selectedMixCtnItem;
    public MixedCartonItem? SelectedMixCtnItem
    {
        get => selectedMixCtnItem;
        set
        {
            selectedMixCtnItem = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public SaveMixedCartonsCommand SaveMixedCartonsCommand { get; set; }
    public AutoGenerateMixedCartonsCommand AutoGenerateMixedCartonsCommand { get; set; }
    public AddMixedCartonCommand AddMixedCartonCommand { get; set; }
    public DeleteMixedCartonCommand DeleteMixedCartonCommand { get; set; }
    public AddMixCtnItemCommand AddMixCtnItemCommand { get; set; }
    public DeleteMixCtnItemCommand DeleteMixCtnItemCommand { get; set; }

    #endregion


    public MixedCartonHandlerVM(Helios helios)
    {
        Helios = helios;

        MixedCartons = new ObservableCollection<MixedCarton>();
        MCItems = new ObservableCollection<MixedCartonItem>();
        Items = new Dictionary<int, NAVItem>();
        MixedCartonItems = new List<MixedCartonItem>();

        RefreshDataCommand = new RefreshDataCommand(this);
        SaveMixedCartonsCommand = new SaveMixedCartonsCommand(this);
        AutoGenerateMixedCartonsCommand = new AutoGenerateMixedCartonsCommand(this);
        AddMixedCartonCommand = new AddMixedCartonCommand(this);
        DeleteMixedCartonCommand = new DeleteMixedCartonCommand(this);
        AddMixCtnItemCommand = new AddMixCtnItemCommand(this);
        DeleteMixCtnItemCommand = new DeleteMixCtnItemCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        MixedCartons.Clear();

        // Get existing Mixed carton data.
        var (mixedCartons, mcItems, items) = await Helios.InventoryReader.GetMixedCartonDataAsync();

        Items = items.ToDictionary(i => i.Number, i => i);

        var mcDict = mixedCartons.ToDictionary(mc => mc.ID, mc => mc);

        foreach (var mixedCartonItem in mcItems)
        {
            if (Items.TryGetValue(mixedCartonItem.ItemNumber, out var item))
            {
                mixedCartonItem.Item = item;
                item.MixedCartons.Add(mixedCartonItem);
            }

            if (!mcDict.TryGetValue(mixedCartonItem.MixedCartonID, out var mixedCarton)) continue;

            mixedCartonItem.MixedCarton = mixedCarton;
            mixedCarton.Items.Add(mixedCartonItem);
        }

        foreach (var mixedCarton in mixedCartons) MixedCartons.Add(mixedCarton);

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    /// <summary>
    /// Set MC Items based on the selected Mixed Carton.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void SetItems()
    {
        MCItems.Clear();

        if (SelectedMixedCarton is null) return;

        foreach (var cartonItem in SelectedMixedCarton.Items)
        {
            MCItems.Add(cartonItem);
        }
    }

    public async Task SaveMixedCartons()
    {
        // Confirm with user.
        if (MessageBox.Show("Are you sure you want to save the changes made to the Mixed Cartons?",
                "Confirm Changes", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        // Update/Replace table.
        await Helios.InventoryUpdater.ReplaceMixedCartonsAsync(MixedCartons);
    }

    public async Task AutoGenerateMixedCartons()
    {
        var dataTask = !Items.Any() && !MixedCartons.Any() ? RefreshDataAsync() : null;

        // Get zone(s) from user.
        var zoneInput = new InputWindow("Enter Zones (separated with pipe '|' character) to check.", "Zones");

        if (dataTask is not null) await dataTask;

        if (zoneInput.ShowDialog() != true) return;

        Mouse.OverrideCursor = Cursors.Wait;

        var zoneString = zoneInput.Input.Text;

        var zones = zoneString.Split('|');

        var stockTask = Helios.InventoryReader
                .NAVAllStockAsync(s => zones.Contains(s.ZoneCode) && s.LocationCode == "9600");

        var existingSignatures = MixedCartons.Select(mc => mc.Signature).ToList();

        var count = 0;

        var stock = (await stockTask)
            .Where(s => s.Qty > 0)
            .GroupBy(s => s.BinID)
            .Where(g => g.Count() > 1)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.ToList());

        // Set item objects against stock.
        foreach (var navStock in stock.SelectMany(s => s.Value))
            if (Items.TryGetValue(navStock.ItemNumber, out var item)) navStock.Item = item;


        foreach (var (_, stockList) in stock)
        {
            var mc = MixedCarton.GetMixedCarton(stockList);

            if (mc is null || existingSignatures.Contains(mc.Signature)) continue;

            existingSignatures.Add(mc.Signature);
            MixedCartons.Add(mc);
            foreach (var mixedCartonItem in mc.Items)
            {
                MixedCartonItems.Add(mixedCartonItem);
            }

            count++;
        }

        Mouse.OverrideCursor = Cursors.Arrow;

        MessageBox.Show($"{count} new Mixed Carton templates added.", "Automated Check Complete", MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public void AddMixedCarton()
    {
        var mc = new MixedCarton();

        MixedCartons.Add(mc);

        SelectedMixedCarton = mc;
    }

    public void DeleteMixedCarton()
    {
        if (SelectedMixedCarton is null) return;

        if (MessageBox.Show($"Are you sure you want to delete the {SelectedMixedCarton.Name} Mixed Carton Template?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        foreach (var mixedCartonItem in SelectedMixedCarton.Items) MixedCartonItems.Remove(mixedCartonItem);
        MixedCartons.Remove(SelectedMixedCarton);
        MCItems.Clear();

        SelectedMixedCarton = null;
    }

    public void AddMixCtnItem()
    {
        if (SelectedMixedCarton is null) return;

        var itemSelection = new ItemSelectionWindow(Items.Values.ToList(), SelectionMode.Extended);
        if (itemSelection.ShowDialog() != true) return;

        var items = itemSelection.Items;
        if (items.Count <= 0) return;

        foreach (var mci in items.Select(item => new MixedCartonItem(SelectedMixedCarton, item)))
            MCItems.Add(mci);
    }

    public void DeleteMixCtnItem()
    {
        if (SelectedMixCtnItem is null) return;

        SelectedMixCtnItem.Remove();
        MixedCartonItems.Remove(SelectedMixCtnItem);
        MCItems.Remove(SelectedMixCtnItem);

        SelectedMixCtnItem = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}