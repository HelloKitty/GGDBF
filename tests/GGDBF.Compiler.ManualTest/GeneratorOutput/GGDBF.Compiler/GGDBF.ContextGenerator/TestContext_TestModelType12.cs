using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.64.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType12 : TestModelType12, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace2.TestModelType2 Model
        {
            get => TestContext.Instance.Test2Datas.ContainsKey(base.ModelId) ? TestContext.Instance.Test2Datas[base.ModelId] : default;
        }
        public TestContext_TestModelType12() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}