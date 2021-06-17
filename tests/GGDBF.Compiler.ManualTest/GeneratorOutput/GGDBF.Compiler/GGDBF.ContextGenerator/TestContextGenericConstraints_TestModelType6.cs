using TestNamespace2;
using GGDBF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.7.0")]
    [DataContractAttribute]
    public partial class TestContextGenericConstraints_TestModelType6<TKey> : TestModelType6<TKey>, IGGDBFSerializable where TKey : unmanaged, System.IConvertible
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType4 Model
        {
            get => TestContextGenericConstraints<TKey>.Instance.Test4Datas[base.ModelId];
        }
        public TestContextGenericConstraints_TestModelType6() { }

        public void Initialize()
        {
        }
    }
}