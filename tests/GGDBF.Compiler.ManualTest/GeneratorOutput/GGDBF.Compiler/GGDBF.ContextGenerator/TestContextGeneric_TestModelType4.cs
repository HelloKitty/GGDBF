using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.20.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType4<TKey> : TestModelType4, IGGDBFSerializable
    {
        public TestContextGeneric_TestModelType4() { }

        public void Initialize()
        {
        }
    }
}