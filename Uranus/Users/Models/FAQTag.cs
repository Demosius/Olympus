using System;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class FAQTag
{
    [ForeignKey(typeof(FAQ))] public Guid FAQ_ID { get; set; }
    [ForeignKey(typeof(Tag))] public Guid TagID { get; set; }
    
}