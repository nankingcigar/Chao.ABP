using System;
using System.Collections.Generic;

namespace Chao.Abp.ResultHandling.Option;

public class ChaoAbpResultHandlingOption
{
    public ChaoAbpResultHandlingOption()
    {
        ExcludeControllerTypes = new List<Type>();
    }

    public virtual IList<Type> ExcludeControllerTypes { get; set; }
}