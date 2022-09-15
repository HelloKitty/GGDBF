using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.42.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType8 : TestModelType8, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace2.TestModelType7 Model
        {
            get => TestContext.Instance.Test7Datas[new TestModelType7Key(base.ModelId1_Test123, base.ModelId2_Test345)];
        }
        public TestContext_TestModelType8() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}