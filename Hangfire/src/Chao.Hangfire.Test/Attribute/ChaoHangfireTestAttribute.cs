using Hangfire;
using System;
using System.Reflection;
using Xunit.Sdk;

namespace Chao.Hangfire.Test.Attribute;

public class ChaoHangfireTestAttribute : BeforeAfterTestAttribute
{
    public override void Before(MethodInfo methodUnderTest)
    {
        var backgroundJobClient = new BackgroundJobClient();
        var clientFactoryPropertyInfo = typeof(BackgroundJob).GetProperty("ClientFactory", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        object o = (Func<IBackgroundJobClient>)(() => backgroundJobClient);
        clientFactoryPropertyInfo.SetValue(null, o);
        base.Before(methodUnderTest);
    }
}