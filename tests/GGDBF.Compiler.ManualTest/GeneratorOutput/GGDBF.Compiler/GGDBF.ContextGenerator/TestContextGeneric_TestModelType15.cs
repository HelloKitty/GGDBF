using TestNamespace2;
using GGDBF;
using System;
using TestNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.64.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType15<TKey> : TestModelType15<TKey>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace.TestModelType Model
        {
            get => TestContextGeneric<TKey>.Instance.TestDatas.ContainsKey(base.ModelId) ? TestContextGeneric<TKey>.Instance.TestDatas[base.ModelId] : default;
        }
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<Int16, TestModelType> _SerializedModels;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType> Models
        {
            get => _SerializedModels != null ? _SerializedModels.Load(TestContextGeneric<TKey>.Instance.TestDatas) : base.Models;
        }
        [DataMemberAttribute(Order = 2)]
        public TestContextGeneric_TestModelType15_TestOwnedTypeModel2<TKey>[] _SerializedOwnedTypeModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestOwnedTypeModel2<TKey>> OwnedTypeModelCollection
        {
            get => _SerializedOwnedTypeModelCollection != null ? _SerializedOwnedTypeModelCollection : base.OwnedTypeModelCollection;
        }
        public TestContextGeneric_TestModelType15() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
            _SerializedModels = GGDBFHelpers.CreateSerializableCollection(m => m.Id, Models);
            _SerializedOwnedTypeModelCollection = converter.Convert<TestNamespace2.TestOwnedTypeModel2<TKey>, TestContextGeneric_TestModelType15_TestOwnedTypeModel2<TKey>>(OwnedTypeModelCollection);
        }
    }

    [GeneratedCodeAttribute("GGDBF", "0.1.64.0")]
    [DataContractAttribute]
    public partial record TestContextGeneric_TestModelType15_TestOwnedTypeModel2<TKey> : TestOwnedTypeModel2<TKey>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace.TestModelType Model
        {
            get => TestContextGeneric<TKey>.Instance.TestDatas.ContainsKey(base.ModelId) ? TestContextGeneric<TKey>.Instance.TestDatas[base.ModelId] : default;
        }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}