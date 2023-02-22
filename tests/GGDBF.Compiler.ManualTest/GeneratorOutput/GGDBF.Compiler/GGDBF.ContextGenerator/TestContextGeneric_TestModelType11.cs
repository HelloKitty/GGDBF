using TestNamespace2;
using GGDBF;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.64.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType11<TKey> : TestModelType11<TKey, TKey>, IGGDBFSerializable
    {
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<TestModelType10Key<TKey>, TestModelType10<TKey>> _SerializedModel;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType10<TKey>> Model
        {
            get => _SerializedModel != null ? _SerializedModel.Load(TestContextGeneric<TKey>.Instance.Test10Datas) : base.Model;
        }
        public TestContextGeneric_TestModelType11() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
            _SerializedModel = GGDBFHelpers.CreateSerializableCollection(m => new TestModelType10Key<TKey>(m.Id1, m.Id2), Model);
        }
    }
}