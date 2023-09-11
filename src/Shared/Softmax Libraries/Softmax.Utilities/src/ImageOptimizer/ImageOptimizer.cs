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

		optimizeSettings ??= new OptimizeSettings();

		using var managedStream =
			new SKManagedStream(stream);

		using var originalBitmap = SKBitmap.Decode(managedStream);

		var finalWidth =
			optimizeSettings.Width < originalBitmap.Width ?
			optimizeSettings.Width ?? _defaultWidth :
			originalBitmap.Width;

		var finalQuality = optimizeSettings.Quality ?? _defaultQuality;

		if (!optimizeSettings.Height.HasValue)
			optimizeSettings.Height = originalBitmap.Height * finalWidth / originalBitmap.Width;

		using var resizedBitmap =
			originalBitmap.Resize(new SKImageInfo(finalWidth, optimizeSettings.Height.Value), SKFilterQuality.Medium);

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

		if (finalStream.Length > stream.Length)
			return stream;

		return finalStream;
	}
}
