using System.Diagnostics;

if (args.Length < 2)
{
    Console.WriteLine("Usage: BinaryCompare.exe <file1> <file2> [BufferSizeInMB] [--continue]");
    Console.WriteLine("Example: BinaryCompare.exe file1.bin file2.bin 64 --continue");
    return 1;
}

string file1 = args[0];
string file2 = args[1];
int bufferSizeMB = 64; // 64MB Default
bool continueOnMismatch = false;

// Parse optional arguments
for (int i = 2; i < args.Length; i++)
{
    if (args[i].Equals("--continue", StringComparison.OrdinalIgnoreCase))
    {
        continueOnMismatch = true;
    }
    else if (int.TryParse(args[i], out int parsedMB))
    {
        bufferSizeMB = parsedMB;
    }
    else
    {
        Console.WriteLine($"Unknown argument: {args[i]}");
        return 1;
    }
}

if (!File.Exists(file1) || !File.Exists(file2))
{
    Console.WriteLine("❌ One or both files do not exist.");
    return 1;
}

long length1 = new FileInfo(file1).Length;
long length2 = new FileInfo(file2).Length;

if (length1 != length2)
{
    Console.WriteLine($"❌ Files differ in size: {length1} vs {length2}");
    return 1;
}

int bufferSize = bufferSizeMB * 1024 * 1024;
byte[] buffer1 = new byte[bufferSize];
byte[] buffer2 = new byte[bufferSize];

try
{
    using var fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
    using var fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);

    long totalRead = 0;
    int mismatchCount = 0;

    Stopwatch stopwatch = Stopwatch.StartNew();

    while (true)
    {
        int read1 = fs1.Read(buffer1, 0, bufferSize);
        int read2 = fs2.Read(buffer2, 0, bufferSize);

        if (read1 != read2)
        {
            Console.WriteLine($"❌ Read size mismatch at offset {totalRead}");
            return 1;
        }

        if (read1 == 0) break; // EOF

        if (!buffer1.AsSpan(0, read1).SequenceEqual(buffer2.AsSpan(0, read2)))
        {
            // Find first mismatch offset in this chunk
            for (int i = 0; i < read1; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    long mismatchOffset = totalRead + i;
                    Console.WriteLine($"❌ Mismatch at byte offset {mismatchOffset}");
                    mismatchCount++;
                    if (!continueOnMismatch)
                    {
                        return 1;
                    }
                    break; // report one mismatch per chunk only
                }
            }
        }

        totalRead += read1;

        double percent = totalRead * 100.0 / length1;
        double secondsElapsed = stopwatch.Elapsed.TotalSeconds;
        double rate = totalRead / secondsElapsed; // bytes per second
        double secondsRemaining = (length1 - totalRead) / rate;
        TimeSpan eta = TimeSpan.FromSeconds(secondsRemaining);
        Console.Write($"\rProgress: {percent:F2}% - ETA: {eta:hh\\:mm\\:ss}       ");
    }

    stopwatch.Stop();
    Console.WriteLine();

    if (mismatchCount == 0)
    {
        Console.WriteLine("✅ Files are identical.");
        return 0;
    }
    else
    {
        Console.WriteLine($"⚠️ {mismatchCount} mismatch(es) found.");
        return 1;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    return 1;
}