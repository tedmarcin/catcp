
# CatCP: File Concatenation and Preview Utility

**CatCP** is a powerful command-line tool for previewing and concatenating file contents. It is designed to process multiple files efficiently, with support for wildcard patterns, exclusions, and customizable line limits. The output is saved as a UTF-8 encoded file and automatically opened in the default text editor.

## Features

- **Wildcard Support**: Use patterns like `*.txt`, `log*.*`, or `te*t.*` to select multiple files for processing.
- **File Exclusions**: Exclude files by specifying patterns (e.g., `-exclude=log,tmp`).
- **Line Limit**: Limit the number of lines read per file using the `-max` parameter (default: 10,000 lines).
- **Error Handling**: Handles file-related errors like missing files, unauthorized access, and invalid paths gracefully.
- **UTF-8 Output**: Generates a consolidated UTF-8 encoded file with clear separators for each processed file.
- **Automatic Opening**: Opens the output file in the default text editor after processing.

## Usage

Run the program with a combination of options and file patterns. Here's the basic syntax:

```bash
catcp.exe [options] [file1] [file2] ...
```

### Options
- `-exclude=<pattern1,pattern2,...>`: Exclude files matching the specified patterns.
- `-max=<number>`: Set the maximum number of lines to read from each file (default: 10,000).

### Examples
1. Concatenate all `.txt` files:
   ```bash
   catcp.exe *.txt
   ```

2. Include all `.txt` and `.log` files, but exclude those with names containing "debug" or "temp":
   ```bash
   catcp.exe *.txt *.log -exclude=debug,temp
   ```

3. Preview a specific number of lines (e.g., 500) from all `.csv` files:
   ```bash
   catcp.exe *.csv -max=500
   ```

4. Combine specific files and wildcard patterns:
   ```bash
   catcp.exe file1.txt logs/*.log -exclude=backup,archive
   ```

## How It Works

1. **File Matching**: Expands wildcard patterns into actual file paths.
2. **Exclusion Check**: Skips files matching the exclusion patterns.
3. **Content Processing**: Reads up to the specified number of lines from each file.
4. **UTF-8 Output**: Appends file content with clear separators into a single output file.
5. **File Opening**: Opens the output file in the default text editor for review.

## Requirements

- **Windows OS**: The program is designed to run on Windows systems.
- **.NET Framework or .NET 5+**: Ensure the required runtime is installed.

## Error Handling

The program provides detailed error messages for common issues, such as:
- Missing files
- Unauthorized access
- Invalid file paths or patterns


---

Feel free to modify or expand this description to fit your specific project goals!
