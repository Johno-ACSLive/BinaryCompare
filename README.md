
# 🔍 BinaryCompare

**BinaryCompare** is a high-performance binary file comparison tool, implemented in both **PowerShell** and **C#**, for comparing large files byte-by-byte. It's optimized for speed and accuracy, with real-time progress reporting and support for configurable buffer sizes.

Completely vibe coded with ChatGPT, please see [Chat History](https://chatgpt.com/share/687f72bf-c548-8003-b535-5658e342d62b)!

---

## 🚀 Features

- ✅ Fast binary comparison using buffered reads
- ✅ Progress bar with estimated time to completion (ETA)
- ✅ Optional continue-on-mismatch mode
- ✅ Configurable buffer size (up to several GBs)
- ✅ Implemented in both PowerShell and C#
- ✅ .NET version supports Ahead-of-Time (AOT) compilation for native speed

---

## 🧰 Use Cases

- Verifying file integrity
- Comparing ISO images or disk snapshots
- Testing deduplication or backup systems
- Spot-checking data corruption across systems

---

## 📦 PowerShell Usage

### 🔧 Requirements
- PowerShell 5.1 or PowerShell Core (7+)

### 📄 Example

```powershell
.\Compare-Files.ps1 -File1 "C:\Path\to\file1.bin" -File2 "C:\Path\to\file2.bin" -BufferSizeMB 64 -ContinueOnMismatch $true
```

### 📝 Parameters

| Parameter              | Description |
|------------------------|-------------|
| `-File1`               | Path to the first file |
| `-File2`               | Path to the second file |
| `-BufferSize`          | (Optional) Size of buffer in MB (default: 64) |
| `-ContinueOnMismatch`  | (Optional) If specified, comparison continues after mismatches |

---

## 🧪 C# Usage

### 🔧 Requirements
- .NET 9 SDK

### 🔨 Build Instructions

#### Build with .NET CLI:
```cmd
dotnet build -c Release
```

#### Publish as native (AOT) self-contained binary:
```cmd
dotnet publish -c Release -r win-x64 -p:PublishAot=true --self-contained true --output bin\publish
```

> Replace `win-x64` with `linux-x64` or `osx-arm64` for other platforms.

### 📄 Example

```cmd
BinaryCompare.exe "C:\Path\to\file1.bin" "C:\Path\to\file2.bin" 64 --continue
```

### 📝 Parameters

| Parameter          | Description |
|--------------------|-------------|
| `file1`            | Path to the first file |
| `file2`            | Path to the second file |
| `BufferSize`       | (Optional) Buffer size in MB (default: 64) |
| `--continue`       | (Optional) If specified, comparison continues after mismatches |

---

## 📊 Sample Output

```plaintext
Progress: 38.54% - ETA: 00:01:12
❌ Mismatch found at byte offset: 67108864
Progress: 100.00% - ETA: 00:00:00
✅ Files match
```

---

## 🛠 Performance Tips

- Use larger buffer sizes (e.g., 128–512 MB) for large files to maximize throughput.
- For ultimate speed, use the AOT-compiled C# version.
- Avoid updating progress too frequently when using large buffers — once per read is ideal.

---

## 📃 License

MIT License. Free to use, modify, and distribute.

---

## 👨‍💻 Author

Vibe coded with ChatGPT, powered by performance and precision. Need help or want to contribute? Open an issue or pull request!