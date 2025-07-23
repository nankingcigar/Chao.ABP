using System;

namespace Chao.Abp.ResultHandling.Model;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
public class WrapResultAttribute() : Attribute
{
    public virtual bool WrapOnError { get; set; } = true;
    public virtual bool WrapOnSuccess { get; set; } = true;
    public static WrapResultAttribute Default { get; set; } = new WrapResultAttribute();
}