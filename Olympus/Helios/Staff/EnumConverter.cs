using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff
{
    public enum EProject
    {
        Vulcan,
        Prometheus,
        EverBurn,
        Pantheon
    }

    public enum ELicence
    {
        LF,
        LO,
        WP
    }

    public static class EnumConverter
    {

        /**************************** CONVERT Data ***************************/
        public static string ProjectToString(EProject project)
        {
            if (project == EProject.Prometheus)
                return "Prometheus";
            if (project == EProject.Vulcan)
                return "Vulcan";
            if (project == EProject.EverBurn)
                return "EverBurn";
            if (project == EProject.Pantheon)
                return "Pantheon";
            return "None";
        }

        public static EProject StringToProject(string project)
        {
            project = project.ToUpper();
            if (project == "VULCAN")
                return EProject.Vulcan;
            if (project == "EVERBURN")
                return EProject.EverBurn;
            if (project == "PANTHEON")
                return EProject.Pantheon;
            return EProject.Prometheus;
        }

    }
}
