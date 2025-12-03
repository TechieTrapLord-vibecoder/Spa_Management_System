using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

/// <summary>
/// Interface for entities that can be synced to the cloud.
/// Implement this on any model that needs to be synchronized.
/// </summary>
public interface ISyncable
{
    /// <summary>
    /// Unique identifier for this record across all devices/locations.
    /// Generated once when the record is created, never changes.
    /// </summary>
    Guid SyncId { get; set; }

    /// <summary>
    /// When this record was last modified locally.
    /// Updated automatically on any change.
    /// </summary>
    DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// When this record was last successfully synced to the cloud.
    /// </summary>
    DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// Current sync status: 'pending', 'synced', 'conflict', 'error'
    /// </summary>
    string SyncStatus { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency during sync.
    /// Incremented on each modification.
    /// </summary>
    int SyncVersion { get; set; }
}

/// <summary>
/// Base class with common sync properties.
/// Inherit from this or implement ISyncable directly.
/// </summary>
public abstract class SyncableEntity : ISyncable
{
    [Column("sync_id")]
    public Guid SyncId { get; set; } = Guid.NewGuid();

    [Column("last_modified_at")]
    public DateTime? LastModifiedAt { get; set; }

    [Column("last_synced_at")]
    public DateTime? LastSyncedAt { get; set; }

    [MaxLength(20)]
    [Column("sync_status")]
    public string SyncStatus { get; set; } = "pending";

    [Column("sync_version")]
    public int SyncVersion { get; set; } = 1;
}

