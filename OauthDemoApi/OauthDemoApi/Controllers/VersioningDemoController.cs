using Microsoft.AspNetCore.Mvc;
using OauthDemoApi.Versioning;

namespace OauthDemoApi.Controllers
{
    [ApiController]
    [Route("api/versioning-demo")]
    [Route("api/v{version:apiVersion}/versioning-demo")]
    public class VersioningDemoController : ControllerBase
    {
        [ApiVersions(Versions.V1, Versions.V1_1)]
        public string Get_V1_1()
        {
            return "V1 - V1.1 OLDEST content";
        }

        [ApiVersions(Versions.V2, Versions.V2_1)]
        public string Get_V2_1()
        {
            return "V2 - V2.1 content";
        }

        [ApiVersions(Versions.V2_2)]
        public string Get()
        {
            return "V2.2+ NEWEST content";
        }
    }
}
