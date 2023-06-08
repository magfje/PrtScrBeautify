using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PrtScrBeautify;

public interface IModification
{
    Image<Rgba32> Apply(Image<Rgba32> image);
}