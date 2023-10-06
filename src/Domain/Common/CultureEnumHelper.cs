namespace Domain.Common;

public static class CultureEnumHelper : object
{
	static CultureEnumHelper()
	{
	}

	public static System.Globalization.CultureInfo GetByLcid(Enums.CultureEnum lcid)
	{
		switch (lcid)
		{
			case Enums.CultureEnum.English:
			{
				var cultureInfo =
					new System.Globalization.CultureInfo(name: "en-US");

				return cultureInfo;
			}

			case Enums.CultureEnum.Persian:
			{
				var cultureInfo =
					new System.Globalization.CultureInfo(name: "fa-IR");

				return cultureInfo;
			}

			default:
			{
				var cultureInfo =
					new System.Globalization.CultureInfo(name: "en-US");

				return cultureInfo;
			}
		}
	}

	public static Enums.CultureEnum GetCurrentUICultureLcid()
	{
		var currentUICultureLcid =
			System.Globalization.CultureInfo.CurrentUICulture.LCID;

		var result =
			(Enums.CultureEnum) currentUICultureLcid;

		return result;
	}

	public static string GetCurrentUICultureName()
	{
		var result =
			System.Globalization.CultureInfo.CurrentUICulture.Name;

		return result;
	}
}
