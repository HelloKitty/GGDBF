using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.1.0")]
    public partial class TestContext_TestModelType3 : TestModelType3
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType2 Model
        {
            get => TestContext.Instance.Test2Datas[base.ModelId];
        }
    }
}