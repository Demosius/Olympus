﻿using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Models;

public class ProjectIcon : Image
{
    [OneToMany(nameof(Project.IconName), nameof(Project.Icon), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Project> Projects { get; set; }

    public ProjectIcon()
    {
        Projects = new List<Project>();
    }

    public ProjectIcon(Image image) : this()
    {
        Name = image.Name;
        FileName = image.FileName;
    }

    // Creation of a new project icon with an already specified project.
    public ProjectIcon(Project project, string iconFileName)
    {
        Projects = new List<Project> { project };
        FileName = iconFileName;
        Name = project.Name;
    }

    public ProjectIcon(List<Project> projects)
    {
        Projects = projects;
    }

    public ProjectIcon(string fullPath, string name, string fileName, List<Project> projects) : base(fullPath, name, fileName)
    {
        Projects = projects;
    }

    /*public void SetImageFilePath(StaffReader reader)
    {
        FullPath = GetImageFilePath(reader);
    }*/

    //public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.ProjectIconDirectory, FileName);

}