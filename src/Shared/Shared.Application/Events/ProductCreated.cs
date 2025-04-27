using Shared.Application.EventBus;

namespace Shared.Application.Events;

public class ProductCreated : EventBase
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public decimal Price { get; init; }

    public Dictionary<Guid, string> CategoriesNames { get; init; } = [];
}