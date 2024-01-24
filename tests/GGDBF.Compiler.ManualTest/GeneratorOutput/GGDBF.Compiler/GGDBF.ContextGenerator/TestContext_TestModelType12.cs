using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{[GeneratedCodeAttribute("GGDBF", "0.5.80.0")]
[DataContractAttribute]
public partial class TestContext_TestModelType12 : TestModelType12, IGGDBFSerializable
{[IgnoreDataMemberAttribute]
public override TestNamespace2.TestModelType2 Model 
{ get => TestContext.Instance.Test2Datas.TryGetValue(base.ModelId, out var value) ? value : default;
}
public TestContext_TestModelType12() { }

public void Initialize(IGGDBFDataConverter converter)
{
}}
}