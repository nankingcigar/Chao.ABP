namespace Chao.Abp.AspNetCore.Mvc.Authorization;

public interface IImpersonatorProvider
{
    bool Impersonator { get; }
}