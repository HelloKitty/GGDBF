﻿using TestNamespace2;
using GGDBF;
using TestNamespace;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.5.0")]
    [DataContractAttribute]
    public partial class TestContextGeneric_TestModelType5<TKey> : TestModelType5<TKey, TestModelType4, TestModelType, Int16>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType Model
        {
            get => TestContextGeneric<TKey>.Instance.TestDatas[base.ModelId];
        }
        [DataMemberAttribute(Order = 1)]
        public SerializableGGDBFCollection<string, TestModelType4> _SerializedModelCollection;

        [IgnoreDataMemberAttribute]
        public override ICollection<TestModelType4> ModelCollection
        {
            get => _SerializedModelCollection != null ? _SerializedModelCollection.Load(TestContextGeneric<TKey>.Instance.Test4Datas) : base.ModelCollection;
        }
        public TestContextGeneric_TestModelType5() { }

        public void Initialize()
        {
            _SerializedModelCollection = GGDBFHelpers.CreateSerializableCollection(m => m.Id, ModelCollection);
        }
    }
}