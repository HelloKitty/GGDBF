using TestNamespace2;
using GGDBF;
using System;
using TestNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
[DataContractAttribute]
public partial class TestContextGenericConstraints_TestModelType15<TKey, TAnotherType, TAnotherType2> : TestModelType15<TKey>, IGGDBFSerializable where TKey : unmanaged, System.IConvertible
	where TAnotherType : class, System.Enum
	where TAnotherType2 : unmanaged
{[IgnoreDataMemberAttribute]
public override TestNamespace.TestModelType Model 
{ get => TestContextGenericConstraints<TKey,TAnotherType,TAnotherType2>.Instance.TestDatas.TryGetValue(base.ModelId, out var value) ? value : default;
}
[DataMemberAttribute(Order = 1)]
public SerializableGGDBFCollection<Int16, TestModelType> _SerializedModels;

[IgnoreDataMemberAttribute]
public override ICollection<TestModelType> Models 
{ get => _SerializedModels != null ? _SerializedModels.Load(TestContextGenericConstraints<TKey,TAnotherType,TAnotherType2>.Instance.TestDatas) : base.Models;
}
[DataMemberAttribute(Order = 2)]
public TestContextGenericConstraints_TestModelType15_TestOwnedTypeModel2<TKey, TAnotherType, TAnotherType2>[] _SerializedOwnedTypeModelCollection;

[IgnoreDataMemberAttribute]
public override ICollection<TestOwnedTypeModel2<TKey>> OwnedTypeModelCollection 
{ get => _SerializedOwnedTypeModelCollection != null ? _SerializedOwnedTypeModelCollection : base.OwnedTypeModelCollection;
}
public TestContextGenericConstraints_TestModelType15() { }

public void Initialize(IGGDBFDataConverter converter)
{
_SerializedModels = GGDBFHelpers.CreateSerializableCollection(m => m.Id, Models);
_SerializedOwnedTypeModelCollection = converter.Convert<TestNamespace2.TestOwnedTypeModel2<TKey>, TestContextGenericConstraints_TestModelType15_TestOwnedTypeModel2<TKey, TAnotherType, TAnotherType2>>(OwnedTypeModelCollection);
}}

[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
[DataContractAttribute]
public partial record TestContextGenericConstraints_TestModelType15_TestOwnedTypeModel2<TKey, TAnotherType, TAnotherType2> : TestOwnedTypeModel2<TKey>, IGGDBFSerializable where TKey : unmanaged, System.IConvertible
	where TAnotherType : class, System.Enum
	where TAnotherType2 : unmanaged
{[IgnoreDataMemberAttribute]
public override TestNamespace.TestModelType Model 
{ get => TestContextGenericConstraints<TKey,TAnotherType,TAnotherType2>.Instance.TestDatas.TryGetValue(base.ModelId, out var value) ? value : default;
}

public void Initialize(IGGDBFDataConverter converter)
{
}}
}