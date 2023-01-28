using MongoDB.Bson.Serialization.Attributes;

namespace Servicios.api.Libreria.Core.Entities
{
    [BsonCollectionAtrribute("Empleado")]

    public class EmpleadoEntity : Document
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; }
    }
}
