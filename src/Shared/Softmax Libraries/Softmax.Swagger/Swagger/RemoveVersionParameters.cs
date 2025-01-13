using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softmax.Swagger;

internal class SetDefaultValueForVersionHeader : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var versionParameters = 
			operation.Parameters
			.Where(p => p.Name == "X-Version")
			.ToList();

		var currentVersion =
			context.ApiDescription.GroupName?.Split('v')[1];

		versionParameters.ForEach(versionParameter =>
		{
			versionParameter.Schema.Default = new OpenApiString($"{currentVersion}.0");
		});
	}
}
