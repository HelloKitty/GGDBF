using TestNamespace2;
using GGDBF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.64.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType13<TKey> : TestModelType13<TKey, TKey>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace2.TestModelType10<TKey> Model
        {
            get => TestContextGeneric<TKey>.Instance.Test10Datas.ContainsKey(new TestModelType10Key<TKey>(base.ModelId1, base.ModelId2)) ? TestContextGeneric<TKey>.Instance.Test10Datas[new TestModelType10Key<TKey>(base.ModelId1, base.ModelId2)] : default;
        }
        public TestContextGeneric_TestModelType13() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}