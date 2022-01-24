using TestNamespace2;
using System;
using TestNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.43.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType5 : TestModelType5<Int32, TestModelType4, TestModelType, Int16>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace.TestModelType Model
        {
            get => TestContext.Instance.TestDatas[base.ModelId];
        }
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<String, TestModelType4> _SerializedModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType4> ModelCollection
        {
            get => _SerializedModelCollection != null ? _SerializedModelCollection.Load(TestContext.Instance.Test4Datas) : base.ModelCollection;
        }
        public TestContext_TestModelType5() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
            _SerializedModelCollection = GGDBFHelpers.CreateSerializableCollection(m => m.Id, ModelCollection);
        }
    }
}