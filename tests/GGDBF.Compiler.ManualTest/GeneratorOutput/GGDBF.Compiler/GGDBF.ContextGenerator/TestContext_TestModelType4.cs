using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.1.0")]
    public partial class TestContext_TestModelType4 : TestModelType4, IGGDBFSerializable
    {
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<string, TestModelType2> _ModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType2> ModelCollection
        {
            get => _ModelCollection != null ? _ModelCollection.Load(TestContext.Instance.Test2Datas) : base.ModelCollection;
        }

        public void Initialize()
        {
            _ModelCollection = GGDBFHelpers.CreateSerializableCollection(m => m.Id, ModelCollection);
        }
    }
}