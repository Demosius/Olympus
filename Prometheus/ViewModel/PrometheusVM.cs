using Prometheus.Model;
using Prometheus.View;
using Prometheus.View.Pages;
using Prometheus.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProEnums = Prometheus.ViewModel.Helpers.EnumConverter;

namespace Prometheus.ViewModel
{
    class PrometheusVM : INotifyPropertyChanged
    {
        public ObservableCollection<DataCategory> Categories { get; set; }
        public ObservableCollection<DataType> Types { get; set; }
        public Dictionary<EDataType, BREADBase> DataPages { get; set; }

        private BREADBase currentPage;
        public BREADBase CurrentPage 
        {
            get => currentPage; 
            set
            {
                currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            } 
        }

        private DataCategory selectedCategory;
        public DataCategory SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                SelectedType = null;
                SetTypes();
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private DataType selectedType;
        public DataType SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                if (!(value is null)) SetPage(value.Type);
                OnPropertyChanged(nameof(SelectedType));
            }
        }

        public PrometheusVM()
        {
            Categories = new ObservableCollection<DataCategory>();
            Types = new ObservableCollection<DataType>();
            DataPages = new Dictionary<EDataType, BREADBase>();
            foreach (var category in ProEnums.GetDataCategories())
            {
                Categories.Add(new DataCategory(category));
            }
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

        public void SetPage(EDataType type)
        {
            CurrentPage = GetPage(type);
        }

        public BREADBase GetPage(EDataType type)
        {
            // If it already exists in the given pages, use it.
            if (DataPages.ContainsKey(type))
                return DataPages[type];
            if (type == EDataType.Batch)
                return GeneratePage(new BatchView());
            if (type == EDataType.NAVBin)
                return GeneratePage(new BinView());

            // If nothing else works, return null.
            return null;
        }

        public BREADBase GeneratePage(BREADBase page)
        {
            DataPages.Add(page.DataType, page);
            return page;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
