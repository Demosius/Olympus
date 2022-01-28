using Uranus.Staff.Model;
using System.Collections.Generic;
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

        public bool Employee(Employee employee, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(employee, pushType);

        public bool Department(Department department, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(department, pushType);

        public bool Role(Role role, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(role, pushType);

        public void EstablishInitialProjects(List<Project> newProjects)
        {
            Chariot.Database.RunInTransaction(() =>
            {
                foreach (var project in newProjects)
                {
                    Chariot.Database.InsertOrReplace(project);
                    Chariot.Database.InsertOrReplace(project.Icon);
                }
            });
        }

        public void CopyProjectIconsFromSource(string sourceDirectory)
        {
            foreach (var filePath in Directory.GetFiles(sourceDirectory))
            {
                var fileName = Path.GetFileName(filePath);
                if (Path.GetExtension(filePath) == ".ico")
                {
                    File.Copy(filePath, Path.Combine(Chariot.ProjectIconDirectory, fileName), true);
                }
            }
        }
    }
}
