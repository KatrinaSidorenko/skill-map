// <copyright file="GzipUtil.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IO.Compression;
using System.Text;
using SkillMap.Shared.Extensions;

namespace SkillMap.Shared.Gzip;

public static class GzipUtils
{
    public static async Task<byte[]> Decompress(this byte[] gzip, CancellationToken ct)
    {
        using (var stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
        {
            const int size = 4096 * 8;
            var buffer = new byte[size];
            using (var memory = new MemoryStream())
            {
                var count = 0;
                do
                {
                    count = await stream.ReadAsync(buffer, 0, size, ct);
                    if (count > 0)
                    {
                        await memory.WriteAsync(buffer, 0, count, ct);
                    }
                }
                while (count > 0);

                return memory.ToArray();
            }
        }
    }

    public static async Task<byte[]> Compress(this byte[] data, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
        {
            await gzip.WriteAsync(data, ct);
            await gzip.FlushAsync(ct);
        }

        ms.Position = 0;
        return ms.ToArray();
    }

    public static async Task<byte[]> GzipJsonObjectUtf8(this object o, CancellationToken ct)
    {
        var json = o.SerializeOrDefault();
        var bytes = Encoding.UTF8.GetBytes(json);
        return await bytes.Compress(ct);
    }

    public static async Task<T> InGzipJsonObjectUtf8<T>(this byte[] gzipped, CancellationToken ct)
    {
        var decompressed = await gzipped.Decompress(ct);
        var json = Encoding.UTF8.GetString(decompressed);
        return json.DeserializeOrDefault<T>();
    }
}