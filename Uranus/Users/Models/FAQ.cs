using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class FAQ
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }

    [ManyToMany(typeof(FAQTag), nameof(FAQTag.FAQ_ID), nameof(Tag.FAQList), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Tag> Tags { get; set; }

    public FAQ()
    {
        ID = Guid.NewGuid();
        Question = string.Empty;
        Answer = string.Empty;
        Tags = new List<Tag>();
    }

    public FAQ(string question, string answer)
    {
        ID = Guid.NewGuid();
        Question = question;
        Answer = answer;
        Tags = new List<Tag>();
    }
}