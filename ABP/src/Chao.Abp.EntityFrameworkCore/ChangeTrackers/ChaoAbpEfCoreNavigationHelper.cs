using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Volo.Abp.EntityFrameworkCore.ChangeTrackers;

namespace Chao.Abp.EntityFrameworkCore.ChangeTrackers;

public class ChaoAbpEfCoreNavigationHelper : AbpEfCoreNavigationHelper
{
    protected override void EntityEntryTrackedOrStateChanged(EntityEntry entityEntry)
    {
        if (entityEntry.State != EntityState.Unchanged)
        {
            return;
        }

        var entryId = GetEntityEntryIdentity(entityEntry);
        if (entryId == null)
        {
            return;
        }

        if (EntityEntries.ContainsKey(entryId))
        {
            return;
        }

        EntityEntries.Add(entryId, new AbpEntityEntry(entryId, entityEntry));
    }
}