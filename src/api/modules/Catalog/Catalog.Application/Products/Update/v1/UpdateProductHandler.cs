using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Storage.File;
using FSH.Framework.Core.Storage;
using FSH.Starter.WebApi.Catalog.Domain;
using FSH.Starter.WebApi.Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Catalog.Application.Products.Update.v1;
public sealed class UpdateProductHandler(
    ILogger<UpdateProductHandler> logger,
    IStorageService storageService,
    [FromKeyedServices("catalog:products")] IRepository<Product> repository)
    : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = product ?? throw new ProductNotFoundException(request.Id);

        Uri imageUri = null!;
        // if (request.Image != null || request.DeleteCurrentImage)
        // {
        //user.ImageUrl = await storageService.UploadAsync<Product>(request.ImagePath, FileType.Image);
        imageUri = await storageService.UploadAsync<Product>(request.ImagePath, FileType.Image);
        //if (request.DeleteCurrentImage && imageUri != null)
        //{
        //    storageService.Remove(imageUri);
        //}
        // }

        var updatedProduct = product.Update(request.Name, request.Description, request.Price, imageUri);
        await repository.UpdateAsync(updatedProduct, cancellationToken);
        logger.LogInformation("product with id : {ProductId} updated.", product.Id);
        return new UpdateProductResponse(product.Id);
    }
}
