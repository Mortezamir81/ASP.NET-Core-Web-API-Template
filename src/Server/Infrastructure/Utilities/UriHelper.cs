namespace Infrastructure.Utilities;

public class UriHelper
{
	public static string CombineUri(string baseUri, string query, string[]? segments = null)
	{
		var uriBuilder = new StringBuilder(baseUri.TrimEnd('/'));

		if (segments?.Length > 0)
		{
			foreach (var segment in segments)
				uriBuilder.Append('/').Append(segment.Trim('/'));
		}

		if (!string.IsNullOrWhiteSpace(query))
			uriBuilder.Append('?').Append(query.TrimStart('?'));

		return uriBuilder.ToString();
	}
}
