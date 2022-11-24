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
    FixedBinChecker,
    Cadmus,
    Panacea
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
            "FIXEDBINCHECKER" => EProject.FixedBinChecker,
            "CADMUS" => EProject.Cadmus,
            "PANACEA" => EProject.Panacea,
            _ => EProject.None
        };
    }

}