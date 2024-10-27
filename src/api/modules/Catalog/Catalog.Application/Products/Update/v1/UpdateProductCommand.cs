using FSH.Framework.Core.Storage.File.Features;
using MediatR;

namespace FSH.Starter.WebApi.Catalog.Application.Products.Update.v1;
public sealed record UpdateProductCommand(
    Guid Id,
    string? Name,
    decimal Price,
    FileUploadCommand? ImagePath,
    string? Description = null) : IRequest<UpdateProductResponse>;
