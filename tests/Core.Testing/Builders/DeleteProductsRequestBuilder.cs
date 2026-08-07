using Application.Features.Products.Models.Requests;
using Core.Builders;

namespace Core.Testing.Builders;

public sealed class DeleteProductsRequestBuilder : Builder<DeleteProductsRequest>
{
    protected override DeleteProductsRequest Item { get; set; }

    long[] ids = [];
    bool ignoreNotFound = false;

    public DeleteProductsRequestBuilder()
    {
        Item = new DeleteProductsRequest(ids, ignoreNotFound);
    }

    public DeleteProductsRequestBuilder WithIds(long[] ids)
    {
        this.ids = ids;
        Item = new DeleteProductsRequest(ids, ignoreNotFound);
        return this;
    }

    public DeleteProductsRequestBuilder WithIgnoreNotFound(bool ignoreNotFound)
    {
        this.ignoreNotFound = ignoreNotFound;
        Item = new DeleteProductsRequest(ids, ignoreNotFound);
        return this;
    }

    public DeleteProductsRequestBuilder WithValue(string propertyName, object? value)
    {
        return propertyName switch
        {
            nameof(DeleteProductsRequest.Ids) => WithIds((long[])value!),
            nameof(DeleteProductsRequest.IgnoreNotFound) => WithIgnoreNotFound((bool)value!),
            _ => throw new ArgumentException($"Unknown property: {propertyName}"),
        };
    }
}