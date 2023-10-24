using System;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace Chao.Abp.CAS;

[LoggingFilter]
public interface ITokenApi
{
    [HttpPost]
    Task<Token> Get([Uri] string url, [Header] string __tenant, [FormField] string grant_type, [FormField] string scope, [FormField] string client_id, [FormField] string username, [FormField] string password);
}