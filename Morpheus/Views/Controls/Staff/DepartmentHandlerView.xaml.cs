using System;
using Morpheus.ViewModels.Controls.Staff;
using Styx;
using Uranus;

namespace Morpheus.Views.Controls.Staff
{
    /// <summary>
    /// Interaction logic for DepartmentHandlerView.xaml
    /// </summary>
    public partial class DepartmentHandlerView
    {
        public DepartmentHandlerVM VM { get; set; }
        public Helios Helios { get; set; }
        public Charon Charon { get; set; }

        public DepartmentHandlerView(Helios helios, Charon charon)
        {
            Helios = helios;
            Charon = charon;
            VM = DepartmentHandlerVM.CreateEmpty(Helios, Charon);
            InitializeComponent();
            DataContext = VM;
        }

        private async void DepartmentHandlerView_OnInitialized(object? sender, EventArgs e)
        {
            VM = await DepartmentHandlerVM.CreateAsync(Helios, Charon);
            DataContext = VM;
        }
    }
}
