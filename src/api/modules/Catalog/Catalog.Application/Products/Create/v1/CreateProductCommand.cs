using System.ComponentModel;
using FSH.Framework.Core.Storage.File.Features;
using MediatR;

namespace FSH.Starter.WebApi.Catalog.Application.Products.Create.v1;
public sealed record CreateProductCommand(
    [property: DefaultValue("Sample Product")] string? Name,
    [property: DefaultValue(10)] decimal Price,
    [property: DefaultValue("")] FileUploadCommand? Image,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<CreateProductResponse>;
