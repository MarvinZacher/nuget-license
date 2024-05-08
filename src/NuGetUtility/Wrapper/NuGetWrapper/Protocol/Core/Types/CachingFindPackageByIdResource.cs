// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Packaging;
using NuGetUtility.Wrapper.NuGetWrapper.Packaging;
using NuGetUtility.Wrapper.NuGetWrapper.Packaging.Core;
using NuGetUtility.Wrapper.NuGetWrapper.Versioning;

namespace NuGetUtility.Wrapper.NuGetWrapper.Protocol.Core.Types
{
    internal class CachingFindPackageByIdResource : IFindPackageByIdResource
    {
        private readonly SourceCacheContext _cacheContext;
        private readonly FindPackageByIdResource _packageByIdResource;

        public CachingFindPackageByIdResource(FindPackageByIdResource packageByIdResource, SourceCacheContext cacheContext)
        {
            _packageByIdResource = packageByIdResource;
            _cacheContext = cacheContext;
        }

        public async Task<Stream?> TryGetStreamAsync(PackageIdentity identity,
            CancellationToken cancellationToken)
        {
            try
            {
                MemoryStream packageData = new MemoryStream();
                bool success = await _packageByIdResource.CopyNupkgToStreamAsync(identity.Id, new NuGetVersion(identity.Version.ToString()!), packageData, _cacheContext, new NullLogger(), cancellationToken);
                if (!success)
                {
                    return null;
                }

                return packageData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
