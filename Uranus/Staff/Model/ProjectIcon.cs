using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.IO;

namespace Uranus.Staff.Model;

public class ProjectIcon : Image
{
    [OneToMany(nameof(Project.IconName), nameof(Project.Icon), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Project> Projects { get; set; }

    public ProjectIcon() { }

    // Creation of a new project icon with an already specified project.
    public ProjectIcon(Project project, string iconFileName)
    {
        Projects = new List<Project> { project };
        FileName = iconFileName;
        Name = project.Name;
    }

    public void SetImageFilePath(StaffReader reader)
    {
        FullPath = GetImageFilePath(reader);
    }

    public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.ProjectIconDirectory, FileName);

}