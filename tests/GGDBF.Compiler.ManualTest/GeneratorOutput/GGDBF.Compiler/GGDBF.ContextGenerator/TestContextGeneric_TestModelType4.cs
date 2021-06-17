using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.8.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType4<TKey> : TestModelType4, IGGDBFSerializable
    {
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<string, TestModelType2> _SerializedModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType2> ModelCollection
        {
            get => _SerializedModelCollection != null ? _SerializedModelCollection.Load(TestContextGeneric<TKey>.Instance.Test2Datas) : base.ModelCollection;
        }
        public TestContextGeneric_TestModelType4() { }

        public void Initialize()
        {
            _SerializedModelCollection = GGDBFHelpers.CreateSerializableCollection(m => m.Id, ModelCollection);
        }
    }
}