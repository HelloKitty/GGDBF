using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.43.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType17 : TestModelType17, IGGDBFSerializable
    {
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<TestModelType18Key, TestModelType18> _SerializedSubClasses;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType18> SubClasses
        {
            get => _SerializedSubClasses != null ? _SerializedSubClasses.Load(TestContext.Instance.Test18Datas) : base.SubClasses;
        }
        public TestContext_TestModelType17() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
            _SerializedSubClasses = GGDBFHelpers.CreateSerializableCollection(m => new TestModelType18Key(m.Id1, m.Id2), SubClasses);
        }
    }
}