﻿using TestNamespace2;
using GGDBF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
[DataContractAttribute]
public partial class TestContextGenericConstraints_TestModelType9<TKey, TAnotherType, TAnotherType2> : TestModelType9<TKey,TAnotherType>, IGGDBFSerializable where TKey : unmanaged, System.IConvertible
	where TAnotherType : class, System.Enum
	where TAnotherType2 : unmanaged
{[IgnoreDataMemberAttribute]
public override TestNamespace2.TestModelType4 Model 
{ get => TestContextGenericConstraints<TKey,TAnotherType,TAnotherType2>.Instance.Test4Datas.TryGetValue(base.ModelId, out var value) ? value : default;
}
public TestContextGenericConstraints_TestModelType9() { }

public void Initialize(IGGDBFDataConverter converter)
{
}}
}