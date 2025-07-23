using System;

namespace Chao.Abp.ExceptionHandling;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ErrorCodeAttribute(params string[] prefix) : Attribute
{
    public virtual string[] Prefix { get; set; } = prefix;
    public virtual bool ClassName { get; set; } = true;
    public virtual bool MethodName { get; set; } = true;
}