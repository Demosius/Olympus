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
        Torch,
        Pantheon,
        Khaos
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
            if (project == EProject.Torch)
                return "Torch";
            if (project == EProject.Pantheon)
                return "Pantheon";
            if (project == EProject.Khaos)
                return "Khaos";;
            return "None";
        }

        public static EProject StringToProject(string project)
        {
            project = (project ?? "none").ToUpper();
            if (project == "VULCAN")
                return EProject.Vulcan;
            if (project == "TORCH")
                return EProject.Torch;
            if (project == "PANTHEON")
                return EProject.Pantheon;
            if (project == "KHAOS")
                return EProject.Khaos;
            if (project == "PROMETHEUS")
                return EProject.Prometheus;
            return EProject.None;
        }

    }
}
