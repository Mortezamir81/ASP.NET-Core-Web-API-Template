namespace Dtat.Result
{
	public class Result : object
	{
		public Result() : base()
		{
			IsSuccess = true;

			MessageCode = (int) MessageCodes.HttpSuccessCode;

			Messages =
				new System.Collections.Generic.List<string>();
		}

		public bool IsSuccess { get; set; }

		public int MessageCode { get; set; }

		public System.Collections.Generic.List<string> Messages { get; }

		public void AddErrorMessage(string message)
		{
			message =
				String.Fix(text: message);

			if (message == null)
			{
				return;
			}

			if (Messages.Contains(message))
			{
				return;
			}

			Messages.Add(message);

			MessageCode = (int) MessageCodes.HttpBadRequestCode;

			IsSuccess = false;
		}

		public void AddErrorMessage(string message, MessageCodes messageCodes)
		{
			AddErrorMessage(message);

			MessageCode = (int) messageCodes;
		}

		public void AddErrorMessage(string message, int messageCodes)
		{
			AddErrorMessage(message);

			MessageCode = messageCodes;
		}


		public void AddSuccessMessage(string message)
		{
			message =
				String.Fix(text: message);

			if (message == null)
			{
				return;
			}

			if (Messages.Contains(message))
			{
				return;
			}

			Messages.Add(message);

			MessageCode = (int) MessageCodes.HttpSuccessCode;

			IsSuccess = true;
		}

		public void AddSuccessMessage(string message, MessageCodes messageCodes)
		{
			AddSuccessMessage(message);

			MessageCode = (int) messageCodes;
		}

		public void AddSuccessMessage(string message, int messageCodes)
		{
			AddSuccessMessage(message);

			MessageCode = messageCodes;
		}


		public void RemoveMessage(string message)
		{
			message =
				String.Fix(text: message);

			if (message == null)
			{
				return;
			}

			Messages.Remove(message);

			if (Messages.Count == 0)
			{
				IsSuccess = true;
			}
		}

		public void ClearMessages()
		{
			IsSuccess = true;

			Messages.Clear();
		}
	}
}
