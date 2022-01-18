using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Uranus.Staff
{
    public enum EProject
    {
        None,
        Vulcan,
        Prometheus,
        Phoenix,
        Pantheon,
        Khaos,
        Aion
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
        public static EProject StringToProject(string project)
        {
            project = (project ?? "none").ToUpper();
            if (project == "VULCAN")
                return EProject.Vulcan;
            if (project == "PHOENIX")
                return EProject.Phoenix;
            if (project == "PANTHEON")
                return EProject.Pantheon;
            if (project == "KHAOS")
                return EProject.Khaos;
            if (project == "PROMETHEUS")
                return EProject.Prometheus;
            if (project == "AION")
                return EProject.Aion;
            return EProject.None;
        }

    }
}
