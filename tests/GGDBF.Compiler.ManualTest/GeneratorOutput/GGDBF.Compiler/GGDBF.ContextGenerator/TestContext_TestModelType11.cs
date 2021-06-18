using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.11.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType11 : TestModelType11<Int32, Int16>, IGGDBFSerializable
    {
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<TestModelType10Key<Int32>, TestModelType10<Int32>> _SerializedModel;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType10<Int32>> Model
        {
            get => _SerializedModel != null ? _SerializedModel.Load(TestContext.Instance.Test10Datas) : base.Model;
        }
        public TestContext_TestModelType11() { }

        public void Initialize()
        {
            _SerializedModel = GGDBFHelpers.CreateSerializableCollection(m => new TestModelType10Key<Int32>(m.Id1, m.Id2), Model);
        }
    }
}