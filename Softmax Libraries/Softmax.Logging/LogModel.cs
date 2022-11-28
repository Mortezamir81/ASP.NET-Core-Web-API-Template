using System.Collections.Generic;
using System.Text.Json;

namespace Dtat.Logging
{
	public class LogModel : object
	{
		public LogModel() : base()
		{
		}

		public string LogLevel { get; set; }

		public string ApplicationName { get; set; }


		public string Namespace { get; set; }

		public string ClassName { get; set; }

		public string MethodName { get; set; }



		public string LocalIP { get; set; }

		public string LocalPort { get; set; }

		public string RemoteIP { get; set; }

		public string Username { get; set; }

		public string RequestPath { get; set; }

		public string HttpReferrer { get; set; }



		public string Message { get; set; }

		public List<object> Parameters { get; set; }

		public List<object> Exceptions { get; set; }


		public override string ToString()
		{
			var json = 
				JsonSerializer.Serialize(this);

			return json;
		}
	}
}
