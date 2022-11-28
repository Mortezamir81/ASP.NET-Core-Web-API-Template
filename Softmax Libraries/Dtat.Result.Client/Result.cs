using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtat.Results.Client
{
	public class Result
	{
		public bool IsFailed { get; set; }

		public bool IsSuccess { get; set; }

		public List<string> Errors { get; set; }

		public List<string> Successes { get; set; }
	}
}
