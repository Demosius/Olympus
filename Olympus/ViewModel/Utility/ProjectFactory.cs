using System;
using Aion.View;
using Khaos.View;
using Pantheon.View;
using Phoenix.View;
using Prometheus.View;
using Styx;
using Uranus;
using Uranus.Staff;
using Uranus.Staff.Model;
using Vulcan.View;

namespace Olympus.ViewModel.Utility
{
    public static class ProjectFactory
    {
        public static IProject GetProject(Project project) => GetProject(project.Reference);

        public static IProject GetProject(EProject project)
        {
            return project switch
            {
                EProject.Vulcan => new VulcanPage(),
                EProject.Prometheus => new PrometheusPage(App.Helios, App.Charon),
                EProject.Pantheon => new PantheonPage(),
                EProject.Phoenix => new PhoenixPage(),
                EProject.Khaos => new KhaosPage(),
                EProject.Aion => new AionPage(App.Helios, App.Charon),
                EProject.None => null,
                _ => throw new ArgumentOutOfRangeException(nameof(project), project, null)
            };
        }

        public static bool CanLaunch(Project project, Charon charon) => CanLaunch(project.Reference, charon);
        
        public static bool CanLaunch(EProject project, Charon charon)
        {
            if (charon.CurrentUser is not null)
                return true;
            return !RequiresUser(project);
        }

        public static bool RequiresUser(EProject project)
        {
            return project switch
            {
                EProject.Aion => AionPage.RequiresUser,
                EProject.Vulcan => VulcanPage.RequiresUser,
                EProject.Prometheus => PrometheusPage.RequiresUser,
                EProject.Phoenix => PhoenixPage.RequiresUser,
                EProject.Pantheon => PantheonPage.RequiresUser,
                EProject.Khaos => KhaosPage.RequiresUser,
                EProject.None => false,
                _ => throw new ArgumentOutOfRangeException(nameof(project), project, null)
            };
        }
    }
}
