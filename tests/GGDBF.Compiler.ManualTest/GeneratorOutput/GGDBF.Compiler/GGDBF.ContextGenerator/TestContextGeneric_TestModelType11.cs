using TestNamespace2;
using GGDBF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.26.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType11<TKey> : TestModelType11<TKey, TKey>, IGGDBFSerializable
    {
        public TestContextGeneric_TestModelType11() { }

        public void Initialize()
        {
        }
    }
}