namespace Uranus.Staff;

public enum EProject
{
    None,
    Prometheus,
    Pantheon,
    Aion,
    Hydra,
    Cadmus,
    Sphynx,
    Argos,
    Panacea,
    Quest,
    Deimos,
    Vulcan,
    Phoenix,
    Khaos,
    Hades,
    Hermes,
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
        project = project.ToUpper();
        return project switch
        {
            "PANTHEON" => EProject.Pantheon,
            "PROMETHEUS" => EProject.Prometheus,
            "AION" => EProject.Aion,
            "HYDRA" => EProject.Hydra,
            "CADMUS" => EProject.Cadmus,
            "SPHYNX" => EProject.Sphynx,
            "ARGOS" => EProject.Argos,
            "PANACEA" => EProject.Panacea,
            "QUEST" => EProject.Quest,
            "DEIMOS" => EProject.Deimos,
            "VULCAN" => EProject.Vulcan,
            "PHOENIX" => EProject.Phoenix,
            "KHAOS" => EProject.Khaos,
            "HADES" => EProject.Hades,
            "HERMES" => EProject.Hermes,
            _ => EProject.None
        };
    }

}