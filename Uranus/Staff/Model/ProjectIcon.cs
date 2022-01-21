using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    public class ProjectIcon : Image
    {
        [ForeignKey(typeof(Project))]
        public string ProjectName { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Project Project { get; set; }

        public ProjectIcon() { }

        // Creation of a new project icon with an already specified project.
        public ProjectIcon(Project project, string iconFileName)
        {
            Project = project;
            FileName = iconFileName;
            ProjectName = project.Name;
            Name = ProjectName.ToString();
        }

        public void SetImageFilePath(StaffReader reader)
        {
            FullPath = GetImageFilePath(reader);
        }

        public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.ProjectIconDirectory, FileName);

    }
}
