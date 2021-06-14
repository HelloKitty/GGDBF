using TestNamespace2;
using System;
using TestNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.1.0")]
    public partial class TestContext_TestModelType5 : TestModelType5<Int32, TestModelType4, TestModelType, Int16>
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType Model
        {
            get => TestContext.Instance.TestDatas[base.ModelId];
        }
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<string, TestModelType4> _ModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType4> ModelCollection
        {
            get => _ModelCollection.Load(TestContext.Instance.Test4Datas);
        }
    }
}