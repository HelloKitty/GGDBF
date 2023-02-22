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
    public partial class TestContext_TestModelType18 : TestModelType18, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace2.TestModelType4 Model
        {
            get => TestContext.Instance.Test4Datas.ContainsKey(base.ModelId) ? TestContext.Instance.Test4Datas[base.ModelId] : default;
        }
        [IgnoreDataMemberAttribute]
        public override TestNamespace2.TestModelType17 Model17
        {
            get => TestContext.Instance.Test17Datas.ContainsKey(base.Model17Id) ? TestContext.Instance.Test17Datas[base.Model17Id] : default;
        }
        public TestContext_TestModelType18() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}