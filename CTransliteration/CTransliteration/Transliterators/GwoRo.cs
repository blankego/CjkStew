using System;
using System.Collections.Generic;

namespace Cjk.Phonetic.Mandarin
{
	public class GwoRo: IMTransliterator
	{
		#region Dictionaries
		protected static readonly TokenDict<INIT> _iDict = new TokenDict<INIT>{
			{"",INIT.Void},
			{"b",INIT.B},{"p",INIT.P},{"m",INIT.M},{"f",INIT.F},{"v",INIT.V},
			{"d",INIT.D},{"t",INIT.T},{"n",INIT.N},{"l",INIT.L},
			{"g",INIT.G},{"k",INIT.K},{"ng",INIT.NG},{"h",INIT.H},
			{"j",INIT.J},{"ch",INIT.CH},{"sh",INIT.SH},{"r",INIT.R},{"gn",INIT.NG},
			{"tz",INIT.TZ},{"ts",INIT.TS},{"s",INIT.S},

		};
		protected static readonly Dictionary<INIT,string> _iTrans = new Dictionary<INIT, string>{
			{INIT.Void,""},
			{INIT.B,"b"},{INIT.P,"p"},{INIT.M,"m"},{INIT.F,"f"},{INIT.V,"v"},
			{INIT.D,"d"},{INIT.T,"t"},{INIT.N,"n"},{INIT.L,"l"},
			{INIT.G,"g"},{INIT.K,"k"},{INIT.NG,"ng"},{INIT.H,"h"},
			{INIT.J,"j"},{INIT.CH,"ch"},{INIT.SH,"sh"},{INIT.R,"r"},
			{INIT.TZ,"tz"},{INIT.TS,"ts"},{INIT.S,"s"},

		};
		protected static readonly TokenDict<MRt> _rDict = new TokenDict<MRt>{
			#region clear
			{"",MRt.ZERO.c},
			{"a",MRt.A.c}, {"o",MRt.O.c}, {"e",MRt.E.c}, {"è",MRt.EH.c},
			{"y",MRt.Y.c}, {"el",MRt.EL.c},
			{"ai",MRt.AI.c}, {"ei",MRt.EI.c}, {"au",MRt.AU.c}, {"ou",MRt.OU.c},
			{"an",MRt.AN.c}, {"en",MRt.EN.c}, {"ang",MRt.ANG.c}, {"eng",MRt.ENG.c},
			{"am",MRt.AM.c}, {"em",MRt.EM.c},
			{"i",MRt.ZERO.I.c}, {"ia",MRt.A.I.c}, {"ie",MRt.EH.I.c}, {"io",MRt.O.I.c},
			{"iai",MRt.AI.I.c}, {"iau",MRt.AU.I.c}, {"iou",MRt.OU.I.c},
			{"ian",MRt.AN.I.c}, {"in",MRt.EN.I.c}, {"iang",MRt.ANG.I.c}, {"ing",MRt.ENG.I.c},
			{"iam",MRt.AM.I.c}, {"im",MRt.EM.I.c},
			{"u",MRt.ZERO.U.c}, {"ua",MRt.A.U.c}, {"uo",MRt.O.U.c},
			{"uai",MRt.AI.U.c}, {"uei",MRt.EI.U.c}, {"uan",MRt.AN.U.c}, {"uen",MRt.EN.U.c},			
			{"uang",MRt.ANG.U.c}, {"ong",MRt.ONG.c},{"ueng",MRt.ONG.c},
			{"iu",MRt.ZERO.IU.c}, {"iuo",MRt.O.IU.c}, {"iue",MRt.EH.IU.c},
			{"iuan",MRt.AN.IU.c}, {"iun",MRt.EN.IU.c}, {"iong",MRt.ENG.IU.c},
			#endregion

			#region muddy
			{"ar",MRt.A.m}, {"or",MRt.O.m}, {"er",MRt.E.m}, {"èr",MRt.EH.m},
			{"yr",MRt.Y.m}, {"erl",MRt.EL.m},
			{"air",MRt.AI.m}, {"eir",MRt.EI.m}, {"aur",MRt.AU.m}, {"our",MRt.OU.m},
			{"arn",MRt.AN.m}, {"ern",MRt.EN.m}, {"arng",MRt.ANG.m}, {"erng",MRt.ENG.m},
			{"arm",MRt.AM.m}, {"erm",MRt.EM.m},
			{"yi",MRt.ZERO.I.m}, {"ya",MRt.A.I.m}, {"ye",MRt.EH.I.m}, {"yo",MRt.O.I.m},
			{"yai",MRt.AI.I.m}, {"yau",MRt.AU.I.m}, {"you",MRt.OU.I.m},
			{"yan",MRt.AN.I.m}, {"yn",MRt.EN.I.m}, {"yang",MRt.ANG.I.m}, {"yng",MRt.ENG.I.m},
			{"yam",MRt.AM.I.m}, {"ym",MRt.EM.I.m},
			{"wu",MRt.ZERO.U.m}, {"wa",MRt.A.U.m}, {"wo",MRt.O.U.m},
			{"wai",MRt.AI.U.m}, {"wei",MRt.EI.U.m}, {"wan",MRt.AN.U.m}, {"wen",MRt.EN.U.m},
			
			{"wang",MRt.ANG.U.m}, {"orng",MRt.ONG.m},{"weng",MRt.ONG.m},
			{"yu",MRt.ZERO.IU.m}, {"yuo",MRt.O.IU.m}, {"yue",MRt.EH.IU.m},
			{"yuan",MRt.AN.IU.m}, {"yun",MRt.EN.IU.m}, {"yong",MRt.ENG.IU.m},
			#endregion

			#region rising
			{"aa",MRt.A.r}, {"oo",MRt.O.r}, {"ee",MRt.E.r}, {"èè",MRt.EH.r},
			{"yy",MRt.Y.r}, {"eel",MRt.EL.r},
			{"ae",MRt.AI.r}, {"eei",MRt.EI.r}, {"ao",MRt.AU.r}, {"oou",MRt.OU.r},
			{"aan",MRt.AN.r}, {"een",MRt.EN.r}, {"aang",MRt.ANG.r}, {"eeng",MRt.ENG.r},
			{"aam",MRt.AM.r}, {"eem",MRt.EM.r},
			{"ii",MRt.ZERO.I.r}, {"ea",MRt.A.I.r}, {"iee",MRt.EH.I.r}, {"eo",MRt.O.I.r},
			{"eai",MRt.AI.I.r}, {"eau",MRt.AU.I.r}, {"eou",MRt.OU.I.r},
			{"ean",MRt.AN.I.r}, {"iin",MRt.EN.I.r}, {"eang",MRt.ANG.I.r}, {"iing",MRt.ENG.I.r},
			{"eam",MRt.AM.I.r}, {"iim",MRt.EM.I.r},
			{"uu",MRt.ZERO.U.r}, {"oa",MRt.A.U.r}, {"uoo",MRt.O.U.r},
			{"oai",MRt.AI.U.r}, {"oei",MRt.EI.U.r}, {"oan",MRt.AN.U.r}, {"oen",MRt.EN.U.r},
			
			{"oang",MRt.ANG.U.r}, {"oong",MRt.ONG.r},{"oeng",MRt.ONG.r},
			{"eu",MRt.ZERO.IU.r}, {"euo",MRt.O.IU.r}, {"eue",MRt.EH.IU.r},
			{"euan",MRt.AN.IU.r}, {"eun",MRt.EN.IU.r}, {"eong",MRt.ENG.IU.r},

			//y,w oblique includes alternative spellings
			{"yii",MRt.ZERO.I.r}, {"yea",MRt.A.I.r}, {"yee",MRt.EH.I.r}, {"yeo",MRt.O.I.r},			
			{"yeai",MRt.AI.I.r}, {"yeau",MRt.AU.I.r}, {"yeou",MRt.OU.I.r},
			{"yean",MRt.AN.I.r}, {"yiin",MRt.EN.I.r}, {"yeang",MRt.ANG.I.r}, {"yiing",MRt.ENG.I.r},
			{"yeam",MRt.AM.I.r}, {"yiim",MRt.EM.I.r},
			{"wuu",MRt.ZERO.U.r}, {"woa",MRt.A.U.r}, {"woo",MRt.O.U.r}, 
			{"woai",MRt.AI.U.r}, {"woei",MRt.EI.U.r}, {"woan",MRt.AN.U.r}, {"woen",MRt.EN.U.r},
			{"woang",MRt.ANG.U.r},{"weeng",MRt.ONG.r},{"woeng",MRt.ONG.r},
			{"yeu",MRt.ZERO.IU.r}, {"yeuo",MRt.O.IU.r}, {"yeue",MRt.EH.IU.r},
			{"yeuan",MRt.AN.IU.r}, {"yeun",MRt.EN.IU.r}, {"yeong",MRt.ENG.IU.r},					
			
			
			{"yaa",MRt.A.I.r}, {"yoo",MRt.O.I.r},
			{"yae",MRt.AI.I.r}, {"yao",MRt.AU.I.r}, {"yoou",MRt.OU.I.r},
			{"yaan",MRt.AN.I.r},{"yaang",MRt.ANG.I.r},
			{"yaam",MRt.AM.I.r},
			{"waa",MRt.A.U.r},
			{"wae",MRt.AI.U.r}, {"weei",MRt.EI.U.r},{"waan",MRt.AN.U.r},{"waang",MRt.ANG.U.r},
			{"yuu",MRt.ZERO.IU.r}, {"yuoo",MRt.O.IU.r}, {"yuee",MRt.EH.IU.r},
			{"yuaan",MRt.AN.IU.r}, {"yuun",MRt.EN.IU.r}, {"yoong",MRt.ENG.IU.r},
			//end oblique
			#endregion

			#region going
			{"ah",MRt.A.g}, {"oh",MRt.O.g}, {"eh",MRt.E.g}, {"èh",MRt.EH.g},
			{"yh",MRt.Y.g}, {"ell",MRt.EL.g},
			{"ay",MRt.AI.g}, {"ey",MRt.EI.g}, {"aw",MRt.AU.g}, {"ow",MRt.OU.g},
			{"ann",MRt.AN.g}, {"enn",MRt.EN.g}, {"anq",MRt.ANG.g}, {"enq",MRt.ENG.g},
			{"amm",MRt.AM.g}, {"emm",MRt.EM.g},
			{"ih",MRt.ZERO.I.g}, {"iah",MRt.A.I.g}, {"ieh",MRt.EH.I.g}, {"ioh",MRt.O.I.g},
			{"iay",MRt.AI.I.g}, {"iaw",MRt.AU.I.g}, {"iow",MRt.OU.I.g},
			{"iann",MRt.AN.I.g}, {"inn",MRt.EN.I.g}, {"ianq",MRt.ANG.I.g}, {"inq",MRt.ENG.I.g},
			{"iamm",MRt.AM.I.g}, {"imm",MRt.EM.I.g},
			{"uh",MRt.ZERO.U.g}, {"uah",MRt.A.U.g}, {"uoh",MRt.O.U.g},
			{"uay",MRt.AI.U.g}, {"uey",MRt.EI.U.g}, {"uann",MRt.AN.U.g}, {"uenn",MRt.EN.U.g},
			
			{"uanq",MRt.ANG.U.g}, {"onq",MRt.ONG.g},{"uenq",MRt.ONG.g},
			{"iuh",MRt.ZERO.IU.g}, {"iuoh",MRt.O.IU.g}, {"iueh",MRt.EH.IU.g},
			{"iuann",MRt.AN.IU.g}, {"iunn",MRt.EN.IU.g}, {"ionq",MRt.ENG.IU.g},

			//y,w oblique
			{"yih",MRt.ZERO.I.g}, {"yah",MRt.A.I.g}, {"yeh",MRt.EH.I.g}, {"yoh",MRt.O.I.g},
			{"yay",MRt.AI.I.g}, {"yaw",MRt.AU.I.g}, {"yow",MRt.OU.I.g},
			{"yann",MRt.AN.I.g}, {"ynn",MRt.EN.I.g}, {"yanq",MRt.ANG.I.g}, {"ynq",MRt.ENG.I.g},
			{"yamm",MRt.AM.I.g}, {"ymm",MRt.EM.I.g},			
			{"wuh",MRt.ZERO.U.g}, {"wah",MRt.A.U.g}, {"woh",MRt.O.U.g},
			{"way",MRt.AI.U.g}, {"wey",MRt.EI.U.g}, {"wann",MRt.AN.U.g}, {"wenn",MRt.EN.U.g},
			
			{"wanq",MRt.ANG.U.g}, {"wenq",MRt.ONG.g},
			{"yuh",MRt.ZERO.IU.g}, {"yuoh",MRt.O.IU.g}, {"yueh",MRt.EH.IU.g},
			{"yuann",MRt.AN.IU.g}, {"yunn",MRt.EN.IU.g}, {"yonq",MRt.ENG.IU.g},

			{"yinn",MRt.EN.I.g},{"yinq",MRt.ENG.I.g},{"yimm",MRt.EM.I.g},
			//end oblique
			#endregion

			#region entering
			{"aq",MRt.A.e}, {"oq",MRt.O.e}, {"eq",MRt.E.e}, {"èq",MRt.EH.e},
			{"yq",MRt.Y.e},			
			{"iq",MRt.ZERO.I.e}, {"iaq",MRt.A.I.e}, {"ieq",MRt.EH.I.e}, {"ioq",MRt.O.I.e},			
			{"uq",MRt.ZERO.U.e}, {"uaq",MRt.A.U.e}, {"ueq",MRt.E.U.e}, {"uoq",MRt.O.U.e},
			{"iuq",MRt.ZERO.IU.e}, {"iuoq",MRt.O.IU.e}, {"iueq",MRt.EH.IU.e},

			//y w oblique
			{"yiq",MRt.ZERO.I.e}, {"yaq",MRt.A.I.e}, {"yeq",MRt.EH.I.e}, {"yoq",MRt.O.I.e},			
			{"wuq",MRt.ZERO.U.e}, {"waq",MRt.A.U.e}, {"weq",MRt.E.U.e}, {"woq",MRt.O.U.e},
			{"yuq",MRt.ZERO.IU.e}, {"yuoq",MRt.O.IU.e}, {"yueq",MRt.EH.IU.e}
			//end oblique
			#endregion
		} ;
		protected static readonly Dictionary<MRt,string> _ywObliqueTrans = new Dictionary<MRt,string>{
			{MRt.ENG.U.c,"ueng"},{MRt.ENG.U.m,"weng"},

			{MRt.ZERO.I.r,"yii"}, {MRt.A.I.r,"yea"}, {MRt.EH.I.r,"yee"}, {MRt.O.I.r,"yeo"},			
			{MRt.AI.I.r,"yeai"}, {MRt.AU.I.r,"yeau"}, {MRt.OU.I.r,"yeou"},
			{MRt.AN.I.r,"yean"}, {MRt.EN.I.r,"yiin"}, {MRt.ANG.I.r,"yeang"}, {MRt.ENG.I.r,"yiing"},
			{MRt.AM.I.r,"yeam"}, {MRt.EM.I.r,"yiim"},
			{MRt.ZERO.U.r,"wuu"}, {MRt.A.U.r,"woa"}, {MRt.O.U.r,"woo"}, 
			{MRt.AI.U.r,"woai"}, {MRt.EI.U.r,"woei"}, {MRt.AN.U.r,"woan"}, {MRt.EN.U.r,"woen"},
			
			{MRt.ANG.U.r,"woang"}, {MRt.ONG.r,"woeng"},
			{MRt.ZERO.IU.r,"yeu"}, {MRt.O.IU.r,"yeuo"}, {MRt.EH.IU.r,"yeue"},
			{MRt.AN.IU.r,"yeuan"}, {MRt.EN.IU.r,"yeun"}, {MRt.ENG.IU.r,"yeong"},

			{MRt.ZERO.I.g,"yih"}, {MRt.A.I.g,"yah"}, {MRt.EH.I.g,"yeh"}, {MRt.O.I.g,"yoh"},
			{MRt.AI.I.g,"yay"}, {MRt.AU.I.g,"yaw"}, {MRt.OU.I.g,"yow"},
			{MRt.AN.I.g,"yann"}, {MRt.EN.I.g,"yinn"}, {MRt.ANG.I.g,"yanq"}, {MRt.ENG.I.g,"yinq"},
			{MRt.AM.I.g,"yamm"}, {MRt.EM.I.g,"yimm"},			
			{MRt.ZERO.U.g,"wuh"}, {MRt.A.U.g,"wah"}, {MRt.O.U.g,"woh"},
			{MRt.AI.U.g,"way"}, {MRt.EI.U.g,"wey"}, {MRt.AN.U.g,"wann"}, {MRt.EN.U.g,"wenn"},
			
			{MRt.ANG.U.g,"wanq"}, {MRt.ONG.g,"wenq"},
			{MRt.ZERO.IU.g,"yuh"}, {MRt.O.IU.g,"yuoh"}, {MRt.EH.IU.g,"yueh"},
			{MRt.AN.IU.g,"yuann"}, {MRt.EN.IU.g,"yunn"}, {MRt.ENG.IU.g,"yonq"},

			{MRt.ZERO.I.e,"yiq"}, {MRt.A.I.e,"yaq"}, {MRt.EH.I.e,"yeq"}, {MRt.O.I.e,"yoq"},			
			{MRt.ZERO.U.e,"wuq"}, {MRt.A.U.e,"waq"}, {MRt.E.U.e,"weq"}, {MRt.O.U.e,"woq"},
			{MRt.ZERO.IU.e,"yuq"}, {MRt.O.IU.e,"yuoq"}, {MRt.EH.IU.e,"yueq"}		
				
		};
		protected static readonly Dictionary<MRt, string> _rTrans = new Dictionary<MRt,string>{
			{MRt.ZERO.c, ""},
			{MRt.A.c, "a"}, {MRt.O.c, "o"}, {MRt.E.c, "e"}, {MRt.EH.c, "è"},
			{MRt.Y.c, "y"}, {MRt.EL.c, "el"},
			{MRt.AI.c, "ai"}, {MRt.EI.c, "ei"}, {MRt.AU.c, "au"}, {MRt.OU.c, "ou"},
			{MRt.AN.c, "an"}, {MRt.EN.c, "en"}, {MRt.ANG.c, "ang"}, {MRt.ENG.c, "eng"},
			{MRt.AM.c, "am"}, {MRt.EM.c, "em"},
			{MRt.ZERO.I.c, "i"}, {MRt.A.I.c, "ia"}, {MRt.EH.I.c, "ie"}, {MRt.O.I.c, "io"},
			{MRt.AI.I.c, "iai"}, {MRt.AU.I.c, "iau"}, {MRt.OU.I.c, "iou"},
			{MRt.AN.I.c, "ian"}, {MRt.EN.I.c, "in"}, {MRt.ANG.I.c, "iang"}, {MRt.ENG.I.c, "ing"},
			{MRt.AM.I.c, "iam"}, {MRt.EM.I.c, "im"},
			{MRt.ZERO.U.c, "u"}, {MRt.A.U.c, "ua"}, {MRt.O.U.c, "uo"},
			{MRt.AI.U.c, "uai"}, {MRt.EI.U.c, "uei"}, {MRt.AN.U.c, "uan"}, {MRt.EN.U.c, "uen"},			
			{MRt.ANG.U.c, "uang"}, {MRt.ONG.c, "ong"},
			{MRt.ZERO.IU.c, "iu"}, {MRt.O.IU.c, "iuo"}, {MRt.EH.IU.c, "iue"},
			{MRt.AN.IU.c, "iuan"}, {MRt.EN.IU.c, "iun"}, {MRt.ENG.IU.c, "iong"},

			{MRt.A.m, "ar"}, {MRt.O.m, "or"}, {MRt.E.m, "er"}, {MRt.EH.m, "èr"},
			{MRt.Y.m, "yr"}, {MRt.EL.m, "erl"},
			{MRt.AI.m, "air"}, {MRt.EI.m, "eir"}, {MRt.AU.m, "aur"}, {MRt.OU.m, "our"},
			{MRt.AN.m, "arn"}, {MRt.EN.m, "ern"}, {MRt.ANG.m, "arng"}, {MRt.ENG.m, "erng"},
			{MRt.AM.m, "arm"}, {MRt.EM.m, "erm"},
			{MRt.ZERO.I.m, "yi"}, {MRt.A.I.m, "ya"}, {MRt.EH.I.m, "ye"}, {MRt.O.I.m, "yo"},
			{MRt.AI.I.m, "yai"}, {MRt.AU.I.m, "yau"}, {MRt.OU.I.m, "you"},
			{MRt.AN.I.m, "yan"}, {MRt.EN.I.m, "yn"}, {MRt.ANG.I.m, "yang"}, {MRt.ENG.I.m, "yng"},
			{MRt.AM.I.m, "yam"}, {MRt.EM.I.m, "ym"},
			{MRt.ZERO.U.m, "wu"}, {MRt.A.U.m, "wa"}, {MRt.O.U.m, "wo"},
			{MRt.AI.U.m, "wai"}, {MRt.EI.U.m, "wei"}, {MRt.AN.U.m, "wan"}, {MRt.EN.U.m, "wen"},
			
			{MRt.ANG.U.m, "wang"}, {MRt.ONG.m, "orng"},
			{MRt.ZERO.IU.m, "yu"}, {MRt.O.IU.m, "yuo"}, {MRt.EH.IU.m, "yue"},
			{MRt.AN.IU.m, "yuan"}, {MRt.EN.IU.m, "yun"}, {MRt.ENG.IU.m, "yong"},

			{MRt.A.r, "aa"}, {MRt.O.r, "oo"}, {MRt.E.r, "ee"}, {MRt.EH.r, "èè"},
			{MRt.Y.r, "yy"}, {MRt.EL.r, "eel"},
			{MRt.AI.r, "ae"}, {MRt.EI.r, "eei"}, {MRt.AU.r, "ao"}, {MRt.OU.r, "oou"},
			{MRt.AN.r, "aan"}, {MRt.EN.r, "een"}, {MRt.ANG.r, "aang"}, {MRt.ENG.r, "eeng"},
			{MRt.AM.r, "aam"}, {MRt.EM.r, "eem"},
			{MRt.ZERO.I.r, "ii"}, {MRt.A.I.r, "ea"}, {MRt.EH.I.r, "iee"}, {MRt.O.I.r, "eo"},
			{MRt.AI.I.r, "eai"}, {MRt.AU.I.r, "eau"}, {MRt.OU.I.r, "eou"},
			{MRt.AN.I.r, "ean"}, {MRt.EN.I.r, "iin"}, {MRt.ANG.I.r, "eang"}, {MRt.ENG.I.r, "iing"},
			{MRt.AM.I.r, "eam"}, {MRt.EM.I.r, "iim"},
			{MRt.ZERO.U.r, "uu"}, {MRt.A.U.r, "oa"}, {MRt.O.U.r, "uoo"},
			{MRt.AI.U.r, "oai"}, {MRt.EI.U.r, "oei"}, {MRt.AN.U.r, "oan"}, {MRt.EN.U.r, "oen"},
			
			{MRt.ANG.U.r, "oang"}, {MRt.ONG.r, "oong"},
			{MRt.ZERO.IU.r, "eu"}, {MRt.O.IU.r, "euo"}, {MRt.EH.IU.r, "eue"},
			{MRt.AN.IU.r, "euan"}, {MRt.EN.IU.r, "eun"}, {MRt.ENG.IU.r, "eong"},

			{MRt.A.g, "ah"}, {MRt.O.g, "oh"}, {MRt.E.g, "eh"}, {MRt.EH.g, "èh"},
			{MRt.Y.g, "yh"}, {MRt.EL.g, "ell"},
			{MRt.AI.g, "ay"}, {MRt.EI.g, "ey"}, {MRt.AU.g, "aw"}, {MRt.OU.g, "ow"},
			{MRt.AN.g, "ann"}, {MRt.EN.g, "enn"}, {MRt.ANG.g, "anq"}, {MRt.ENG.g, "enq"},
			{MRt.AM.g, "amm"}, {MRt.EM.g, "emm"},
			{MRt.ZERO.I.g, "ih"}, {MRt.A.I.g, "iah"}, {MRt.EH.I.g, "ieh"}, {MRt.O.I.g, "ioh"},
			{MRt.AI.I.g, "iay"}, {MRt.AU.I.g, "iaw"}, {MRt.OU.I.g, "iow"},
			{MRt.AN.I.g, "iann"}, {MRt.EN.I.g, "inn"}, {MRt.ANG.I.g, "ianq"}, {MRt.ENG.I.g, "inq"},
			{MRt.AM.I.g, "iamm"}, {MRt.EM.I.g, "imm"},
			{MRt.ZERO.U.g, "uh"}, {MRt.A.U.g, "uah"}, {MRt.O.U.g, "uoh"},
			{MRt.AI.U.g, "uay"}, {MRt.EI.U.g, "uey"}, {MRt.AN.U.g, "uann"}, {MRt.EN.U.g, "uenn"},
			
			{MRt.ANG.U.g, "uanq"}, {MRt.ONG.g, "onq"},
			{MRt.ZERO.IU.g, "iuh"}, {MRt.O.IU.g, "iuoh"}, {MRt.EH.IU.g, "iueh"},
			{MRt.AN.IU.g, "iuann"}, {MRt.EN.IU.g, "iunn"}, {MRt.ENG.IU.g, "ionq"},

			{MRt.A.e, "aq"}, {MRt.O.e, "oq"}, {MRt.E.e, "eq"}, {MRt.EH.e, "èq"},
			{MRt.Y.e, "yq"},			
			{MRt.ZERO.I.e, "iq"}, {MRt.A.I.e, "iaq"}, {MRt.EH.I.e, "ieq"}, {MRt.O.I.e, "ioq"},			
			{MRt.ZERO.U.e, "uq"}, {MRt.A.U.e, "uaq"}, {MRt.E.U.e, "ueq"}, {MRt.O.U.e, "uoq"},
			{MRt.ZERO.IU.e, "iuq"}, {MRt.O.IU.e, "iuoq"}, {MRt.EH.IU.e, "iueq"}

		} ;

