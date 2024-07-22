using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
[DataContractAttribute]
public partial class TestContext_TestModelType18 : TestModelType18, IGGDBFSerializable
{[IgnoreDataMemberAttribute]
public override TestNamespace2.TestModelType4 Model 
{ get => TestContext.Instance.Test4Datas.TryGetValue(base.ModelId, out var value) ? value : default;
}
[IgnoreDataMemberAttribute]
public override TestNamespace2.TestModelType17 Model17 
{ get => TestContext.Instance.Test17Datas.TryGetValue(base.Model17Id, out var value) ? value : default;
}
public TestContext_TestModelType18() { }

public void Initialize(IGGDBFDataConverter converter)
{
}}
}