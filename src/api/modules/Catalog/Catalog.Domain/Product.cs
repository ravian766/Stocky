using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Catalog.Domain.Events;

namespace FSH.Starter.WebApi.Catalog.Domain;
public class Product : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }

    public Uri? ImagePath { get; private set; }

    public static Product Create(string name, string? description, decimal price, Uri? imagePath)
    {
        var product = new Product();

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.ImagePath = imagePath;

        product.QueueDomainEvent(new ProductCreated() { Product = product });

        return product;
    }

    public Product Update(string? name, string? description, decimal? price, Uri? imagePath)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (price.HasValue && Price != price) Price = price.Value;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = imagePath;

        this.QueueDomainEvent(new ProductUpdated() { Product = this });
        return this;
    }

    public static Product Update(Guid id, string name, string? description, decimal price, Uri? imagePath)
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            ImagePath = imagePath
            
        };

        product.QueueDomainEvent(new ProductUpdated() { Product = product });

        return product;
    }
}
