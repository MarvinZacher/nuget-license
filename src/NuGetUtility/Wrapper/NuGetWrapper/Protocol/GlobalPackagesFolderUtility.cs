// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Collections.Generic;
using System.Security.Principal;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using IWrappedPackageMetadata = NuGetUtility.Wrapper.NuGetWrapper.Packaging.IPackageMetadata;
using OriginalGlobalPackagesFolderUtility = NuGet.Protocol.GlobalPackagesFolderUtility;
using OriginalPackageIdentity = NuGet.Packaging.Core.PackageIdentity;
using PackageIdentity = NuGetUtility.Wrapper.NuGetWrapper.Packaging.Core.PackageIdentity;

namespace NuGetUtility.Wrapper.NuGetWrapper.Protocol
{
    internal class GlobalPackagesFolderUtility : IGlobalPackagesFolderUtility
    {
        private readonly string _globalPackagesFolder;

        public GlobalPackagesFolderUtility(ISettings settings)
        {
            _globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);
        }

        public IWrappedPackageMetadata? GetPackage(PackageIdentity identity)
        {
            DownloadResourceResult cachedPackage = OriginalGlobalPackagesFolderUtility.GetPackage(new OriginalPackageIdentity(identity.Id, new NuGetVersion(identity.Version.ToString()!)), _globalPackagesFolder);
            if (cachedPackage == null)
            {
                return null;
            }

            return GetPackageFromReader(identity, cachedPackage.PackageReader);
        }

        public static IWrappedPackageMetadata? GetPackageFromStream(PackageIdentity identity, Stream stream)
        {
            using PackageReaderBase packageReader = new PackageArchiveReader(stream);
            return GetPackageFromReader(identity, packageReader);
        }

        public static IWrappedPackageMetadata? GetPackageFromReader(PackageIdentity identity, PackageReaderBase pkgStream)
        {
            var manifest = Manifest.ReadFrom(pkgStream.GetNuspec(), true);

            if (manifest.Metadata.Version.Equals(identity.Version))
            {
                return null;
            }

            string? licenseContent = null;
            if (manifest.Metadata.LicenseMetadata?.Type == LicenseType.File)
            {
                string licenseFile = manifest.Metadata.LicenseMetadata!.License;
                Stream stream = pkgStream.GetStream(licenseFile);
                using StreamReader reader = new StreamReader(stream);
                licenseContent = reader.ReadToEnd();
            }

            return new WrappedPackageMetadata(manifest.Metadata, licenseContent);
        }
    }
}
