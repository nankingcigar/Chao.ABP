using System.Collections.Generic;
using System.Net;

namespace Chao.Abp.WebApiClient;

public class ChaoAbpWebApiClientOption
{
    public virtual IEnumerable<HttpStatusCode> HttpStatusCodes { get; set; } = [HttpStatusCode.OK, HttpStatusCode.NoContent];
}