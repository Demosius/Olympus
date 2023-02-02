using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class Tag
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Name { get; set; }

    [ManyToMany(typeof(NotificationTag), nameof(NotificationTag.TagID), nameof(DevNotification.Tags), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<DevNotification> Notifications { get; set; }

    [ManyToMany(typeof(FAQTag), nameof(FAQTag.TagID), nameof(FAQ.Tags), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<FAQ> FAQList { get; set; }

    public Tag()
    {
        Name = string.Empty;
        Notifications = new List<DevNotification>();
        FAQList = new List<FAQ>();
    }

    public Tag(string name)
    {
        ID = Guid.NewGuid();
        Name =name;
        Notifications = new List<DevNotification>();
        FAQList =new List<FAQ>();
    }

    public Tag(Guid iD, string name)
    {
        ID = iD;
        Name = name;
        Notifications = new List<DevNotification>();
        FAQList =new List<FAQ>();
    }
}