		#endregion
		static public readonly GwoRo Inst = new GwoRo ();

		public GwoRo ()	{}

		#region IMTransliterator implementation
		public MSyl 
		MunchSyllable (string s, ref int idx)
		{
			int mark = idx;
			bool neutral = s [idx] == '.' ? true : false;
			if (neutral)
				++idx;
			INIT init;
			MRt rt;
			_iDict.TryMatchStart (s, ref idx, out init);
			bool sonorantClear = false;
			if (s [idx] == 'h') {//mh nh ngh rh lh
				idx++;
				sonorantClear = true;
			}

			_rDict.TryMatchStart (s, ref idx, out rt);
			if (init == INIT.Void && rt.Equals (MRt.Default)) {
				idx = mark;
				return MSyl.Default;
			}
			if (init.GetGroup () == INITGROUP.JGRP && rt.Medial.IsPalatized () && init != INIT.R) {//ji chi shi
				init = init == INIT.J ? INIT.G : init == INIT.CH ? INIT.K : INIT.H; 
			} else if (init.IsSonorant () && rt.Tone == TONE.CLEAR) {// sonorants check
				rt.Tone = sonorantClear ? TONE.CLEAR : TONE.MUDDY;
			}
			return new MSyl (init, rt.Medial, rt.Final, neutral ? TONE.NEUTRAL : rt.Tone);


		}

