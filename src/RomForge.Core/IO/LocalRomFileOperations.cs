using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace RomForge.Core.IO;

public sealed class LocalRomFileOperations : IRomFileOperations
{
    public Task<Result> RenameAsync(
        string from,
        string to,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            File.Move(from, to, overwrite: false);
            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Task.FromResult(Result.Fail(new ExceptionalError(ex)));
        }
    }

    public Task<Result> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            File.Delete(path);
            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Task.FromResult(Result.Fail(new ExceptionalError(ex)));
        }
    }

    public Task<Result> TruncateAsync(
        string path,
        long length,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
            fs.SetLength(length);
            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Task.FromResult(Result.Fail(new ExceptionalError(ex)));
        }
    }
}
