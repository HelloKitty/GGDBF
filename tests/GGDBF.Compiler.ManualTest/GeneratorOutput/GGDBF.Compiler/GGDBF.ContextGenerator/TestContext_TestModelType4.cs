using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.31.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType4 : TestModelType4, IGGDBFSerializable
    {
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<String, TestModelType2> _SerializedModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType2> ModelCollection
        {
            get => _SerializedModelCollection != null ? _SerializedModelCollection.Load(TestContext.Instance.Test2Datas) : base.ModelCollection;
        }
        public TestContext_TestModelType4() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
            _SerializedModelCollection = GGDBFHelpers.CreateSerializableCollection(m => m.Id, ModelCollection);
        }
    }
}