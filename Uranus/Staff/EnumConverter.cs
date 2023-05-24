namespace Uranus.Staff;

public enum EProject
{
    None,
    Vulcan,
    Prometheus,
    Phoenix,
    Pantheon,
    Khaos,
    Aion,
    Hydra,
    Cadmus,
    Sphynx,
    Argos,
    Hades,
    Panacea,
    Quest,
    Deimos,
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
            "VULCAN" => EProject.Vulcan,
            "PHOENIX" => EProject.Phoenix,
            "PANTHEON" => EProject.Pantheon,
            "KHAOS" => EProject.Khaos,
            "PROMETHEUS" => EProject.Prometheus,
            "AION" => EProject.Aion,
            "HYDRA" => EProject.Hydra,
            "CADMUS" => EProject.Cadmus,
            "SPHYNX" => EProject.Sphynx,
            "ARGOS" => EProject.Argos,
            "HADES" => EProject.Hades,
            "PANACEA" => EProject.Panacea,
            "QUEST" => EProject.Quest,
            "DEIMOS" => EProject.Deimos,
            _ => EProject.None
        };
    }

}