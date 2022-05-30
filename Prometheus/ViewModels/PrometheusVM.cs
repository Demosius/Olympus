using Prometheus.Model;
using Prometheus.ViewModels.Helpers;
using Prometheus.Views;
using Prometheus.Views.Pages;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Annotations;
using ProEnums = Prometheus.ViewModels.Helpers.EnumConverter;

namespace Prometheus.ViewModels;

public class PrometheusVM : INotifyPropertyChanged
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public ObservableCollection<DataCategory> Categories { get; set; }
    public ObservableCollection<DataType> Types { get; set; }
    public Dictionary<EDataCategory, CatPage> Pages { get; set; }

    private CatPage? currentPage;
    public CatPage? CurrentPage
    {
        get => currentPage;
        set
        {
            currentPage = value;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }

    private DataCategory? selectedCategory;
    public DataCategory? SelectedCategory
    {
        get => selectedCategory;
        set
        {
            selectedCategory = value;
            SelectedType = null;
            if (value is not null) SetPage(value.Category);
            OnPropertyChanged(nameof(SelectedCategory));
        }
    }

    private DataType? selectedType;
    public DataType? SelectedType
    {
        get => selectedType;
        set
        {
            selectedType = value;
            OnPropertyChanged(nameof(SelectedType));
        }
    }

    public PrometheusVM()
    {
        Categories = new ObservableCollection<DataCategory>();
        Types = new ObservableCollection<DataType>();
        Pages = new Dictionary<EDataCategory, CatPage>();
        foreach (var category in ProEnums.GetDataCategories())
        {
            Categories.Add(new DataCategory(category));
        }
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
    }

    public void SetTypes()
    {
        Types.Clear();
        if (selectedCategory is null) return;
        foreach (var type in selectedCategory.DataTypes)
        {
            Types.Add(type);
        }
    }

    public void SetPage(EDataCategory category)
    {
        CurrentPage = GetPage(category);
    }

    public CatPage? GetPage(EDataCategory category)
    {
        if (Helios is null || Charon is null) return null;

        // If it already exists in the given pages, use it.
        if (Pages.ContainsKey(category))
            return Pages[category];

        return category switch
        {
            EDataCategory.Inventory => GeneratePage(new InventoryPage()),
            EDataCategory.Equipment => GeneratePage(new EquipmentPage()),
            EDataCategory.Staff => GeneratePage(new StaffPage()),
            EDataCategory.Users => GeneratePage(new UserPage(Helios, Charon)),
            _ => null
        };
    }

    public CatPage GeneratePage(CatPage page)
    {
        Pages.Add(page.DataCategory, page);
        return page;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}