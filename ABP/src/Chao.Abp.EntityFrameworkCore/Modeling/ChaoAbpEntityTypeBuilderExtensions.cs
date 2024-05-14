using Chao.Abp.EntityFrameworkCore.ValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.EntityFrameworkCore.ValueComparers;

namespace Chao.Abp.EntityFrameworkCore.Modeling;

public static class ChaoAbpEntityTypeBuilderExtensions
{
    public static void ConfigureByConvention_Chao(this EntityTypeBuilder b)
    {
        b.TryConfigureExtraProperties();
    }

    public static void TryConfigureExtraProperties(this EntityTypeBuilder b)
    {
        if (!b.Metadata.ClrType.IsAssignableTo<IHasExtraProperties>())
        {
            return;
        }

        b.Property<ExtraPropertyDictionary>(nameof(IHasExtraProperties.ExtraProperties))
            .HasColumnName(nameof(IHasExtraProperties.ExtraProperties))
            .HasConversion(new ChaoExtraPropertiesValueConverter(b.Metadata.ClrType))
            .Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());

        b.TryConfigureObjectExtensions();
    }
}