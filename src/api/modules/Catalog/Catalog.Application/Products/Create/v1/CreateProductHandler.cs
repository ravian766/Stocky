using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Storage.File;
using FSH.Framework.Core.Storage;
using FSH.Starter.WebApi.Catalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Catalog.Application.Products.Create.v1;
public sealed class CreateProductHandler(
    ILogger<CreateProductHandler> logger,
    IStorageService storageService,
    [FromKeyedServices("catalog:products")] IRepository<Product> repository)
    : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Uri imageUri = null; //request.ImagePath ?? null!;
        //// if (request.Image != null || request.DeleteCurrentImage)
        //// {
        ////user.ImageUrl = await storageService.UploadAsync<Product>(request.ImagePath, FileType.Image);
        imageUri = await storageService.UploadAsync<Product>(request.Image, FileType.Image, cancellationToken);
        ////if (request.DeleteCurrentImage && imageUri != null)
        ////{
        ////    storageService.Remove(imageUri);
        ////}
        //// }

        var product = Product.Create(request.Name!, request.Description, request.Price, imageUri);
        await repository.AddAsync(product, cancellationToken);
        logger.LogInformation("product created {ProductId}", product.Id);
        return new CreateProductResponse(product.Id);
    }
}
