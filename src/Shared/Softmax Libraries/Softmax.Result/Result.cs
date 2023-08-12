using Dtat;
using System.Collections.Generic;

namespace Softmax.Results;

public class Result : object
{
	public Result() : base()
	{
		IsSuccess = true;

		MessageCode = (int) Results.MessageCode.HttpSuccessCode;

		Messages =
			new List<string>();
	}

	public Result(bool isSuccess, int messageCode, List<string> messages) : base()
	{
		IsSuccess = isSuccess;

		MessageCode = messageCode;

		Messages = messages;
	}

	public bool IsSuccess { get; set; }

	public int MessageCode { get; set; }

	public List<string> Messages { get; set; }

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

		MessageCode = (int) Results.MessageCode.HttpBadRequestCode;

		IsSuccess = false;
	}

	public void AddErrorMessage(string message, MessageCode messageCode)
	{
		AddErrorMessage(message);

		MessageCode = (int) messageCode;
	}

	public void AddErrorMessage(string message, int messageCode)
	{
		AddErrorMessage(message);

		MessageCode = messageCode;
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

		MessageCode = (int) Results.MessageCode.HttpSuccessCode;

		IsSuccess = true;
	}

	public void AddSuccessMessage(string message, MessageCode messageCode)
	{
		AddSuccessMessage(message);

		MessageCode = (int) messageCode;
	}

	public void AddSuccessMessage(string message, int messageCode)
	{
		AddSuccessMessage(message);

		MessageCode = messageCode;
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
