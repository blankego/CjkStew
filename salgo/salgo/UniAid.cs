using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Salgo
{
	/// <summary>
	/// Unicode cdoe point string.
	/// Immutable, every code point occupies 3 bytes
	/// </summary>
	unsafe public class UString : IEnumerable<CodePoint>
	{

		readonly CodePoint[] _data;
		readonly int _length, _strLength;

		#region props
		public int Length{ get { return _length; } }

		public int StrLength{ get { return _strLength; } }
		#endregion

		#region ctor
		unsafe public UString (string s, int idx, int len)
		{
			int end = idx + len, pIdx = 0;
			_data = new CodePoint[len];
				
			for (; idx != end; ++pIdx)
			{
				char c = s[idx++];//avoid copying : internal is crappy
				if (( c & 0xFC00 ) == 0xD800)
				{ 
					int cp = 0x10000 + ( c << 10 | ( s[idx++] & 0x3FF ) );
					_data[pIdx].Lo = (char)cp;
					_data[pIdx].Hi = (byte)(cp >> 16);
				}
				else
				{
					_data[pIdx].Hi = 0;
					_data[pIdx].Lo = c;//new CodePoint(c); 
				}						
			}
			_strLength = len;
			_length = pIdx;

//			Array.Resize(ref _data, cpIdx);//TODO: really necessary?
		}

		public UString (string s):this(s,0,s.Length){}

		public UString (string s, int idx):this(s,idx,s.Length - idx){}


		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<CodePoint> GetEnumerator()
		{
			for (int i = 0; i != _length; ++i)
			{
				yield return _data[i];
			}
		}
		#endregion

		unsafe public bool Contains(CodePoint item)
		{
			bool found = false;
			for (int i = 0; i < Length; ++i)
			{
				if (_data[i] == item)
				{
					found = true;
					break;
				}
			}
			return found;
		}

		public CodePoint this [int index] {
			get {
				return _data[index];
			}		
		}

		//[Acceleration(AccelMode.SSE1)]
		unsafe public override string ToString()
		{
			IntPtr ip = IntPtr.Zero;
			char * raw;
			if (_strLength > 0xFFFFF)
			{
				raw = (char*)( ip = Marshal.AllocHGlobal(_strLength * 2 + 2) ).ToPointer();
			}
			else
			{
				char * pStack = stackalloc char[_strLength + 1];//stackalloc quirk!
				raw = pStack;
			}
			try
			{
				char * pChar = raw;
				for (int i = 0; i!= _length; ++i)
				{
					CodePoint cp = _data[i];
					if (cp.Hi == 0)
					{
						*pChar++ = cp.Lo;
					}
					else
					{
						int code = ( ( cp.Hi << 16 ) | cp.Lo ) - 0x10000;
						*pChar++ = (char)( 0xD800 | ( code >> 10 ) );
						*pChar++ = (char)( 0xDC00 | ( code & 0x3FF ) );
					}				 			
				}
				*pChar = '\0';//Zeroed out or not the spec doesn't guarantee it, We can't take the chance!
				return new string(raw);
			} finally
			{
				if (ip != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(ip);
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	public struct CodePoint : IComparable<CodePoint> , IEquatable<CodePoint>
	{
		char _lo;
		byte _hi;

		public char Lo { get { return _lo; } internal set { _lo = value; } }

		public byte Hi { get { return _hi; } internal set { _hi = value; } }

		public bool InBmp {
			get{ return Hi == 0;}
		}

		public CodePoint (int i)
		{
			_hi = (byte)( i >> 16 );
			_lo = (char)i;
		}

		public CodePoint (char c)
		{
			_hi = 0;
			_lo = c;
		}

		unsafe public int ToU16(char *pChar)
		{
			int len = Hi == 0 ? 1 : 2;
			if (len == 1)
			{
				*pChar++ = Lo; 
			}
			else
			{
				int cp = ( ( Hi << 16 ) | Lo ) - 0x10000;
				*pChar++ = (char)( 0xD800 | ( cp >> 10 ) );
				*pChar++ = (char)( 0xDC00 | ( cp & 0x3FF ) );
			}
			*pChar = '\0'; //Zeroed out or not the spec doesn't guarantee it, We can't take the chance!
			return len;
		}

		static public explicit operator int(CodePoint cp)
		{
			return ( cp.Hi << 16 ) | cp.Lo;
		}

		static public implicit operator CodePoint(char c)
		{
			return new CodePoint(c);
		}

		static public implicit operator CodePoint(int i)
		{
			return new CodePoint(i);
		}

		static public implicit operator CodePoint(string s)
		{
			return s.ToCodePoint();
		}
		#region IEquatable implementation
		static public bool operator ==(CodePoint a, CodePoint b)
		{
			return (int)a == (int)b;
		}

		static public bool operator !=(CodePoint a, CodePoint b)
		{
			return (int)a != (int)b;
		}

		public override int GetHashCode()
		{
			return ( Hi << 16 ) | Lo;
		}

		public override bool Equals(object obj)
		{
			return obj is CodePoint && obj != null ? this.Equals((CodePoint)obj) : false;
		}

		public bool Equals(CodePoint other)
		{
			return Hi == other.Hi && Lo == other.Lo;
		}
		#endregion		

		#region IComparable implementation
		public int CompareTo(CodePoint other)
		{
			int diff = Hi - other.Hi;
			return diff == 0 ? Lo - other.Lo : diff;
		}
		#endregion


		unsafe public override string ToString()
		{
			char* cs = stackalloc char[3];
			ToU16(cs);
			return new String(cs);
		}

		

	}

	static public class UniAid
	{
		static public IEnumerable<CodePoint> IterCodePoints(this string s, int idx)
		{
			var len = s.Length;
			while (idx < len)
			{
				char c = s[idx++];
				yield return ( 0xD7FF < c && c < 0xDC00 ) ? 
					new CodePoint(0x10000 + ( ( c & 0x3FF ) << 10 | ( s[idx++] & 0x3FF ) )) : 
						new CodePoint(c);
			}
		}

		static public CodePoint ToCodePoint(this string s, int idx = 0)
		{
			char c = s[idx];
			return  ( c > 0xD7FF && c < 0xDC00 ) ? 
				new CodePoint(0x10000 + ( ( c & 0x3FF ) << 10 | ( s[idx + 1] & 0x3FF ) )) : 
					new CodePoint(c);			
		}

		static public string AsString(this IEnumerable<CodePoint> cps)
		{
			var sb = new StringBuilder();
			foreach (var cp in cps)
			{
				int uni = (int)cp;
				if (uni < 0x10000)
				{
					sb.Append((char)uni);
				}
				else
				{
					uni -= 1000;
					sb.Append(0xD800 | ( uni >> 10 )).Append(0xDC00 | ( uni & 0x3FF ));
				}
			}
			return sb.ToString();
		}

		static public UString ToUString(this string s)
		{
			return new UString(s);
		}

		static public string JoinBy<T>(this IEnumerable<T> seq, string sep)
		{
			return String.Join<T>(sep, seq);
		}


	}
}

