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
    [GeneratedCodeAttribute("GGDBF", "0.1.32.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType15 : TestModelType15<Int32>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace.TestModelType Model
        {
            get => TestContext.Instance.TestDatas[base.ModelId];
        }
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<Int16, TestModelType> _SerializedModels;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType> Models
        {
            get => _SerializedModels != null ? _SerializedModels.Load(TestContext.Instance.TestDatas) : base.Models;
        }
        [DataMemberAttribute(Order = 2)]
        public TestContext_TestModelType15_TestOwnedTypeModel2[] _SerializedOwnedTypeModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestOwnedTypeModel2<Int32>> OwnedTypeModelCollection
        {
            get => _SerializedOwnedTypeModelCollection != null ? _SerializedOwnedTypeModelCollection : base.OwnedTypeModelCollection;
        }
        public TestContext_TestModelType15() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
            _SerializedModels = GGDBFHelpers.CreateSerializableCollection(m => m.Id, Models);
            _SerializedOwnedTypeModelCollection = converter.Convert<TestNamespace2.TestOwnedTypeModel2<Int32>, TestContext_TestModelType15_TestOwnedTypeModel2>(OwnedTypeModelCollection);
        }
    }

    [GeneratedCodeAttribute("GGDBF", "0.1.32.0")]
    [DataContractAttribute]
    public partial record TestContext_TestModelType15_TestOwnedTypeModel2 : TestOwnedTypeModel2<Int32>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace.TestModelType Model
        {
            get => TestContext.Instance.TestDatas[base.ModelId];
        }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}