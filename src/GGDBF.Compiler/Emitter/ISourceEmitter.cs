﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	public interface ISourceEmitter
	{
		void Emit(StringBuilder builder);
	}
}
