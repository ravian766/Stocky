using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Catalog;

public partial class Products
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<ProductResponse, Guid, ProductViewModel> Context { get; set; } = default!;

    private EntityTable<ProductResponse, Guid, ProductViewModel> _table = default!;

    protected override void OnInitialized() =>
        Context = new(
            entityName: "Product",
            entityNamePlural: "Products",
            entityResource: FshResources.Products,
            fields: new()
            {
                new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.Price, "Price", "Price")
               // new(prod => prod.ImagePath, "ImagePath", "ImagePath")
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id!.Value,
            searchFunc: async filter =>
            {
                var productFilter = filter.Adapt<SearchProductsCommand>();
                productFilter.MinimumRate = Convert.ToDouble(SearchMinimumRate);
                productFilter.MaximumRate = Convert.ToDouble(SearchMaximumRate);
                var result = await _client.SearchProductsEndpointAsync("1", productFilter);
                return result.Adapt<PaginationResponse<ProductResponse>>();
            },
            createFunc: async prod =>
            {
                await _client.CreateProductEndpointAsync("1", prod.Adapt<CreateProductCommand>());
            },
            updateFunc: async (id, prod) =>
            {
                await _client.UpdateProductEndpointAsync("1", id, prod.Adapt<UpdateProductCommand>());
            },
            deleteFunc: async id => await _client.DeleteProductEndpointAsync("1", id));

    // Advanced Search

    private Guid _searchBrandId;
    private Guid SearchBrandId
    {
        get => _searchBrandId;
        set
        {
            _searchBrandId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMinimumRate;
    private decimal SearchMinimumRate
    {
        get => _searchMinimumRate;
        set
        {
            _searchMinimumRate = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMaximumRate = 9999;
    private decimal SearchMaximumRate
    {
        get => _searchMaximumRate;
        set
        {
            _searchMaximumRate = value;
            _ = _table.ReloadDataAsync();
        }
    }
    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file is not null)
        {
            string? extension = Path.GetExtension(file.Name);
            if (!AppConstants.SupportedImageFormats.Contains(extension.ToLower()))
            {
                Toast.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            string? fileName = $"{Guid.NewGuid():N}";
            fileName = fileName[..Math.Min(fileName.Length, 90)];
            var imageFile = await file.RequestImageFileAsync(AppConstants.StandardImageFormat, AppConstants.MaxImageWidth, AppConstants.MaxImageHeight);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream(AppConstants.MaxAllowedSize).ReadAsync(buffer);
            string? base64String = $"data:{AppConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            Context.AddEditModal.RequestModel.ImagePath = new FileUploadCommand() { Name = fileName, Data = base64String, Extension = extension };

            //await UpdateProfileAsync();
        }
    }
}

public class ProductViewModel : UpdateProductCommand
{
        public string? Image { get; set; }
    //    public string? imageinbytes { get; set; }
    //    public string? imageextension { get; set; }
}
