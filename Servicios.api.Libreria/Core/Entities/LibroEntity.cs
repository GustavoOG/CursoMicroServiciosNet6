using MongoDB.Bson.Serialization.Attributes;

namespace Servicios.api.Libreria.Core.Entities
{

    [BsonCollectionAtrribute("Libro")]
    public class LibroEntity : Document
    {
        [BsonElement("titulo")]
        public string Titulo { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("precio")]
        public Decimal Precio { get; set; }

        [BsonElement("fechaPublicacion")]
        public DateTime FechaPublicacion { get; set; }

        [BsonElement("autor")]
        public AutorEntity Autor { get; set; }
    }
}
