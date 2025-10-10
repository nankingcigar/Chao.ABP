using System;
using System.Collections.Generic;

namespace Chao.Abp.ExceptionHandling.Option;

public class ChaoAbpExceptionHandlingOption
{
    public ChaoAbpExceptionHandlingOption()
    {
        ExcludeControllerTypes = [];
        ExceptionCodeJoinString = ".";
    }

    public virtual string ExceptionCodeJoinString { get; set; }
    public virtual IList<Type> ExcludeControllerTypes { get; set; }
}