// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using NuGetUtility.Wrapper.NuGetWrapper.Packaging;
using NuGetUtility.Wrapper.NuGetWrapper.Packaging.Core;

namespace NuGetUtility.Wrapper.NuGetWrapper.Protocol.Core.Types
{
    public interface IFindPackageByIdResource
    {
        Task<Stream?> TryGetStreamAsync(PackageIdentity identity, CancellationToken cancellationToken);
    }
}
