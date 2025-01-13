using SkiaSharp;
using System.IO;

namespace Softmax.Utilities.Image;

public class ImageOptimizer : IImageOptimizer
{
	private readonly static int _defaultWidth = 1920;
	private readonly static int _defaultQuality = 80;

	public ImageOptimizer()
	{
	}

	public Stream Optimize
		(Stream stream, OptimizeSettings? optimizeSettings)
	{
		if (stream == null)
			throw new NullReferenceException("imageStream");

		optimizeSettings ??= new OptimizeSettings();

		using var originalBitmap = SKBitmap.Decode(stream);

		var finalWidth =
			optimizeSettings.Width < originalBitmap.Width ?
			optimizeSettings.Width ?? _defaultWidth :
			originalBitmap.Width;

		var finalQuality = optimizeSettings.Quality ?? _defaultQuality;

		if (!optimizeSettings.Height.HasValue)
			optimizeSettings.Height = originalBitmap.Height * finalWidth / originalBitmap.Width;

		using var resizedBitmap = originalBitmap.Resize(
			new SKImageInfo(finalWidth, optimizeSettings.Height.Value), SKSamplingOptions.Default
		);

		using var newImage = SKImage.FromBitmap(resizedBitmap);

		var finalStream = new MemoryStream();

		try
		{
			var format = optimizeSettings.ImageExtensions switch
			{
				ImageExtensions.jpg => SKEncodedImageFormat.Jpeg,
				ImageExtensions.png => SKEncodedImageFormat.Png,
				ImageExtensions.webp => SKEncodedImageFormat.Webp,
				_ => SKEncodedImageFormat.Jpeg,
			};

			using var skiaData =
				newImage.Encode(format, finalQuality);

			skiaData.SaveTo(finalStream);
		}
		catch (Exception)
		{
			finalStream?.Dispose();
			throw;
		}

		if (finalStream.Length >= stream.Length)
		{
			finalStream?.Dispose();
			return stream;
		}

		return finalStream;
	}
}
