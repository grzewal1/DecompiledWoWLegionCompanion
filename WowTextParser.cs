using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using WowStaticData;

public class WowTextParser
{
	private List<WowTextParser.ParseToken> m_tokens;

	private int m_readIndex;

	private string m_input;

	private string m_currentValue;

	private int m_bracketLevel;

	private bool m_richText;

	private int m_spellID;

	private static WowTextParser s_parser;

	public static WowTextParser parser
	{
		get
		{
			if (WowTextParser.s_parser == null)
			{
				WowTextParser.s_parser = new WowTextParser();
			}
			return WowTextParser.s_parser;
		}
	}

	static WowTextParser()
	{
	}

	public WowTextParser()
	{
	}

	private void AddCloseBracketToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.ClosedBracket,
			stringValue = "}"
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
		WowTextParser mBracketLevel = this;
		mBracketLevel.m_bracketLevel = mBracketLevel.m_bracketLevel - 1;
		if (this.m_bracketLevel < 0)
		{
			throw new Exception(string.Concat("AddCloseBracketToken(): Mismatched brackets in string ", this.m_input));
		}
	}

	private void AddColorEndToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.ColorEnd,
			stringValue = this.m_currentValue
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
	}

	private void AddColorStartToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.ColorStart,
			stringValue = this.m_currentValue
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
	}

	private void AddMinusToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.Subtract,
			stringValue = "-"
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
	}

	private void AddMultiplyToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.Multiply,
			stringValue = "*"
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
	}

	private void AddNumericToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.Number,
			stringValue = this.m_currentValue
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
	}

	private void AddOpenBracketToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.OpenBracket,
			stringValue = "{"
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
		WowTextParser mBracketLevel = this;
		mBracketLevel.m_bracketLevel = mBracketLevel.m_bracketLevel + 1;
	}

	private void AddPlusToken()
	{
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.Add,
			stringValue = "+"
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
	}

	private void AddTextToken()
	{
		if (this.m_currentValue == string.Empty)
		{
			return;
		}
		WowTextParser.ParseToken parseToken = new WowTextParser.ParseToken()
		{
			type = WowTextParser.TokenType.Text,
			stringValue = this.m_currentValue
		};
		this.m_currentValue = string.Empty;
		this.m_tokens.Add(parseToken);
	}

	private bool CharacterIsNumeric(char c)
	{
		return (c == '.' || c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' ? true : c == '9');
	}

	private bool CharacterIsValid()
	{
		return this.m_readIndex < this.m_input.Length;
	}

	private bool ConsumeCharacter()
	{
		WowTextParser mReadIndex = this;
		mReadIndex.m_readIndex = mReadIndex.m_readIndex + 1;
		return this.m_readIndex < this.m_input.Length;
	}

	private bool ConsumeCharacters(int length)
	{
		WowTextParser mReadIndex = this;
		mReadIndex.m_readIndex = mReadIndex.m_readIndex + length;
		if (this.m_readIndex > this.m_input.Length)
		{
			this.m_readIndex = this.m_input.Length;
		}
		return this.m_readIndex < this.m_input.Length;
	}

	public bool IsRichText()
	{
		return this.m_richText;
	}

	public string Parse(string input, int spellID = 0)
	{
		WowTextParser.TokenType tokenType;
		this.m_input = input;
		this.m_tokens = new List<WowTextParser.ParseToken>();
		this.m_readIndex = 0;
		this.m_currentValue = string.Empty;
		this.m_bracketLevel = 0;
		this.m_richText = false;
		this.m_spellID = spellID;
		if (spellID > 0 && GeneralHelpers.SpellGrantsArtifactXP(spellID))
		{
			return this.ParseForArtifactXP(input, spellID);
		}
		do
		{
			int mReadIndex = this.m_readIndex;
			if (this.ReadCharacter() == '$')
			{
				this.ParseDollarSign();
			}
			else if (this.ReadCharacter() == '}')
			{
				this.AddCloseBracketToken();
				this.ConsumeCharacter();
			}
			else if (this.ReadCharacter() == '+')
			{
				this.ParsePlusSign();
			}
			else if (this.ReadCharacter() == '-')
			{
				this.ParseMinusSign();
			}
			else if (this.ReadCharacter() == '*')
			{
				this.ParseMultiply();
			}
			else if (this.ReadCharacter() != '|')
			{
				this.ParseCharacter();
			}
			else
			{
				this.ParseBar();
			}
			if (this.m_readIndex != mReadIndex)
			{
				continue;
			}
			throw new Exception(string.Concat("Parse: loop failed to advance in string ", this.m_input));
		}
		while (this.CharacterIsValid());
		this.AddTextToken();
		this.SimplifyTokens();
		string empty = string.Empty;
	Label1:
		foreach (WowTextParser.ParseToken mToken in this.m_tokens)
		{
			tokenType = mToken.type;
			switch (tokenType)
			{
				case WowTextParser.TokenType.Text:
				{
					empty = string.Concat(empty, mToken.stringValue);
					continue;
				}
				case WowTextParser.TokenType.Number:
				{
					empty = string.Concat(empty, mToken.stringValue);
					continue;
				}
				default:
				{
					if (tokenType == WowTextParser.TokenType.ColorStart)
					{
						break;
					}
					else
					{
						goto Label0;
					}
				}
			}
			empty = string.Concat(empty, mToken.stringValue);
		}
		return empty;
	Label0:
		if (tokenType == WowTextParser.TokenType.ColorEnd)
		{
			empty = string.Concat(empty, mToken.stringValue);
			goto Label1;
		}
		else
		{
			goto Label1;
		}
	}

	private void ParseActionHours()
	{
		this.ConsumeCharacter();
		this.ReadAndConsumeNumber();
		int num = Convert.ToInt32(this.m_currentValue);
		GarrAbilityEffectRec record = StaticDB.garrAbilityEffectDB.GetRecord(num);
		if (record == null)
		{
			this.m_currentValue = "0";
		}
		else
		{
			this.m_currentValue = record.ActionHours.ToString();
		}
		this.AddNumericToken();
	}

	private void ParseActionValueFlat()
	{
		this.ConsumeCharacter();
		this.ReadAndConsumeNumber();
		int num = Convert.ToInt32(this.m_currentValue);
		GarrAbilityEffectRec record = StaticDB.garrAbilityEffectDB.GetRecord(num);
		if (record == null)
		{
			this.m_currentValue = "0";
		}
		else
		{
			this.m_currentValue = record.ActionValueFlat.ToString();
		}
		this.AddNumericToken();
	}

	private void ParseAmpersand()
	{
		this.ConsumeCharacter();
		if (this.ReadString(10).ToLower() != "garrabdesc")
		{
			return;
		}
		this.ConsumeCharacters(10);
		this.ReadAndConsumeNumber();
		if (this.m_currentValue == string.Empty)
		{
			return;
		}
		int num = Convert.ToInt32(this.m_currentValue);
		GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(num);
		if (record == null)
		{
			return;
		}
		WowTextParser wowTextParser = new WowTextParser();
		this.m_currentValue = wowTextParser.Parse(record.Description, 0);
		this.AddTextToken();
	}

	private void ParseBar()
	{
		this.AddTextToken();
		if (!this.ConsumeCharacter())
		{
			return;
		}
		if (this.ReadCharacter() == 'c' || this.ReadCharacter() == 'C')
		{
			this.ParseColorStart();
		}
		else if (this.ReadCharacter() == 'r' || this.ReadCharacter() == 'R')
		{
			this.ParseColorEnd();
		}
		else if (this.ReadCharacter() == 't' || this.ReadCharacter() == 'T')
		{
			this.ParseInlineIcon();
		}
		else
		{
			this.ParseCharacter();
		}
	}

	private void ParseCharacter()
	{
		if (this.m_bracketLevel <= 0)
		{
			WowTextParser wowTextParser = this;
			wowTextParser.m_currentValue = string.Concat(wowTextParser.m_currentValue, this.ReadCharacter());
			this.ConsumeCharacter();
		}
		else if (!this.CharacterIsNumeric(this.ReadCharacter()))
		{
			this.ConsumeCharacter();
		}
		else
		{
			this.ReadAndConsumeNumber();
			this.AddNumericToken();
		}
	}

	private void ParseColorEnd()
	{
		this.ConsumeCharacter();
		this.m_currentValue = "</color>";
		this.AddColorEndToken();
	}

	private void ParseColorStart()
	{
		this.m_richText = true;
		this.ConsumeCharacter();
		this.m_currentValue = "<color=#";
		for (int i = 0; i < 8; i++)
		{
			WowTextParser wowTextParser = this;
			wowTextParser.m_currentValue = string.Concat(wowTextParser.m_currentValue, this.ReadCharacter());
			this.ConsumeCharacter();
		}
		WowTextParser wowTextParser1 = this;
		wowTextParser1.m_currentValue = string.Concat(wowTextParser1.m_currentValue, ">");
		this.AddColorStartToken();
	}

	private void ParseCombatWeightBase()
	{
		this.ConsumeCharacter();
		this.ReadAndConsumeNumber();
		int num = Convert.ToInt32(this.m_currentValue);
		GarrAbilityEffectRec record = StaticDB.garrAbilityEffectDB.GetRecord(num);
		if (record == null)
		{
			this.m_currentValue = "0";
		}
		else
		{
			this.m_currentValue = record.CombatWeightBase.ToString();
		}
		this.AddNumericToken();
	}

	private void ParseCombatWeightMax()
	{
		this.ConsumeCharacter();
		this.ReadAndConsumeNumber();
		int num = Convert.ToInt32(this.m_currentValue);
		GarrAbilityEffectRec record = StaticDB.garrAbilityEffectDB.GetRecord(num);
		if (record == null)
		{
			this.m_currentValue = "0";
		}
		else
		{
			this.m_currentValue = record.CombatWeightMax.ToString();
		}
		this.AddNumericToken();
	}

	private void ParseDollarSign()
	{
		this.AddTextToken();
		if (!this.ConsumeCharacter())
		{
			return;
		}
		bool flag = (this.PeekCharacter(1) < '0' ? false : this.PeekCharacter(1) <= '9');
		if (this.ReadCharacter() == '{')
		{
			this.AddOpenBracketToken();
			this.ConsumeCharacter();
		}
		else if (this.ReadCharacter() == 'a' || this.ReadCharacter() == 'A')
		{
			this.ParseActionValueFlat();
		}
		else if (this.ReadCharacter() == 'b' || this.ReadCharacter() == 'B')
		{
			this.ParseCombatWeightBase();
		}
		else if (this.ReadCharacter() == 'm' || this.ReadCharacter() == 'M')
		{
			if (!flag || this.m_spellID == 0)
			{
				this.ParseCombatWeightMax();
			}
			else
			{
				this.ParseSpellPoints();
			}
		}
		else if (this.ReadCharacter() == 'h' || this.ReadCharacter() == 'H')
		{
			this.ParseActionHours();
		}
		else if (this.ReadCharacter() == 's' || this.ReadCharacter() == 'S')
		{
			this.ParseSpellPoints();
		}
		else if (this.ReadCharacter() != '@')
		{
			this.ParseCharacter();
		}
		else
		{
			this.ParseAmpersand();
		}
	}

	private string ParseForArtifactXP(string input, int spellID)
	{
		string value = Regex.Match(input, "\\d+").Value;
		if (value == string.Empty)
		{
			return input;
		}
		int num = int.Parse(value);
		int num1 = GeneralHelpers.ApplyArtifactXPMultiplier(num, GarrisonStatus.ArtifactXpMultiplier);
		if (num1 <= 1000000)
		{
			return input.Replace(value, num1.ToString());
		}
		double num2 = (double)((float)num1) / 1000000;
		string str = string.Format("{0:F1} {1}", num2, StaticDB.GetString("MILLION", "million"));
		return input.Replace(value, str);
	}

	private void ParseInlineIcon()
	{
		this.ConsumeCharacter();
		while (this.ReadCharacter() != '|' && this.ConsumeCharacter())
		{
		}
		this.ConsumeCharacter();
		this.ConsumeCharacter();
	}

	private void ParseMinusSign()
	{
		if (this.m_bracketLevel <= 0)
		{
			this.ParseCharacter();
		}
		else
		{
			this.AddMinusToken();
			this.ConsumeCharacter();
		}
	}

	private void ParseMultiply()
	{
		if (this.m_bracketLevel <= 0)
		{
			this.ParseCharacter();
		}
		else
		{
			this.AddMultiplyToken();
			this.ConsumeCharacter();
		}
	}

	private void ParsePlusSign()
	{
		if (this.m_bracketLevel <= 0)
		{
			this.ParseCharacter();
		}
		else
		{
			this.AddPlusToken();
			this.ConsumeCharacter();
		}
	}

	private void ParseSpellPoints()
	{
		this.ConsumeCharacter();
		this.ReadAndConsumeNumber();
		int num = Convert.ToInt32(this.m_currentValue);
		num--;
		if (num < 0)
		{
			num = 0;
		}
		int effectBasePoints = 0;
		StaticDB.spellEffectDB.EnumRecordsByParentID(this.m_spellID, (SpellEffectRec spellEffectRec) => {
			if (spellEffectRec.EffectIndex != num)
			{
				return true;
			}
			effectBasePoints = spellEffectRec.EffectBasePoints;
			return false;
		});
		this.m_currentValue = effectBasePoints.ToString();
		this.AddNumericToken();
	}

	private char PeekCharacter(int offset)
	{
		int mReadIndex = this.m_readIndex + offset;
		if (mReadIndex >= this.m_input.Length || mReadIndex < 0)
		{
			return ' ';
		}
		return this.m_input[mReadIndex];
	}

	private void ReadAndConsumeNumber()
	{
		this.m_currentValue = string.Empty;
		while (this.CharacterIsNumeric(this.ReadCharacter()))
		{
			WowTextParser wowTextParser = this;
			wowTextParser.m_currentValue = string.Concat(wowTextParser.m_currentValue, this.ReadCharacter());
			this.ConsumeCharacter();
		}
	}

	private char ReadCharacter()
	{
		if (this.m_readIndex >= this.m_input.Length)
		{
			return ' ';
		}
		return this.m_input[this.m_readIndex];
	}

	private string ReadString(int length)
	{
		if (this.m_readIndex + length > this.m_input.Length)
		{
			return string.Empty;
		}
		return this.m_input.Substring(this.m_readIndex, length);
	}

	private bool SimplifyAddSubtract()
	{
		for (int i = 0; i < this.m_tokens.Count; i++)
		{
			if (this.m_tokens[i].type == WowTextParser.TokenType.Add || this.m_tokens[i].type == WowTextParser.TokenType.Subtract)
			{
				if (i == 0 || i + 1 >= this.m_tokens.Count || this.m_tokens[i - 1].type != WowTextParser.TokenType.Number || this.m_tokens[i + 1].type != WowTextParser.TokenType.Number)
				{
					throw new Exception(string.Concat("SimplifyAddSubtract(): Invalid multiply in string ", this.m_input));
				}
				WowTextParser.ParseToken item = this.m_tokens[i - 1];
				double num = Convert.ToDouble(item.stringValue);
				WowTextParser.ParseToken parseToken = this.m_tokens[i + 1];
				double num1 = Convert.ToDouble(parseToken.stringValue);
				double num2 = 0;
				num2 = (this.m_tokens[i].type != WowTextParser.TokenType.Add ? num - num1 : num + num1);
				int num3 = (int)num2;
				this.m_tokens.RemoveAt(i + 1);
				this.m_tokens.RemoveAt(i);
				WowTextParser.ParseToken parseToken1 = new WowTextParser.ParseToken()
				{
					type = WowTextParser.TokenType.Number,
					stringValue = num3.ToString()
				};
				this.m_tokens.Insert(i, parseToken1);
				this.m_tokens.RemoveAt(i - 1);
				return true;
			}
		}
		return false;
	}

	private bool SimplifyMultiplication()
	{
		for (int i = 0; i < this.m_tokens.Count; i++)
		{
			if (this.m_tokens[i].type == WowTextParser.TokenType.Multiply)
			{
				if (i == 0 || i + 1 >= this.m_tokens.Count || this.m_tokens[i - 1].type != WowTextParser.TokenType.Number || this.m_tokens[i + 1].type != WowTextParser.TokenType.Number)
				{
					throw new Exception(string.Concat("SimplifyMultiply(): Invalid multiply in string ", this.m_input));
				}
				WowTextParser.ParseToken item = this.m_tokens[i - 1];
				double num = Convert.ToDouble(item.stringValue);
				WowTextParser.ParseToken parseToken = this.m_tokens[i + 1];
				double num1 = Convert.ToDouble(parseToken.stringValue);
				int num2 = (int)(num * num1);
				this.m_tokens.RemoveAt(i + 1);
				this.m_tokens.RemoveAt(i);
				WowTextParser.ParseToken parseToken1 = new WowTextParser.ParseToken()
				{
					type = WowTextParser.TokenType.Number,
					stringValue = num2.ToString()
				};
				this.m_tokens.Insert(i, parseToken1);
				this.m_tokens.RemoveAt(i - 1);
				return true;
			}
		}
		return false;
	}

	private void SimplifyTokens()
	{
		while (this.SimplifyMultiplication())
		{
		}
		while (this.SimplifyAddSubtract())
		{
		}
	}

	private struct ParseToken
	{
		public WowTextParser.TokenType type;

		public string stringValue;
	}

	private enum TokenType
	{
		Text,
		OpenBracket,
		ClosedBracket,
		Number,
		Add,
		Subtract,
		Multiply,
		ColorStart,
		ColorEnd
	}
}