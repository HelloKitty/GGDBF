using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.19.0")]
    [DataContractAttribute]
    public partial class TestContextGenericConstraints_TestModelType4<TKey, TAnotherType, TAnotherType2> : TestModelType4, IGGDBFSerializable where TKey : unmanaged, System.IConvertible
       where TAnotherType : class, System.Enum
       where TAnotherType2 : unmanaged
    {
        public TestContextGenericConstraints_TestModelType4() { }

        public void Initialize()
        {
        }
    }
}