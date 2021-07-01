using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.26.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType11 : TestModelType11<Int32, Int16>, IGGDBFSerializable
    {
        public TestContext_TestModelType11() { }

        public void Initialize()
        {
        }
    }
}