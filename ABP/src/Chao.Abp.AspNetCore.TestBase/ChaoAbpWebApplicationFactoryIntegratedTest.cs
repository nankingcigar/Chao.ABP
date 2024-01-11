using System;
using System.Collections.Generic;
using System.Reflection;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.DependencyInjection;

namespace Chao.Abp.AspNetCore.TestBase;

public abstract class ChaoAbpWebApplicationFactoryIntegratedTest<TProgram> : AbpWebApplicationFactoryIntegratedTest<TProgram> where TProgram : class
{
    public ChaoAbpWebApplicationFactoryIntegratedTest()
        : base()
    {
        LazyServiceProvider = new AbpLazyServiceProvider(this.ServiceProvider);
        InitializedProperties();
    }

    protected virtual AbpLazyServiceProvider LazyServiceProvider { get; }

    public virtual T? LazyGetService<T>()
    {
        return LazyServiceProvider.LazyGetService<T>();
    }

    public virtual object? LazyGetService(Type t)
    {
        return LazyServiceProvider.LazyGetService(t);
    }

    protected virtual void InitializedProperties()
    {
        var properties = new List<PropertyInfo>();
        properties.AddRange(this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
        properties.AddRange(this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public));
        foreach (var property in properties)
        {
            if (property.CanWrite)
            {
                property.SetValue(this, LazyGetService(property.PropertyType));
            }
        }
    }
}