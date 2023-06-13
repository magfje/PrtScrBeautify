using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PrtScrBeautify;

public class Beautify
{
    private readonly List<IModification> _modList;

    public Beautify(List<IModification> modList)
    {
        _modList = modList;
    }

    public Image<Rgba32> ApplyModifications(Image<Rgba32> image)
    {
        //return _modList.Aggregate(image, (current, mod) => mod.Apply(current))

        foreach (var mod in _modList)
            image = mod.Apply(image);

        return image;
    }
}