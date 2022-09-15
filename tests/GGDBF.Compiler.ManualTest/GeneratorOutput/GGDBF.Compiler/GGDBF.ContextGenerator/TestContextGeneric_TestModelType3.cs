using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.56.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType3<TKey> : TestModelType3, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace2.TestModelType2 Model
        {
            get => TestContextGeneric<TKey>.Instance.Test2Datas[base.ModelId];
        }
        public TestContextGeneric_TestModelType3() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}