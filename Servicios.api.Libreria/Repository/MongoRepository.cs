using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Servicios.api.Libreria.Core;
using Servicios.api.Libreria.Core.Entities;
using System.Linq.Expressions;

namespace Servicios.api.Libreria.Repository
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRepository(IOptions<MongoSettings> optiones)
        {
            var cliente = new MongoClient(optiones.Value.ConnectionString);
            var _db = cliente.GetDatabase(optiones.Value.Database);
            _collection = _db.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

        }
        private protected string GetCollectionName(Type documentType) => ((BsonCollectionAtrribute)documentType.GetCustomAttributes(typeof(BsonCollectionAtrribute), true).FirstOrDefault()).CollectionName;

        public async Task<IEnumerable<TDocument>> GetAll()
        {
            return await _collection.Find(p => true).ToListAsync();
        }

        public async Task<TDocument> GetById(string Id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);

            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task InsertDocument(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task UpdateDocument(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);

        }

        public async Task DeleteDocument(string Id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);
            await _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<PaginationEntity<TDocument>> PaginationBy(Expression<Func<TDocument, bool>> filterExpression, PaginationEntity<TDocument> pagination)
        {
            var sort = Builders<TDocument>.Sort.Ascending(pagination.Sort);
            if (pagination.sortDirection == "desc")
            {
                sort = Builders<TDocument>.Sort.Descending(pagination.Sort);
            }

            if (string.IsNullOrEmpty(pagination.Filter))
            {
                pagination.Data = await _collection.Find(p => true)
                        .Sort(sort)
                        .Skip((pagination.Page - 1) * pagination.PageSize)
                        .Limit(pagination.PageSize)
                        .ToListAsync();
            }
            else
            {
                pagination.Data = await _collection.Find(filterExpression)
                           .Sort(sort)
                           .Skip((pagination.Page - 1) * pagination.PageSize)
                           .Limit(pagination.PageSize)
                           .ToListAsync();
            }
            long totalDocuments = await _collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty);
            var totalPager = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalDocuments / pagination.PageSize)));
            pagination.PagesQuantity = totalPager;
            return pagination;
        }

        public async Task<PaginationEntity<TDocument>> PaginationByFilter(PaginationEntity<TDocument> pagination)
        {
            var sort = Builders<TDocument>.Sort.Ascending(pagination.Sort);
            if (pagination.sortDirection == "desc")
            {
                sort = Builders<TDocument>.Sort.Descending(pagination.Sort);
            }
            var totalDocuments = 0;
            if (string.IsNullOrEmpty(pagination.FilterValue?.Valor))
            {
                pagination.Data = await _collection.Find(p => true)
                        .Sort(sort)
                        .Skip((pagination.Page - 1) * pagination.PageSize)
                        .Limit(pagination.PageSize)
                        .ToListAsync();

                totalDocuments = (await _collection.Find(p => true).ToListAsync()).Count();
            }
            else
            {
                var valueFilter = ".*" + pagination.FilterValue.Valor + ".*";
                var filter = Builders<TDocument>.Filter.Regex(pagination.FilterValue.Propiedad, new MongoDB.Bson.BsonRegularExpression(valueFilter, "i"));

                pagination.Data = await _collection.Find(filter)
                           .Sort(sort)
                           .Skip((pagination.Page - 1) * pagination.PageSize)
                           .Limit(pagination.PageSize)
                           .ToListAsync();

                totalDocuments = (await _collection.Find(filter).ToListAsync()).Count();
            }
            //long totalDocuments = await _collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty);
            var rounded = Math.Ceiling(totalDocuments / Convert.ToDecimal(pagination.PageSize));
            var totalPager = Convert.ToInt32(rounded);
            pagination.PagesQuantity = totalPager;
            pagination.TotalRows = (int)totalDocuments;

            return pagination;
        }
    }
}
