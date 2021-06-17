﻿using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.7.0")]
    [DataContractAttribute]
    public partial class TestContextGenericConstraints_TestModelType4<TKey> : TestModelType4, IGGDBFSerializable where TKey : unmanaged, System.IConvertible
    {
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<string, TestModelType2> _SerializedModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType2> ModelCollection
        {
            get => _SerializedModelCollection != null ? _SerializedModelCollection.Load(TestContextGenericConstraints<TKey>.Instance.Test2Datas) : base.ModelCollection;
        }
        public TestContextGenericConstraints_TestModelType4() { }

        public void Initialize()
        {
            _SerializedModelCollection = GGDBFHelpers.CreateSerializableCollection(m => m.Id, ModelCollection);
        }
    }
}