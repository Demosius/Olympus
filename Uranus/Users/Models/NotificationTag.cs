using System;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class NotificationTag
{
    [ForeignKey(typeof(DevNotification))] public Guid NotificationID { get; set; }
    [ForeignKey(typeof(Tag))] public Guid TagID { get; set; }
    
}