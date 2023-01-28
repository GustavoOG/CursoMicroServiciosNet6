namespace Servicios.api.Libreria.Core.Entities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAtrribute : Attribute
    {
        public string CollectionName { get; set; }

        public BsonCollectionAtrribute(string collectionName)
        {
            CollectionName = collectionName;
        }


    }
}
