using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGDBF
{
	public static class GGDBFHelpers
	{
		public static SerializableGGDBFCollection<TKeyType, TModelType> CreateSerializableCollection<TKeyType, TModelType>(Func<TModelType, TKeyType> keyResolutionFunction, IEnumerable<TModelType> models)
		{
			if (keyResolutionFunction == null) throw new ArgumentNullException(nameof(keyResolutionFunction));
			if (models == null) throw new ArgumentNullException(nameof(models));

			//Single enumeration
			TModelType[] modelsArray = models as TModelType[] ?? models.ToArray();
			TKeyType[] keys = new TKeyType[modelsArray.Length];
			
			//Initialize the keys in an efficient manner
			for (int i = 0; i < modelsArray.Length; i++)
				keys[i] = keyResolutionFunction(modelsArray[i]);

			return new SerializableGGDBFCollection<TKeyType, TModelType>(keys);
		}
	}
}
