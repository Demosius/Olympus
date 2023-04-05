using Aion.View;
using Argos.Views;
using Cadmus.Views;
using Hades.Views;
using Hydra.Views;
using Khaos.Views;
using Panacea.Views;
using Pantheon.Views;
using Phoenix.Views;
using Prometheus.Views;
using Sphynx.Views;
using Styx;
using System;
using Uranus.Interfaces;
using Uranus.Staff;
using Uranus.Staff.Models;
using Vulcan.Views;

namespace Olympus.ViewModels.Utility;

public static class ProjectFactory
{
    public static IProject? GetProject(Project project) => GetProject(project.Reference);

    public static IProject? GetProject(EProject project)
    {
        return project switch
        {
            EProject.Vulcan => new VulcanPage(),
            EProject.Prometheus => new PrometheusPage(App.Helios, App.Charon),
            EProject.Pantheon => new PantheonPage(App.Charon, App.Helios),
            EProject.Phoenix => new PhoenixPage(),
            EProject.Khaos => new KhaosPage(),
            EProject.Aion => new AionPage(App.Helios, App.Charon),
            EProject.Hydra => new HydraPage(App.Helios, App.Charon),
            EProject.Cadmus => new CadmusPage(App.Helios),
            EProject.Sphynx => new SphynxPage(App.Helios, App.Charon),
            EProject.Argos => new ArgosPage(App.Helios),
            EProject.Hades => new HadesPage(App.Helios),
            EProject.Panacea => new PanaceaPage(App.Helios),
            EProject.None => null,
            _ => throw new ArgumentOutOfRangeException(nameof(project), project, null)
        };
    }

    public static bool CanLaunch(Project project, Charon charon) => CanLaunch(project.Reference, charon);

    public static bool CanLaunch(EProject project, Charon charon)
    {
        if (charon.User is not null)
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
            EProject.Hydra => HydraPage.RequiresUser,
            EProject.Cadmus => CadmusPage.RequiresUser,
            EProject.Sphynx => SphynxPage.RequiresUser,
            EProject.Argos => ArgosPage.RequiresUser,
            EProject.Hades => HadesPage.RequiresUser,
            EProject.Panacea => PanaceaPage.RequiresUser,
            EProject.None => false,
            _ => throw new ArgumentOutOfRangeException(nameof(project), project, null)
        };
    }
}