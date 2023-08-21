namespace Softmax.Utilities.Image;

public enum ImageExtensions : int
{
	jpg,
	png,
}

public static class ImageExtension
{
	public static string ToFixedString(this ImageExtensions imageExtensions)
	{
		return imageExtensions switch
		{
			ImageExtensions.jpg => nameof(ImageExtensions.jpg),
			ImageExtensions.png => nameof(ImageExtensions.png),
			_ => nameof(ImageExtensions.jpg),
		};
	}

	public static string ToFixedString(this ImageExtensions? imageExtensions)
	{
		return imageExtensions switch
		{
			ImageExtensions.jpg => nameof(ImageExtensions.jpg),
			ImageExtensions.png => nameof(ImageExtensions.png),
			_ => nameof(ImageExtensions.jpg),
		};
	}
}