namespace Dtat.Results.Server
{
	public class Result : object
	{
		public Result() : base()
		{
			IsSuccess = true;
			IsFailed = false;

			Errors =
				new System.Collections.Generic.List<string>();

			Successes =
				new System.Collections.Generic.List<string>();
		}

		public bool IsFailed { get; set; }

		public bool IsSuccess { get; set; }

		public System.Collections.Generic.List<string> Errors {	get; }

		public System.Collections.Generic.List<string> Successes { get; }

		public void AddErrorMessage(string message)
		{
			message =
				String.Fix(text: message);

			if (message == null)
			{
				return;
			}

			if (Errors.Contains(message))
			{
				return;
			}

			Errors.Add(message);

			IsFailed = true;
			IsSuccess = false;
		}

		public void RemoveErrorMessage(string message)
		{
			message =
				String.Fix(text: message);

			if (message == null)
			{
				return;
			}

			Errors.Remove(message);

			if (Errors.Count == 0)
			{
				IsFailed = false;
				IsSuccess = true;
			}
		}

		public void ClearErrorMessages()
		{
			IsFailed = false;
			IsSuccess = true;

			Errors.Clear();
		}

		public void AddSuccessMessage(string message)
		{
			message =
				String.Fix(text: message);

			if (message == null)
			{
				return;
			}

			if (Successes.Contains(message))
			{
				return;
			}

			IsSuccess = true;
			Successes.Add(message);
		}

		public void RemoveSuccessMessage(string message)
		{
			message =
				String.Fix(text: message);

			if (message == null)
			{
				return;
			}

			Successes.Remove(message);
		}

		public void ClearSuccessMessages()
		{
			Successes.Clear();
		}

		public void ClearAllMessages()
		{
			ClearErrorMessages();
			ClearSuccessMessages();
		}
	}
}
