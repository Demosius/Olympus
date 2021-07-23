using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class ProjectIcon : Image
    {
        [ForeignKey(typeof(Project))]
        public string ProjectName { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Project Project { get; set; }

        public override void GetImageFilePath() => Path.Combine(App.Charioteer.StaffReader.ProjectIconDirectory, FileName);
    }
}
