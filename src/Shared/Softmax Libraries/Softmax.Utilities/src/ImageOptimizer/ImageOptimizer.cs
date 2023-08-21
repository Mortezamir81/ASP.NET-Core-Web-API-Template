using SkiaSharp;

namespace Softmax.Utilities.Image;

public class ImageOptimizer : IImageOptimizer
{
	private readonly int _defaultWidth = 1920;
	private readonly int _defaultQuality = 75;

	public ImageOptimizer()
	{
	}

	public Stream Optimize
		(Stream stream, OptimizeSettings? optimizeSettings)
	{
		if (stream == null)
			throw new NullReferenceException("imageStream");

		if (optimizeSettings == null)
			optimizeSettings = new OptimizeSettings();

		using var managedStream =
			new SKManagedStream(stream);

		using var originalBitmap = SKBitmap.Decode(managedStream);

		var finalWidth =
			optimizeSettings.Width < originalBitmap.Width ?
			optimizeSettings.Width ?? _defaultWidth :
			originalBitmap.Width;

		var finalQuality = optimizeSettings.Quality ?? _defaultQuality;

		if (!optimizeSettings.Height.HasValue)
		{
			optimizeSettings.Height = originalBitmap.Height * finalWidth / originalBitmap.Width;
		}

		using var resizedBitmap =
			originalBitmap.Resize(new SKImageInfo(finalWidth, optimizeSettings.Height.Value), SKFilterQuality.Medium);

		using var newImage = SKImage.FromBitmap(resizedBitmap);

		var finalStream = new MemoryStream();

		try
		{
			switch (optimizeSettings.ImageExtensions)
			{
				case ImageExtensions.jpg:
				{
					using var skiaData =
						newImage.Encode(SKEncodedImageFormat.Jpeg, finalQuality);

					skiaData.SaveTo(finalStream);

					break;
				}

				case ImageExtensions.png:
				{
					using var skiaData =
						newImage.Encode(SKEncodedImageFormat.Png, finalQuality);

					skiaData.SaveTo(finalStream);

					break;
				}

				default:
				{
					using var skiaData =
						newImage.Encode(SKEncodedImageFormat.Jpeg, finalQuality);

					skiaData.SaveTo(finalStream);

					break;
				}
			}

		}
		catch
		{
			finalStream.Dispose();

			finalStream = null;

			throw;
		}

		if (finalStream.Length > stream.Length)
		{
			finalStream.Dispose();

			finalStream = null;

			return stream;
		}

		return finalStream;
	}
}
