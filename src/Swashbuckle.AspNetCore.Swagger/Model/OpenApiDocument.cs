using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Swashbuckle.AspNetCore.Swagger
{
    public class OpenApiDocument : Document
    {
        public OpenApiDocument()
        {
            Extensions = new Dictionary<string, object>();
        }

        public string Openapi => "3.0.0";

        public Info Info { get; set; }

        public string BasePath { get; set; }

        public IList<string> Schemes { get; set; }

        public IList<string> Consumes { get; set; }

        public IList<string> Produces { get; set; }

        public IDictionary<string, PathItem> Paths { get; set; }

        public IDictionary<string, Schema> Definitions { get; set; }

        public IDictionary<string, IParameter> Parameters { get; set; }

        public IDictionary<string, Response> Responses { get; set; }

        public IDictionary<string, SecurityScheme> SecurityDefinitions { get; set; }

        public IList<IDictionary<string, IEnumerable<string>>> Security { get; set; }

        public IList<Tag> Tags { get; set; }

        public ExternalDocs ExternalDocs { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Extensions { get; private set; }
    }
}
