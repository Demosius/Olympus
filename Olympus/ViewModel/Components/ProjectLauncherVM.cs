﻿using Olympus.Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympus.Uranus;
using System.Collections.ObjectModel;

namespace Olympus.ViewModel.Components
{
    public class ProjectLauncherVM : INotifyPropertyChanged
    {
        public OlympusVM OlympusVM { get; set; }

        public List<Department> Departments { get; set; }
        public List<Project> AllProjects { get; set; }
        public List<Project> UserProjects { get; set; }

        private ObservableCollection<ProjectGroupVM> projectGroups;
        public ObservableCollection<ProjectGroupVM> ProjectGroups
        {
            get => projectGroups;
            set
            {
                projectGroups = value;
                OnPropertyChanged(nameof(ProjectGroups));
            }
        }

        public ProjectLauncherVM()
        {
            List<Department> deps = App.Helios.StaffReader.Departments(pullType: PullType.IncludeChildren);
            Departments = deps;
            AllProjects = App.Helios.StaffReader.Projects(pullType: PullType.FullRecursive);
            if (App.Charon.UserEmployee is null)
                UserProjects = new List<Project>();
            else
                UserProjects = App.Charon.UserEmployee.Projects;

            ProjectGroupVM projectGroup;

            ProjectGroups = new ObservableCollection<ProjectGroupVM>();
            
            projectGroup = new ProjectGroupVM(this, AllProjects, "All");
            ProjectGroups.Add(projectGroup);
            projectGroup = new ProjectGroupVM(this, UserProjects, "User");
            ProjectGroups.Add(projectGroup);

            foreach (var dep in Departments)
            {
                projectGroup = new ProjectGroupVM(this, dep);
                ProjectGroups.Add(projectGroup);
            }
        }

        public ProjectLauncherVM(OlympusVM olympusVM) : this()
        {
            OlympusVM = olympusVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
