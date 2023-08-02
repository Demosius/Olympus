using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public enum ENotificationType
{
    Information,
    Update,
    Urgent,
}

public class DevNotification : Message
{
    public ENotificationType NotificationType { get; set; }
    public bool Obsolete { get; set; }
    public int SortRank { get; set; }

    [ManyToMany(typeof(NotificationTag), nameof(NotificationTag.NotificationID), nameof(Tag.Notifications), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Tag> Tags { get; set; }

    public DevNotification()
    {
        ConversationID = Guid.Empty;
        Tags = new List<Tag>();
    }

    public DevNotification(User dev, DateTime dateTime)
    {
        UserID = dev.ID;
        User = dev;
        DateTime = dateTime;
        Tags = new List<Tag>();
    }
}