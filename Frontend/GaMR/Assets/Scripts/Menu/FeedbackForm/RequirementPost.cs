using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RequirementPost
{
    public string name;
    public string description;
    public int projectId;
    public CategoryId[] categories;

    public RequirementPost(string name, string description, int projectId, CategoryId[] categories)
    {
        this.name = name;
        this.description = description;
        this.projectId = projectId;
        this.categories = categories;
    }
}

[Serializable]
public struct CategoryId
{
    public int id;

    public CategoryId(int id)
    {
        this.id = id;
    }
}
