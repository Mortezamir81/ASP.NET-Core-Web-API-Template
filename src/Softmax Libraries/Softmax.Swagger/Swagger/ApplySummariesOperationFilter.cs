﻿namespace Softmax.Swagger;

public class ApplySummariesOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var controllerActionDescriptor =
			context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

		if (controllerActionDescriptor == null)
			return;

		var pluralizer = new Pluralizer();

		var actionName =
			controllerActionDescriptor.ActionName;

		var singularizeName =
			pluralizer.Singularize(controllerActionDescriptor.ControllerName);

		var pluralizeName =
			pluralizer.Pluralize(singularizeName);

		var parameterCount =
			operation.Parameters.Where(p => p.Name?.ToLower() != "version" && p.Name?.ToLower() != "api-version").Count();

		if (IsGetAllAction())
		{
			if (string.IsNullOrWhiteSpace(operation.Summary))
				operation.Summary = $"Returns all {pluralizeName}";
		}
		else if (IsActionName("Post", "Create", "Add"))
		{
			if (string.IsNullOrWhiteSpace(operation.Summary))
				operation.Summary = $"Create a {singularizeName}";

			if (operation.Parameters.Count > 0 && string.IsNullOrWhiteSpace(operation.Parameters[0].Description))
				operation.Parameters[0].Description = $"A {singularizeName} representation";
		}
		else if (IsActionName("Read", "Get"))
		{
			if (string.IsNullOrWhiteSpace(operation.Summary))
				operation.Summary = $"Retrieve a {singularizeName} by unique id";

			if (operation.Parameters.Count > 0 && string.IsNullOrWhiteSpace(operation.Parameters[0].Description))
				operation.Parameters[0].Description = $"a unique id for the {singularizeName}";
		}
		else if (IsActionName("Put", "Edit", "Update"))
		{
			if (string.IsNullOrWhiteSpace(operation.Summary))
				operation.Summary = $"Update a {singularizeName} by unique id";

			if (operation.Parameters.Count > 0 && string.IsNullOrWhiteSpace(operation.Parameters[0].Description))
				operation.Parameters[0].Description = $"A {singularizeName} representation";
		}
		else if (IsActionName("Delete", "Remove"))
		{
			if (string.IsNullOrWhiteSpace(operation.Summary))
				operation.Summary = $"Delete a {singularizeName} by unique id";

			if (operation.Parameters.Count > 0 && string.IsNullOrWhiteSpace(operation.Parameters[0].Description))
				operation.Parameters[0].Description = $"A unique id for the {singularizeName}";
		}

		#region Local Functions
		bool IsGetAllAction()
		{
			foreach (var name in new[] { "Get", "Read", "Select" })
			{
				if ((actionName.Equals(name, StringComparison.OrdinalIgnoreCase) && parameterCount == 0) ||
					actionName.Equals($"{name}All", StringComparison.OrdinalIgnoreCase) ||
					actionName.Equals($"{name}{pluralizeName}", StringComparison.OrdinalIgnoreCase) ||
					actionName.Equals($"{name}All{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
					actionName.Equals($"{name}All{pluralizeName}", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}


		bool IsActionName(params string[] names)
		{
			foreach (var name in names)
			{
				if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
					actionName.Equals($"{name}ById", StringComparison.OrdinalIgnoreCase) ||
					actionName.Equals($"{name}{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
					actionName.Equals($"{name}{singularizeName}ById", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
