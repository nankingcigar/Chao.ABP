using System;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace Chao.CAS;

[LoggingFilter]
public interface ICASApi
{
    [XmlReturn(EnsureMatchAcceptContentType = false)]
    [WebApiClientCore.Attributes.HttpGet()]
    Task<Profile> Get([Uri] string url);
}