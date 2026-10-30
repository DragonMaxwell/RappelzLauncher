using System.Xml.Linq;

namespace RappelzLauncher.Core;

public sealed record UpdatePackage(Version Version, string FileName);

public sealed record UpdateManifest(UpdatePackage Launcher, IReadOnlyList<UpdatePackage> Patches)
{
    public static UpdateManifest Parse(string xml)
    {
        XDocument document = XDocument.Parse(xml, LoadOptions.None);
        XElement root = document.Root ?? throw new InvalidDataException("Update manifest is empty.");

        if (root.Name.LocalName != "updates")
        {
            throw new InvalidDataException("Update manifest must use the updates root element.");
        }

        XElement launcher = root.Element("launcherupdate") ?? throw new InvalidDataException("Missing launcherupdate element.");
        XElement patches = root.Element("patches") ?? throw new InvalidDataException("Missing patches element.");
        UpdatePackage launcherPackage = ParsePackage(launcher, 4);
        List<UpdatePackage> patchPackages = patches.Elements("patch").Select(patch => ParsePackage(patch, 3)).OrderBy(patch => patch.Version).ToList();

        if (patchPackages.Select(patch => patch.Version).Distinct().Count() != patchPackages.Count)
        {
            throw new InvalidDataException("Update manifest contains duplicate patch versions.");
        }

        return new UpdateManifest(launcherPackage, patchPackages);
    }

    private static UpdatePackage ParsePackage(XElement element, int minimumComponents)
    {
        string versionText = element.Element("version")?.Value.Trim() ?? throw new InvalidDataException("Update entry is missing its version.");
        string fileName = element.Element("file")?.Value.Trim() ?? throw new InvalidDataException("Update entry is missing its file.");
        Version version = Version.Parse(versionText);

        if (minimumComponents == 4 && version.Revision < 0)
        {
            throw new InvalidDataException($"Launcher version must have four numeric components: {versionText}");
        }

        if (minimumComponents == 3 && version.Build < 0)
        {
            throw new InvalidDataException($"Patch version must have at least three numeric components: {versionText}");
        }

        if (minimumComponents == 4 && (version.Major is < 1 or > 9999 || version.Minor is < 1 or > 12 || version.Build is < 1 or > 31 || version.Revision > 2359 || version.Revision % 100 > 59))
        {
            throw new InvalidDataException($"Launcher version must follow YYYY.MM.DD.HHMM: {versionText}");
        }

        if (fileName != Path.GetFileName(fileName) || fileName.Contains('/') || fileName.Contains('\\') || fileName.Contains(':') || !fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"Invalid update archive name: {fileName}");
        }

        if (minimumComponents == 4)
        {
            _ = new DateTime(version.Major, version.Minor, version.Build);
        }

        return new UpdatePackage(version, fileName);
    }
}
