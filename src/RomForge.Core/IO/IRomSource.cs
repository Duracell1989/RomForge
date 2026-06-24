using System.Collections.Generic;
using System.Threading;

namespace RomForge.Core.IO;

public interface IRomSource
{
    IAsyncEnumerable<RomContent> EnumerateAsync(
        string folderPath,
        CancellationToken cancellationToken = default
    );
}
