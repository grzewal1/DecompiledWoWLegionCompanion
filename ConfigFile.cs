using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConfigFile
{
	private string m_path;

	private List<ConfigFile.Line> m_lines = new List<ConfigFile.Line>();

	public ConfigFile()
	{
	}

	public void Clear()
	{
		this.m_lines.Clear();
	}

	public bool Delete(string key, bool removeEmptySections = true)
	{
		int num = this.FindEntryIndex(key);
		if (num < 0)
		{
			return false;
		}
		this.m_lines.RemoveAt(num);
		if (removeEmptySections)
		{
			int num1 = num - 1;
			while (num1 >= 0)
			{
				ConfigFile.Line item = this.m_lines[num1];
				if (item.m_type != ConfigFile.LineType.SECTION)
				{
					if (!string.IsNullOrEmpty(item.m_raw.Trim()))
					{
						return true;
					}
					num1--;
				}
				else
				{
					break;
				}
			}
			int num2 = num;
			while (num2 < this.m_lines.Count)
			{
				ConfigFile.Line line = this.m_lines[num2];
				if (line.m_type != ConfigFile.LineType.SECTION)
				{
					if (!string.IsNullOrEmpty(line.m_raw.Trim()))
					{
						return true;
					}
					num2++;
				}
				else
				{
					break;
				}
			}
			this.m_lines.RemoveRange(num1, num2 - num1);
		}
		return true;
	}

	private ConfigFile.Line FindEntry(string fullKey)
	{
		int num = this.FindEntryIndex(fullKey);
		if (num < 0)
		{
			return null;
		}
		return this.m_lines[num];
	}

	private int FindEntryIndex(string fullKey)
	{
		for (int i = 0; i < this.m_lines.Count; i++)
		{
			ConfigFile.Line item = this.m_lines[i];
			if (item.m_type == ConfigFile.LineType.ENTRY)
			{
				if (item.m_fullKey.Equals(fullKey, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
		}
		return -1;
	}

	private int FindSectionIndex(string sectionName)
	{
		for (int i = 0; i < this.m_lines.Count; i++)
		{
			ConfigFile.Line item = this.m_lines[i];
			if (item.m_type == ConfigFile.LineType.SECTION)
			{
				if (item.m_sectionName.Equals(sectionName, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
		}
		return -1;
	}

	public bool FullLoad(string path)
	{
		return this.Load(path, false);
	}

	public string GenerateText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.m_lines.Count; i++)
		{
			ConfigFile.Line item = this.m_lines[i];
			ConfigFile.LineType mType = item.m_type;
			if (mType == ConfigFile.LineType.SECTION)
			{
				stringBuilder.AppendFormat("[{0}]", item.m_sectionName);
			}
			else if (mType != ConfigFile.LineType.ENTRY)
			{
				stringBuilder.Append(item.m_raw);
			}
			else if (!item.m_quoteValue)
			{
				stringBuilder.AppendFormat("{0} = {1}", item.m_lineKey, item.m_value);
			}
			else
			{
				stringBuilder.AppendFormat("{0} = \"{1}\"", item.m_lineKey, item.m_value);
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	public string Get(string key, string defaultVal = "")
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return line.m_value;
	}

	public bool Get(string key, bool defaultVal = false)
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return GeneralUtils.ForceBool(line.m_value);
	}

	public int Get(string key, int defaultVal = 0)
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return GeneralUtils.ForceInt(line.m_value);
	}

	public float Get(string key, float defaultVal = 0f)
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return GeneralUtils.ForceFloat(line.m_value);
	}

	public List<ConfigFile.Line> GetLines()
	{
		return this.m_lines;
	}

	public string GetPath()
	{
		return this.m_path;
	}

	public bool Has(string key)
	{
		return this.FindEntry(key) != null;
	}

	public bool LightLoad(string path)
	{
		return this.Load(path, true);
	}

	private bool Load(string path, bool ignoreUselessLines)
	{
		this.m_path = null;
		this.m_lines.Clear();
		if (!File.Exists(path))
		{
			Console.WriteLine(string.Concat("Error loading config file ", path));
			return false;
		}
		int num = 1;
		using (StreamReader streamReader = File.OpenText(path))
		{
			string empty = string.Empty;
			while (streamReader.Peek() != -1)
			{
				string str = streamReader.ReadLine();
				string str1 = str.Trim();
				if (!ignoreUselessLines || str1.Length > 0)
				{
					bool flag = (str1.Length <= 0 ? false : str1[0] == ';');
					if (!ignoreUselessLines || !flag)
					{
						ConfigFile.Line line = new ConfigFile.Line()
						{
							m_raw = str,
							m_sectionName = empty
						};
						if (flag)
						{
							line.m_type = ConfigFile.LineType.COMMENT;
						}
						else if (str1.Length > 0)
						{
							if (str1[0] != '[')
							{
								int num1 = str1.IndexOf('=');
								if (num1 >= 0)
								{
									string str2 = str1.Substring(0, num1).Trim();
									string str3 = str1.Substring(num1 + 1, str1.Length - num1 - 1).Trim();
									if (str3.Length > 2)
									{
										int length = str3.Length - 1;
										if ((str3[0] == '\"' || str3[0] == '“' || str3[0] == '”') && (str3[length] == '\"' || str3[length] == '“' || str3[length] == '”'))
										{
											str3 = str3.Substring(1, str3.Length - 2);
											line.m_quoteValue = true;
										}
									}
									line.m_type = ConfigFile.LineType.ENTRY;
									line.m_fullKey = string.Format("{0}.{1}", empty, str2);
									line.m_lineKey = str2;
									line.m_value = str3;
								}
								else
								{
									Console.WriteLine(string.Format("ConfigFile.Load() - invalid entry \"{0}\" on line {1} in file {2}", str, num, path));
									if (!ignoreUselessLines)
									{
										this.m_lines.Add(line);
									}
									continue;
								}
							}
							else if (str1.Length < 2 || str1[str1.Length - 1] != ']')
							{
								Console.WriteLine(string.Format("ConfigFile.Load() - invalid section \"{0}\" on line {1} in file {2}", str, num, path));
								if (!ignoreUselessLines)
								{
									this.m_lines.Add(line);
								}
								continue;
							}
							else
							{
								line.m_type = ConfigFile.LineType.SECTION;
								string str4 = str1.Substring(1, str1.Length - 2);
								empty = str4;
								line.m_sectionName = str4;
								this.m_lines.Add(line);
								continue;
							}
						}
						this.m_lines.Add(line);
					}
				}
			}
		}
		this.m_path = path;
		return true;
	}

	private ConfigFile.Line RegisterEntry(string fullKey)
	{
		if (string.IsNullOrEmpty(fullKey))
		{
			return null;
		}
		int num = fullKey.IndexOf('.');
		if (num < 0)
		{
			return null;
		}
		string str = fullKey.Substring(0, num);
		string empty = string.Empty;
		if (fullKey.Length > num + 1)
		{
			empty = fullKey.Substring(num + 1, fullKey.Length - num - 1);
		}
		ConfigFile.Line line = null;
		int num1 = this.FindSectionIndex(str);
		if (num1 >= 0)
		{
			int num2 = num1 + 1;
			while (num2 < this.m_lines.Count)
			{
				ConfigFile.Line item = this.m_lines[num2];
				if (item.m_type != ConfigFile.LineType.SECTION)
				{
					if (item.m_type == ConfigFile.LineType.ENTRY)
					{
						if (item.m_lineKey.Equals(empty, StringComparison.OrdinalIgnoreCase))
						{
							line = item;
							break;
						}
					}
					num2++;
				}
				else
				{
					break;
				}
			}
			if (line == null)
			{
				line = new ConfigFile.Line()
				{
					m_type = ConfigFile.LineType.ENTRY,
					m_sectionName = str,
					m_lineKey = empty,
					m_fullKey = fullKey
				};
				this.m_lines.Insert(num2, line);
			}
		}
		else
		{
			ConfigFile.Line mSectionName = new ConfigFile.Line();
			if (this.m_lines.Count > 0)
			{
				mSectionName.m_sectionName = this.m_lines[this.m_lines.Count - 1].m_sectionName;
			}
			this.m_lines.Add(mSectionName);
			ConfigFile.Line line1 = new ConfigFile.Line()
			{
				m_type = ConfigFile.LineType.SECTION,
				m_sectionName = str
			};
			this.m_lines.Add(line1);
			line = new ConfigFile.Line()
			{
				m_type = ConfigFile.LineType.ENTRY,
				m_sectionName = str,
				m_lineKey = empty,
				m_fullKey = fullKey
			};
			this.m_lines.Add(line);
		}
		return line;
	}

	public bool Save(string path = null)
	{
		bool flag;
		if (path == null)
		{
			path = this.m_path;
		}
		if (path == null)
		{
			Console.WriteLine("ConfigFile.Save() - no path given");
			return false;
		}
		string str = this.GenerateText();
		try
		{
			FileUtils.SetFileWritableFlag(path, true);
			File.WriteAllText(path, str);
			this.m_path = path;
			return true;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			Console.WriteLine(string.Format("ConfigFile.Save() - Failed to write file at {0}. Exception={1}", path, exception.Message));
			flag = false;
		}
		return flag;
	}

	public bool Set(string key, object val)
	{
		return this.Set(key, (val != null ? val.ToString() : string.Empty));
	}

	public bool Set(string key, bool val)
	{
		return this.Set(key, (!val ? "false" : "true"));
	}

	public bool Set(string key, string val)
	{
		ConfigFile.Line line = this.RegisterEntry(key);
		if (line == null)
		{
			return false;
		}
		line.m_value = val;
		return true;
	}

	public class Line
	{
		public string m_raw;

		public ConfigFile.LineType m_type;

		public string m_sectionName;

		public string m_lineKey;

		public string m_fullKey;

		public string m_value;

		public bool m_quoteValue;

		public Line()
		{
		}
	}

	public enum LineType
	{
		UNKNOWN,
		COMMENT,
		SECTION,
		ENTRY
	}
}