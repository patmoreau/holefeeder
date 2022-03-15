using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders;

public class CategoryBuilder
{
    private static readonly object _locker = new();
    private static int _seed = 1;
    private static readonly List<CategoryEntity> _categories = new();
    private readonly string _color;
    private bool _favorite;

    private readonly Guid _id;
    private string _name;
    private CategoryType _type;
    private Guid _userId;

    private CategoryBuilder(Guid id, int seed)
    {
        _id = id;
        _name = $"Category{seed}";
        _type = CategoryType.Expense;
        _color = $"#{seed}";
        _favorite = false;
        _userId = Guid.NewGuid();
    }

    public static IReadOnlyList<CategoryEntity> Categories => _categories;

    public static CategoryBuilder Create(Guid id)
    {
        lock (_locker)
        {
            return new CategoryBuilder(id, _seed);
        }
    }

    public CategoryBuilder Named(string name)
    {
        _name = name;

        return this;
    }

    public CategoryBuilder OfType(CategoryType type)
    {
        _type = type;

        return this;
    }

    public CategoryBuilder IsFavorite()
    {
        _favorite = true;

        return this;
    }

    public CategoryBuilder ForUser(Guid userId)
    {
        _userId = userId;

        return this;
    }

    public void Build()
    {
        var schema = new CategoryEntity
        {
            Id = _id,
            Name = _name,
            Type = _type,
            UserId = _userId,
            Favorite = _favorite,
            Color = _color
        };

        lock (_locker)
        {
            _seed++;
            _categories.Add(schema);
        }
    }
}
