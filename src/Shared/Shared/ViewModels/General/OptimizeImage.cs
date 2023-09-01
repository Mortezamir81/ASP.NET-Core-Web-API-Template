using Softmax.Utilities.Image;

namespace ViewModels.General;

public class OptimizeImageViewModel
{
	public OptimizeImageViewModel()
	{
		OptimizeImage = new OptimizeImage();
	}

	public OptimizeImage OptimizeImage { get; set; }
}

public class OptimizeImage
{
	private int _quality = 75;
	private ImageExtensions _imageExtensions = Softmax.Utilities.Image.ImageExtensions.jpg;
	private int _width = 1920;

	public int? Width
	{
		get => _width;
		set => _width = value ?? _width;
	}

	public int? Quality
	{
		get => _quality;
		set => _quality = value ?? _quality;
	}

	public ImageExtensions? ImageExtensions
	{
		get => _imageExtensions;
		set => _imageExtensions = value ?? _imageExtensions;
	}
}