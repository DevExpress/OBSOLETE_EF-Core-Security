using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Serialize.Linq.Serializers;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class ExternalCriteriaSerializer {
        public enum SerializerType { XmlSerializer, JsonSerializer };
        private ExpressionSerializer serializer;
        public ExternalCriteriaSerializer() {
            serializer = new ExpressionSerializer(new XmlSerializer());
        }
        public ExternalCriteriaSerializer(SerializerType serializerType) {
            switch(serializerType) {
                case SerializerType.XmlSerializer:
                    serializer = new ExpressionSerializer(new XmlSerializer());
                    break;
                case SerializerType.JsonSerializer:
                    serializer = new ExpressionSerializer(new JsonSerializer());
                    break;
                default:
                    serializer = new ExpressionSerializer(new XmlSerializer());
                    break;
            }            
        }
        public string Serialize(Expression criteria) {
            return serializer.SerializeText(criteria);
        }
        public Expression Deserialize(string serializedCriteria) {
            return serializer.DeserializeText(serializedCriteria);
        }
    }
}