		public IList<AltMSyl> 
		Parse (string s)
		{
			throw new System.NotImplementedException ();
		}

		public string 
		Transcribe (MSyl syl)
		{
			string init = "";
			INIT i = syl.Initial;
			TONE t = syl.Tone;
			if (t == TONE.NEUTRAL) {
				init = ".";
				t = TONE.CLEAR; 
			}

			try {
				if (i == INIT.Void) {
					if (syl.Medial > MED._)
					if (t > TONE.MUDDY && t < TONE.NEUTRAL || syl.Rime == RIME.ONG) {//oblique forms
						return init + _ywObliqueTrans [new MRt (syl.Rime, t)];
					}					
				} else {
					if (syl.InitialGroup == INITGROUP.GGRP && syl.IsPalatized) {//gi ki hi gni
						init += i == INIT.G ? "j" : i == INIT.K ? "ch" : i == INIT.H ? "sh" : "gn";
					} else {
						init += _iTrans [i];
						if (i.IsSonorant ()) {
							if (syl.Tone == TONE.CLEAR)
								init += "h";
							else if (syl.Tone == TONE.MUDDY)
								t = TONE.CLEAR;
						}
					}
				}

				return init + _rTrans [new MRt (syl.Rime, t)];
			} catch (Exception) {
				throw new ArgumentException (
					string.Format ("Try to transcribe a invalid mandarin syllable:{0} into GwoyeuRomatzyh!", syl));
			}
		}
		#endregion

	}
}

