using Application.Models.Responses;
using Core.Builders;

namespace Core.Testing.Builders
{
    public class DeleteProductsResponseBuilder : Builder<DeleteProductsResponse>
    {
        protected override DeleteProductsResponse Item { get; set; }

        long[] deletedIds = [];
        long[] notFoundIds = [];

        public DeleteProductsResponseBuilder()
        {
            Item = new DeleteProductsResponse(deletedIds, notFoundIds);
        }

        public DeleteProductsResponseBuilder WithDeletedIds(long[] deletedIds)
        {
            this.deletedIds = deletedIds;
            Item = new DeleteProductsResponse(deletedIds, notFoundIds);
            return this;
        }

        public DeleteProductsResponseBuilder WithNotFoundIds(long[] notFoundIds)
        {
            this.notFoundIds = notFoundIds;
            Item = new DeleteProductsResponse(deletedIds, notFoundIds);
            return this;
        }
    }
}