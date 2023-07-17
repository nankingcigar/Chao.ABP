using System;
using System.Collections.Generic;

namespace Chao.Abp.ExceptionHandling.Option;

public class ChaoAbpExceptionHandlingOption
{
    public ChaoAbpExceptionHandlingOption()
    {
        ExcludeControllerTypes = new List<Type>();
    }

    public virtual IList<Type> ExcludeControllerTypes { get; set; }
}