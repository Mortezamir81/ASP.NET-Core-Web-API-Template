namespace Softmax.Utilities.Image;

public interface IImageOptimizer
{
	Stream Optimize(Stream stream, OptimizeSettings? optimizeSettings);
}
