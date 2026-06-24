using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace RomForge.Core.IO;

public interface IRomFileOperations
{
    Task<Result> RenameAsync(string from, string to, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string path, CancellationToken cancellationToken = default);
    Task<Result> TruncateAsync(string path, long length, CancellationToken cancellationToken = default);
}
