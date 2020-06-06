using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace OauthDemoApi.Versioning
{
    public class ApiVersionsAttribute : ApiVersionsBaseAttribute, IApiVersionProvider
    {
        public ApiVersionsAttribute(string introduced, bool excludeAlpha = false) : base(ListVersions(introduced, null, excludeAlpha))
        {
            Options = ApiVersionProviderOptions.None;
        }

        public ApiVersionsAttribute(string introduced, string deprecated) : base(ListVersions(introduced, deprecated, true))
        {
            Options = ApiVersionProviderOptions.Deprecated;
        }

        public ApiVersionProviderOptions Options { get; }

        private static ApiVersion[] ListVersions(string introduced, string deprecated, bool excludeAlpha)
        {
            var introducedVersion = ApiVersion.Parse(introduced);
            var versionStrings = typeof(Versions).GetFields()
                .Select(x => x.GetValue(null))
                .OfType<string>();

            if (excludeAlpha)
                versionStrings = versionStrings.Where(x => x != Versioning.Versions.Alpha);

            var versions = versionStrings
                .Select(ApiVersion.Parse)
                .Where(x => x >= introducedVersion);

            if (!string.IsNullOrEmpty(deprecated))
            {
                var deprecatedVersion = ApiVersion.Parse(deprecated);
                versions = versions.Where(x => x <= deprecatedVersion);
            }

            return versions.ToArray();
        }
    }
}
