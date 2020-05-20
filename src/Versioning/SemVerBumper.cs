using System;

namespace Skarp.Version.Cli.Versioning
{
    public class SemVerBumper
    {
        /// <summary>
        /// Bump the currently parsed version information with the specified <paramref name="bump"/>
        /// </summary>
        /// <param name="bump">The bump to apply to the version</param>
        /// <param name="specificVersionToApply">The specific version to apply if bump is Specific</param>
        /// <param name="buildMeta"></param>
        public SemVer Bump(SemVer currentVersion, VersionBump bump, string specificVersionToApply = "",
            string buildMeta = "")
        {
            var newVersion = SemVer.FromString(currentVersion.ToSemVerVersionString());
            newVersion.BuildMeta = buildMeta;

            switch (bump)
            {
                case VersionBump.Major:
                {
                    HandleMajorBump(newVersion);
                    break;
                }
                case VersionBump.PreMajor:
                {
                    HandlePreMajorBump(newVersion);
                    break;
                }
                case VersionBump.Minor:
                {
                    HandleMinorBump(newVersion);
                    break;
                }
                case VersionBump.PreMinor:
                {
                    HandlePreMinorBump(newVersion);
                    break;
                }
                case VersionBump.Patch:
                {
                    HandlePatchBump(newVersion);
                    break;
                }
                case VersionBump.PrePatch:
                {
                    HandlePrePatchBump(newVersion);
                    break;
                }
                case VersionBump.PreRelease:
                {
                    HandlePreReleaseBump(newVersion);
                    break;
                }
                case VersionBump.Specific:
                {
                    HandleSpecificVersion(specificVersionToApply, newVersion);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException($"VersionBump : {bump} not supported");
                }
            }

            return newVersion;
        }

        private static void HandleSpecificVersion(string specificVersionToApply, SemVer newVersion)
        {
            if (string.IsNullOrEmpty(specificVersionToApply))
            {
                throw new ArgumentException($"When bump is specific, specificVersionToApply must be provided");
            }

            var specific = SemVer.FromString(specificVersionToApply);
            newVersion.Major = specific.Major;
            newVersion.Minor = specific.Minor;
            newVersion.Patch = specific.Patch;
            newVersion.PreRelease = specific.PreRelease;
            newVersion.BuildMeta = specific.BuildMeta;
        }

        private static void HandlePreReleaseBump(SemVer newVersion)
        {
            if (!newVersion.IsPreRelease)
            {
                throw new InvalidOperationException(
                    "Cannot Prerelease bump when not already a prerelease. Please use prepatch, preminor or prepatch to prepare");
            }

            string preReleaseLabel = "next";

            if (!int.TryParse(newVersion.PreRelease, out var preReleaseNumber))
            {
                // it was not just a number, let's try to split it (pre-release might look like `next.42`)
                var preReleaseSplit = newVersion.PreRelease.Split(".");
                if (preReleaseSplit.Length != 2)
                {
                    throw new ArgumentException(
                        $"Pre-release part invalid. Must be either numeric or `label.number`. Got {newVersion.PreRelease}");
                }

                if (!int.TryParse(preReleaseSplit[1], out preReleaseNumber))
                {
                    throw new ArgumentException(
                        "Second part of pre-release is not numeric, cannot apply automatic prerelease roll. Should follow pattern `label.number`");
                }

                preReleaseLabel = preReleaseSplit[0];
            }

            // increment the pre-release number
            preReleaseNumber += 1;
            newVersion.PreRelease = $"{preReleaseLabel}.{preReleaseNumber}";
        }

        private static void HandlePrePatchBump(SemVer newVersion)
        {
            newVersion.Patch += 1;
            newVersion.PreRelease = "next.0";
        }

        private void HandlePatchBump(SemVer newVersion)
        {
            if (!newVersion.IsPreRelease)
            {
                newVersion.Patch += 1;
            }
            else
            {
                newVersion.PreRelease = string.Empty;
                newVersion.BuildMeta = string.Empty;
            }
        }

        private void HandlePreMinorBump(SemVer newVersion)
        {
            newVersion.Minor += 1;
            newVersion.Patch = 0;
            newVersion.PreRelease = "next.0";
        }

        private void HandleMinorBump(SemVer newVersion)
        {
            if (!newVersion.IsPreRelease)
            {
                newVersion.Minor += 1;
                newVersion.Patch = 0;
            }
            else
            {
                newVersion.PreRelease = string.Empty;
                newVersion.BuildMeta = string.Empty;
            }
        }

        private void HandlePreMajorBump(SemVer newVersion)
        {
            newVersion.Major += 1;
            newVersion.Minor = 0;
            newVersion.Patch = 0;
            newVersion.PreRelease = "next.0";
        }

        private void HandleMajorBump(SemVer newVersion)
        {
            if (!newVersion.IsPreRelease)
            {
                newVersion.Major += 1;
                newVersion.Minor = 0;
                newVersion.Patch = 0;
            }
            else
            {
                newVersion.PreRelease = string.Empty;
                newVersion.BuildMeta = string.Empty;
            }
        }
    }
}