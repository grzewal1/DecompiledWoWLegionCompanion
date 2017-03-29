using System;
using System.IO;

public class FileUtils
{
	public readonly static char[] FOLDER_SEPARATOR_CHARS;

	static FileUtils()
	{
		FileUtils.FOLDER_SEPARATOR_CHARS = new char[] { '/', '\\' };
	}

	public FileUtils()
	{
	}

	public static string GameAssetPathToName(string path)
	{
		int num = path.LastIndexOf('/');
		if (num < 0)
		{
			return path;
		}
		return path.Substring(num + 1);
	}

	public static string GameToSourceAssetName(string folder, string name, string dotExtension = ".prefab")
	{
		return string.Format("{0}/{1}{2}", folder, name, dotExtension);
	}

	public static string GameToSourceAssetPath(string path, string dotExtension = ".prefab")
	{
		return string.Format("{0}{1}", path, dotExtension);
	}

	public static bool GetLastFolderAndFileFromPath(string path, out string folderName, out string fileName)
	{
		folderName = null;
		fileName = null;
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}
		int num = path.LastIndexOfAny(FileUtils.FOLDER_SEPARATOR_CHARS);
		if (num > 0)
		{
			int num1 = path.LastIndexOfAny(FileUtils.FOLDER_SEPARATOR_CHARS, num - 1);
			int num2 = (num1 >= 0 ? num1 + 1 : 0);
			folderName = path.Substring(num2, num - num2);
		}
		if (num < 0)
		{
			fileName = path;
		}
		else if (num < path.Length - 1)
		{
			fileName = path.Substring(num + 1);
		}
		return (folderName != null ? true : fileName != null);
	}

	public static string GetOnDiskCapitalizationForDir(string dirPath)
	{
		return FileUtils.GetOnDiskCapitalizationForDir(new DirectoryInfo(dirPath));
	}

	public static string GetOnDiskCapitalizationForDir(DirectoryInfo dirInfo)
	{
		DirectoryInfo parent = dirInfo.Parent;
		if (parent == null)
		{
			return dirInfo.Name;
		}
		string name = parent.GetDirectories(dirInfo.Name)[0].Name;
		return Path.Combine(FileUtils.GetOnDiskCapitalizationForDir(parent), name);
	}

	public static string GetOnDiskCapitalizationForFile(string filePath)
	{
		return filePath;
	}

	public static string GetOnDiskCapitalizationForFile(FileInfo fileInfo)
	{
		DirectoryInfo directory = fileInfo.Directory;
		string name = directory.GetFiles(fileInfo.Name)[0].Name;
		return Path.Combine(FileUtils.GetOnDiskCapitalizationForDir(directory), name);
	}

	public static string MakeMetaPathFromSourcePath(string path)
	{
		return string.Format("{0}.meta", path);
	}

	public static string MakeSourceAssetMetaPath(string path)
	{
		return FileUtils.MakeMetaPathFromSourcePath(FileUtils.MakeSourceAssetPath(path));
	}

	public static string MakeSourceAssetPath(DirectoryInfo folder)
	{
		return FileUtils.MakeSourceAssetPath(folder.FullName);
	}

	public static string MakeSourceAssetPath(FileInfo fileInfo)
	{
		return FileUtils.MakeSourceAssetPath(fileInfo.FullName);
	}

	public static string MakeSourceAssetPath(string path)
	{
		string str = path.Replace("\\", "/");
		int num = str.IndexOf("/Assets", StringComparison.OrdinalIgnoreCase);
		return str.Remove(0, num + 1);
	}

	public static bool SetFileWritableFlag(string path, bool setWritable)
	{
		bool flag;
		if (!File.Exists(path))
		{
			return false;
		}
		try
		{
			FileAttributes attributes = File.GetAttributes(path);
			FileAttributes fileAttribute = (!setWritable ? attributes | FileAttributes.ReadOnly : attributes & (FileAttributes.Archive | FileAttributes.Compressed | FileAttributes.Device | FileAttributes.Directory | FileAttributes.Encrypted | FileAttributes.Hidden | FileAttributes.Normal | FileAttributes.NotContentIndexed | FileAttributes.Offline | FileAttributes.ReparsePoint | FileAttributes.SparseFile | FileAttributes.System | FileAttributes.Temporary));
			if (setWritable && Environment.OSVersion.Platform == PlatformID.MacOSX)
			{
				fileAttribute = fileAttribute | FileAttributes.Normal;
			}
			if (fileAttribute != attributes)
			{
				File.SetAttributes(path, fileAttribute);
				flag = (File.GetAttributes(path) == fileAttribute ? true : false);
			}
			else
			{
				flag = true;
			}
		}
		catch (DirectoryNotFoundException directoryNotFoundException)
		{
			return false;
		}
		catch (FileNotFoundException fileNotFoundException)
		{
			return false;
		}
		catch (Exception exception)
		{
			return false;
		}
		return flag;
	}

	public static bool SetFolderWritableFlag(string dirPath, bool writable)
	{
		string[] files = Directory.GetFiles(dirPath);
		for (int i = 0; i < (int)files.Length; i++)
		{
			FileUtils.SetFileWritableFlag(files[i], writable);
		}
		string[] directories = Directory.GetDirectories(dirPath);
		for (int j = 0; j < (int)directories.Length; j++)
		{
			FileUtils.SetFolderWritableFlag(directories[j], writable);
		}
		return true;
	}

	public static string SourceToGameAssetName(string path)
	{
		int num = path.LastIndexOf('/');
		if (num < 0)
		{
			return path;
		}
		int num1 = path.LastIndexOf('.');
		if (num1 < 0)
		{
			return path;
		}
		return path.Substring(num + 1, num1);
	}

	public static string SourceToGameAssetPath(string path)
	{
		int num = path.LastIndexOf('.');
		if (num < 0)
		{
			return path;
		}
		return path.Substring(0, num);
	}
}