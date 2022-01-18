using Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Uranus.Staff
{
    public class StaffCreator
    {
        public StaffChariot Chariot { get; set; }

        public StaffCreator(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

        public bool Employee(Employee employee, PushType pushType = PushType.ObjectOnly) => Chariot.Create(employee, pushType);

        public bool Department(Department department, PushType pushType = PushType.ObjectOnly) => Chariot.Create(department, pushType);

        public bool Role(Role role, PushType pushType = PushType.ObjectOnly) => Chariot.Create(role, pushType);

        public void EstablishInitialProjects(List<Project> newProjects)
        {
            Chariot.Database.RunInTransaction(() =>
            {
                foreach (Project project in newProjects)
                {
                    Chariot.Database.InsertOrReplace(project);
                    Chariot.Database.InsertOrReplace(project.Icon);
                }
            });
        }

        public void CopyProjectIconsFromSource(string sourceDirectory)
        {
            string fileName;
            foreach (string filePath in Directory.GetFiles(sourceDirectory))
            {
                fileName = Path.GetFileName(filePath);
                if (Path.GetExtension(filePath) == ".ico")
                {
                    File.Copy(filePath, Path.Combine(Chariot.ProjectIconDirectory, fileName), true);
                }
            }
        }
    }
}
