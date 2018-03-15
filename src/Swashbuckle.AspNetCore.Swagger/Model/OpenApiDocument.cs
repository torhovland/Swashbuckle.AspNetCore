using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Swashbuckle.AspNetCore.Swagger
{
    public class OpenApiDocument
    {
        public string Openapi => "3.0.0";

        public bool ShouldSerializeSwagger()
        {
            return false;
        }

        public bool ShouldSerializeHost()
        {
            return false;
        }
    }
}
