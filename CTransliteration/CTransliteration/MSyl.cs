using System;
using System.Runtime.InteropServices;

namespace Cjk.Phonetic.Mandarin
{

	#region enums
	/// <summary>
	/// Letters and Letter combinations used as Enum symbols are borrowed from GwoyeuRomatzyh
	/// </summary>
	public enum INIT:byte
	{
		Void = 0,
		B = 1,
		P,
		M,
		F,
		V,
		D = 7,
		T,
		N,
		L = 11,
		G = 13,
		K,
		NG,
		H,
		J = 19,
		CH,
		SH = 22,
		R = 23,
		TZ = 25,
		TS,
		S = 28,
	}

	public enum INITGROUP:byte
	{
		_GRP = 0,
		BGRP,
		DGRP,
		GGRP,
		JGRP,
		TZGRP,
	}

	public enum TONE:byte
	{
		CLEAR = 0,
		MUDDY = 0x20,//0b00100000,
		RISING = 0x40,//0b01000000,
		GOING = 0x60,//0b01100000,
		ENTERING = 0x80,//0b10000000,
		NEUTRAL = 0xA0,//0b10100000,
	}

	public enum MED:byte
	{
		_ = 0, 
		I = 0x40,/*0b01000000*/
		U = 0x80,//0b10000000,
		IU = 0xC0,//0b11000000, 
		MAGIC = 255,
	}

	public enum FIN:byte
	{
		ZERO = 0,
		A = 1,
		O,
		E,
		È,
		Y,
		EL,
		AI = 11,
		EI = 13,
		AU = 21,
		OU = 23,
		AN = 31,
		EN = 33,
		ANG = 41,
		ENG = 43,
		AM = 51,
		EM = 53,
	}

	public enum RIME:byte
	{
		ZILCH =  FIN.ZERO,
		A = FIN.A,
		O = FIN.O,
		E = FIN.E,
		EH = FIN.È,
		Y = FIN.Y,
		EL = FIN.EL,
		AI = FIN.AI,
		EI = FIN.EI,
		AU = FIN.AU,
		OU = FIN.OU,
		AN = FIN.AN,
		EN = FIN.EN,
		ANG = FIN.ANG,
		ENG = FIN.ENG,
		AM = FIN.AM,
		EM = FIN.EM,
		I = MED.I,
		IA = MED.I | FIN.A,
		IE = MED.I | FIN.È,
		IO = MED.I | FIN.O,
		IAI = MED.I | FIN.AI,
		IAU = MED.I | FIN.AU,
		IOU = MED.I | FIN.OU,
		IAN = MED.I | FIN.AN,
		IN = MED.I | FIN.EN,
		IANG = MED.I | FIN.ANG,
		ING = MED.I | FIN.ENG,
		IAM = MED.I | FIN.AM,
		IM = MED.I | FIN.EM,
		U = MED.U,
		UA = MED.U | FIN.A,
		UO = MED.U | FIN.O,
		UAI = MED.U | FIN.AI,
		UEI = MED.U | FIN.EI,
		UAN = MED.U | FIN.AN,
		UN = MED.U | FIN.EN,
		UANG = MED.U | FIN.ANG,
		ONG = MED.U | FIN.ENG,
		UEH = MED.U | FIN.È,//This is only for pinyin transcription
		IU = MED.IU,
		IUO = MED.IU | FIN.O,
		IUE = MED.IU | FIN.È,
		IUAN = MED.IU | FIN.AN,
		IUN = MED.IU | FIN.EN,
		IONG = MED.IU | FIN.ENG,
	}
	#endregion

	#region Extions functions
	public static class MandarinSoundPartEx
	{
		static public INITGROUP GetGroup (this INIT init)
		{
			return (INITGROUP)(((byte)init + 5) / 6);
		}

		static public bool IsPalatized (this MED med)
		{
			return (med & MED.I) == MED.I;
		}

		static public bool IsSonorant (this INIT init)
		{
			int pos = ((byte)init) % 6;
			return pos == 3 || pos == 5;
		}
	}
	#endregion

	[StructLayout(LayoutKind.Explicit,Pack=1,Size=2)]
	public struct MSyl : IEquatable<MSyl>
	{
		[FieldOffset(0)]
		ushort
			_code;
		[FieldOffset(0)]
		byte
			_initTone;
		[FieldOffset(1)]
		byte
			_rime;
		internal const byte 	
			IMASK = 0x1F, //0b00011111,
			TMASK = 0xE0, //0b11100000,
			MMASK = 0xC0, //0b11000000,
			FMASK = 0x3F;//0b00111111;
		public static readonly MSyl Default = new MSyl (0);

		#region ctors
		internal MSyl (byte initTone, byte rime)
		{
			_code = 0;//The compiler doesn't know squat about union layout! >8(
			_initTone = initTone;
			_rime = rime;
		}

		internal MSyl (ushort code)
		{
			_initTone = _rime = 0;
			_code = code;
		}

		public MSyl (INIT init, MED med, FIN fin, TONE tone)
			:this((byte)((byte)init|(byte)tone),(byte)((byte)med|(byte)fin))
		{
		}
		#endregion

		public INIT Initial {
			get{ return (INIT)(IMASK & _initTone);}
			set{ _initTone = (byte)(_initTone & TMASK | (byte)value);}
		}

		public TONE Tone {
			get{ return (TONE)(TMASK & _initTone);}
			set{ _initTone = (byte)(_initTone & IMASK | (byte)value);}
		}

		public MED Medial {
			get{ return (MED)(MMASK & _rime); }
			set{ _rime = (byte)(_rime & FMASK | (byte)value);}
		}

		public FIN Final {
			get{ return (FIN)(FMASK & _rime); }
			set{ _rime = (byte)(_rime & MMASK | (byte)value);}
		}

		public RIME Rime {
			get{ return (RIME)_rime; }
			set{ _rime = (byte)value; }
		}

		public bool IsPalatized {
			get{ return (Medial & MED.I) == MED.I; }
		}

		public INITGROUP InitialGroup {
			get{ return Initial.GetGroup ();}
		}

		#region IEquatable implementation
		static public bool operator == (MSyl lhs, MSyl rhs)
		{
			return lhs.Equals (rhs);
		}

		static public bool operator != (MSyl lhs, MSyl rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode ()
		{
			return _code.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			return obj == null ? false :
				obj is MSyl ? this.Equals ((MSyl)obj) : false;
		}

		public bool Equals (MSyl other)
		{
			return this._code == other._code;
		}
		#endregion

		public override string 
		ToString ()
		{
			return Bopomofo.Inst.Transcribe (this);
		}

		public string 
		ToString (IMTransliterator trans)
		{
			return trans.Transcribe (this);
		}
	}

}

