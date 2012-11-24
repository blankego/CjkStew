using System;
using System.Collections.Generic;

namespace Cjk.Phonetic.Mandarin
{
	public class Pinyin : IMTransliterator
	{
		#region Dictionaries
		protected static  TokenDict<MImt> _miDict =  new TokenDict<MImt>{
			{"b", MImt.B._.n }, {"p", MImt.P._.n },
			{"m", MImt.M._.n }, {"f", MImt.F._.n }, {"v", MImt.V._.n },
			{"d", MImt.D._.n }, {"t", MImt.T._.n },
			{"n", MImt.N._.n }, {"l", MImt.L._.n },
			{"g", MImt.G._.n }, {"k", MImt.K._.n },
			{"ng", MImt.NG._.n }, {"h", MImt.H._.n },
			{"zh", MImt.J._.n }, {"ch", MImt.CH._.n },
			{"sh", MImt.SH._.n }, {"r", MImt.R._.n },
			{"z", MImt.TZ._.n }, {"c", MImt.TS._.n }, {"s", MImt.S._.n },

			{"bi", MImt.B.I.n }, {"pi", MImt.P.I.n }, {"mi", MImt.M.I.n },
			{"fi",MImt.F.I.n},
			{"di", MImt.D.I.n }, {"ti", MImt.T.I.n },
			{"ni", MImt.N.I.n }, {"li", MImt.L.I.n },
			{"ji", MImt.G.I.n }, {"qi", MImt.K.I.n },
			{"gni", MImt.NG.I.n }, {"xi", MImt.H.I.n },
			{"zyi", MImt.TZ.I.n }, {"cyi", MImt.TS.I.n }, {"syi", MImt.S.I.n },
			{"y", MImt.Void.I.n }, {"yi", MImt.Void.I.n },
			{"bu", MImt.B.U.n }, {"pu", MImt.P.U.n }, {"mu", MImt.M.U.n }, {"fu", MImt.F.U.n }, {"vu", MImt.V.U.n },
			{"du", MImt.D.U.n }, {"tu", MImt.T.U.n }, {"nu", MImt.N.U.n }, {"lu", MImt.L.U.n },
			{"gu", MImt.G.U.n }, {"ku", MImt.K.U.n },
			{"ngu", MImt.NG.U.n }, {"hu", MImt.H.U.n },
			{"zhu", MImt.J.U.n }, {"chu", MImt.CH.U.n }, {"shu", MImt.SH.U.n },
			{"ru", MImt.R.U.n },
			{"zu", MImt.TZ.U.n }, {"cu", MImt.TS.U.n }, {"su", MImt.S.U.n },
			{"w", MImt.Void.U.n }, {"wu", MImt.Void.U.n },
			{"nü", MImt.N.IU.n }, {"lü", MImt.L.IU.n },
			{"ju", MImt.G.IU.n }, {"qu", MImt.K.IU.n },
			{"gnu", MImt.NG.IU.n }, {"xu", MImt.H.IU.n },
			{"zü", MImt.TZ.IU.n }, {"cü", MImt.TS.IU.n }, {"sü", MImt.S.IU.n },
			{"yu", MImt.Void.IU.n },
//			//use magic value to substitute the final y for zhy chy shy ry zy cy sy
//			{"zhi", MImt.J.MAGIC.n }, {"chi", MImt.CH.MAGIC.n },
//			{"shi", MImt.SH.MAGIC.n }, {"ri", MImt.R.MAGIC.n },
//			{"zi", MImt.TZ.MAGIC.n }, {"ci", MImt.TS.MAGIC.n }, {"si", MImt.S.MAGIC.n },


			{"bī", MImt.B.I.c }, {"pī", MImt.P.I.c }, {"mī", MImt.M.I.c },
			{"fī",MImt.F.I.c},
			{"dī", MImt.D.I.c }, {"tī", MImt.T.I.c },
			{"nī", MImt.N.I.c }, {"lī", MImt.L.I.c },
			{"jī", MImt.G.I.c }, {"qī", MImt.K.I.c },
			{"gnī", MImt.NG.I.c }, {"xī", MImt.H.I.c },
			{"zyī", MImt.TZ.I.c }, {"cyī", MImt.TS.I.c }, {"syī", MImt.S.I.c },
			{"yī", MImt.Void.I.c },
			{"bū", MImt.B.U.c }, {"pū", MImt.P.U.c }, {"mū", MImt.M.U.c }, {"fū", MImt.F.U.c }, {"vū", MImt.V.U.c },
			{"dū", MImt.D.U.c }, {"tū", MImt.T.U.c }, {"nū", MImt.N.U.c }, {"lū", MImt.L.U.c },
			{"gū", MImt.G.U.c }, {"kū", MImt.K.U.c },
			{"ngū", MImt.NG.U.c }, {"hū", MImt.H.U.c },
			{"zhū", MImt.J.U.c }, {"chū", MImt.CH.U.c }, {"shū", MImt.SH.U.c },
			{"rū", MImt.R.U.c },
			{"zū", MImt.TZ.U.c }, {"cū", MImt.TS.U.c }, {"sū", MImt.S.U.c },
			{"wū", MImt.Void.U.c },
			{"nǖ", MImt.N.IU.c }, {"lǖ", MImt.L.IU.c },
			{"jū", MImt.G.IU.c }, {"qū", MImt.K.IU.c },
			{"gnū", MImt.NG.IU.c }, {"xū", MImt.H.IU.c },
			{"zǖ", MImt.TZ.IU.c }, {"cǖ", MImt.TS.IU.c }, {"sǖ", MImt.S.IU.c },
			{"yū", MImt.Void.IU.c },

//			{"zhī", MImt.J.MAGIC.c }, {"chī", MImt.CH.MAGIC.c },
//			{"shī", MImt.SH.MAGIC.c }, {"rī", MImt.R.MAGIC.c },
//			{"zī", MImt.TZ.MAGIC.c }, {"cī", MImt.TS.MAGIC.c }, {"sī", MImt.S.MAGIC.c },
//

			{"bí", MImt.B.I.m }, {"pí", MImt.P.I.m }, {"mí", MImt.M.I.m },
			{"fí",MImt.F.I.m},
			{"dí", MImt.D.I.m }, {"tí", MImt.T.I.m },
			{"ní", MImt.N.I.m }, {"lí", MImt.L.I.m },
			{"jí", MImt.G.I.m }, {"qí", MImt.K.I.m },
			{"gní", MImt.NG.I.m }, {"xí", MImt.H.I.m },
			{"zyí", MImt.TZ.I.m }, {"cyí", MImt.TS.I.m }, {"syí", MImt.S.I.m },
			{"yí", MImt.Void.I.m },
			{"bú", MImt.B.U.m }, {"pú", MImt.P.U.m }, {"mú", MImt.M.U.m }, {"fú", MImt.F.U.m }, {"vú", MImt.V.U.m },
			{"dú", MImt.D.U.m }, {"tú", MImt.T.U.m }, {"nú", MImt.N.U.m }, {"lú", MImt.L.U.m },
			{"gú", MImt.G.U.m }, {"kú", MImt.K.U.m },
			{"ngú", MImt.NG.U.m }, {"hú", MImt.H.U.m },
			{"zhú", MImt.J.U.m }, {"chú", MImt.CH.U.m }, {"shú", MImt.SH.U.m },
			{"rú", MImt.R.U.m },
			{"zú", MImt.TZ.U.m }, {"cú", MImt.TS.U.m }, {"sú", MImt.S.U.m },
			{"wú", MImt.Void.U.m },
			{"nǘ", MImt.N.IU.m }, {"lǘ", MImt.L.IU.m },
			{"jú", MImt.G.IU.m }, {"qú", MImt.K.IU.m },
			{"gnú", MImt.NG.IU.m }, {"xú", MImt.H.IU.m },
			{"zǘ", MImt.TZ.IU.m }, {"cǘ", MImt.TS.IU.m }, {"sǘ", MImt.S.IU.m },
			{"yú", MImt.Void.IU.m },
//
//			{"zhí", MImt.J.MAGIC.m }, {"chí", MImt.CH.MAGIC.m },
//			{"shí", MImt.SH.MAGIC.m }, {"rí", MImt.R.MAGIC.m },
//			{"zí", MImt.TZ.MAGIC.m }, {"cí", MImt.TS.MAGIC.m }, {"sí", MImt.S.MAGIC.m },


			{"bǐ", MImt.B.I.r }, {"pǐ", MImt.P.I.r }, {"mǐ", MImt.M.I.r },
			{"fǐ",MImt.F.I.r},
			{"dǐ", MImt.D.I.r }, {"tǐ", MImt.T.I.r },
			{"nǐ", MImt.N.I.r }, {"lǐ", MImt.L.I.r },
			{"jǐ", MImt.G.I.r }, {"qǐ", MImt.K.I.r },
			{"gnǐ", MImt.NG.I.r }, {"xǐ", MImt.H.I.r },
			{"zyǐ", MImt.TZ.I.r }, {"cyǐ", MImt.TS.I.r }, {"syǐ", MImt.S.I.r },
			{"yǐ", MImt.Void.I.r },
			{"bǔ", MImt.B.U.r }, {"pǔ", MImt.P.U.r }, {"mǔ", MImt.M.U.r }, {"fǔ", MImt.F.U.r }, {"vǔ", MImt.V.U.r },
			{"dǔ", MImt.D.U.r }, {"tǔ", MImt.T.U.r }, {"nǔ", MImt.N.U.r }, {"lǔ", MImt.L.U.r },
			{"gǔ", MImt.G.U.r }, {"kǔ", MImt.K.U.r },
			{"ngǔ", MImt.NG.U.r }, {"hǔ", MImt.H.U.r },
			{"zhǔ", MImt.J.U.r }, {"chǔ", MImt.CH.U.r }, {"shǔ", MImt.SH.U.r },
			{"rǔ", MImt.R.U.r },
			{"zǔ", MImt.TZ.U.r }, {"cǔ", MImt.TS.U.r }, {"sǔ", MImt.S.U.r },
			{"wǔ", MImt.Void.U.r },
			{"nǚ", MImt.N.IU.r }, {"lǚ", MImt.L.IU.r },
			{"jǔ", MImt.G.IU.r }, {"qǔ", MImt.K.IU.r },
			{"gnǔ", MImt.NG.IU.r }, {"xǔ", MImt.H.IU.r },
			{"zǚ", MImt.TZ.IU.r }, {"cǚ", MImt.TS.IU.r }, {"sǚ", MImt.S.IU.r },
			{"yǔ", MImt.Void.IU.r },

//			{"zhǐ", MImt.J.MAGIC.r }, {"chǐ", MImt.CH.MAGIC.r },
//			{"shǐ", MImt.SH.MAGIC.r }, {"rǐ", MImt.R.MAGIC.r },
//			{"zǐ", MImt.TZ.MAGIC.r }, {"cǐ", MImt.TS.MAGIC.r }, {"sǐ", MImt.S.MAGIC.r },
//
//
			{"bì", MImt.B.I.g }, {"pì", MImt.P.I.g }, {"mì", MImt.M.I.g },
			{"fì",MImt.F.I.g},
			{"dì", MImt.D.I.g }, {"tì", MImt.T.I.g },
			{"nì", MImt.N.I.g }, {"lì", MImt.L.I.g },
			{"jì", MImt.G.I.g }, {"qì", MImt.K.I.g },
			{"gnì", MImt.NG.I.g }, {"xì", MImt.H.I.g },
			{"zyì", MImt.TZ.I.g }, {"cyì", MImt.TS.I.g }, {"syì", MImt.S.I.g },
			{"yì", MImt.Void.I.g },
			{"bù", MImt.B.U.g }, {"pù", MImt.P.U.g }, {"mù", MImt.M.U.g }, {"fù", MImt.F.U.g }, {"vù", MImt.V.U.g },
			{"dù", MImt.D.U.g }, {"tù", MImt.T.U.g }, {"nù", MImt.N.U.g }, {"lù", MImt.L.U.g },
			{"gù", MImt.G.U.g }, {"kù", MImt.K.U.g },
			{"ngù", MImt.NG.U.g }, {"hù", MImt.H.U.g },
			{"zhù", MImt.J.U.g }, {"chù", MImt.CH.U.g }, {"shù", MImt.SH.U.g },
			{"rù", MImt.R.U.g },
			{"zù", MImt.TZ.U.g }, {"cù", MImt.TS.U.g }, {"sù", MImt.S.U.g },
			{"wù", MImt.Void.U.g },
			{"nǜ", MImt.N.IU.g }, {"lǜ", MImt.L.IU.g },
			{"jù", MImt.G.IU.g }, {"qù", MImt.K.IU.g },
			{"gnù", MImt.NG.IU.g }, {"xù", MImt.H.IU.g },
			{"zǜ", MImt.TZ.IU.g }, {"cǜ", MImt.TS.IU.g }, {"sǜ", MImt.S.IU.g },
			{"yù", MImt.Void.IU.g },
//
//			{"zhì", MImt.J.MAGIC.g }, {"chì", MImt.CH.MAGIC.g },
//			{"shì", MImt.SH.MAGIC.g }, {"rì", MImt.R.MAGIC.g },
//			{"zì", MImt.TZ.MAGIC.g }, {"cì", MImt.TS.MAGIC.g }, {"sì", MImt.S.MAGIC.g },
//
		};

		
		protected static TokenDict<MRt> _mfDict = new TokenDict<MRt>{
			{"a", MRt.A.n}, {"o", MRt.O.n}, {"e", MRt.E.n}, {"ê", MRt.EH.n}, {"er", MRt.EL.n},
			{"r", MRt.EL.n},
			{"ai", MRt.AI.n}, {"ei", MRt.EI.n}, {"i", MRt.Y.n}, {"ao", MRt.AU.n}, {"ou", MRt.OU.n},
			{"u", MRt.ZERO.U.n},
			{"an", MRt.AN.n}, {"en", MRt.EN.n}, {"ang", MRt.ANG.n}, {"eng", MRt.ENG.n},
			{"am", MRt.AM.n}, {"em", MRt.EM.n}, {"n", MRt.EN.n}, {"ng", MRt.ENG.n}, {"m", MRt.EM.n},
			//magic value
			{"ong", MRt.ONG.n},

			{"ā", MRt.A.c}, {"ō", MRt.O.c}, {"ē", MRt.E.c}, {"ê¯", MRt.EH.c}, {"ēr", MRt.EL.c},
			{"āi", MRt.AI.c}, {"ēi", MRt.EI.c}, {"ī", MRt.Y.c}, {"āo", MRt.AU.c}, {"ōu", MRt.OU.c}, {"ū", MRt.ZERO.U.c},
			{"ān", MRt.AN.c}, {"ēn", MRt.EN.c}, {"āng", MRt.ANG.c}, {"ēng", MRt.ENG.c}, {"ām", MRt.AM.c},
			{"ēm", MRt.EM.c},
			{"ōng", MRt.ONG.c},

			{"á", MRt.A.m}, {"ó", MRt.O.m}, {"é", MRt.E.m}, {"ê´", MRt.EH.m}, {"ér", MRt.EL.m},
			{"ái", MRt.AI.m}, {"éi", MRt.EI.m}, {"í", MRt.Y.m}, {"áo", MRt.AU.m}, {"óu", MRt.OU.m}, {"ú", MRt.ZERO.U.m},
			{"án", MRt.AN.m}, {"én", MRt.EN.m}, {"áng", MRt.ANG.m}, {"éng", MRt.ENG.m}, {"ám", MRt.AM.m},
			{"ém", MRt.EM.m},
			{"óng", MRt.ONG.m},

			{"ǎ", MRt.A.r}, {"ǒ", MRt.O.r}, {"ě", MRt.E.r}, {"êˇ", MRt.EH.r}, {"ěr", MRt.EL.r},
			{"ǎi", MRt.AI.r}, {"ěi", MRt.EI.r}, {"ǐ", MRt.Y.r}, {"ǎo", MRt.AU.r}, {"ǒu", MRt.OU.r}, {"ǔ", MRt.ZERO.U.r},
			{"ǎn", MRt.AN.r}, {"ěn", MRt.EN.r}, {"ǎng", MRt.ANG.r}, {"ěng", MRt.ENG.r}, {"ǎm", MRt.AM.r},
			{"ěm", MRt.EM.r},
			{"ǒng", MRt.ONG.r},

			{"à", MRt.A.g}, {"ò", MRt.O.g}, {"è", MRt.E.g}, {"ê`", MRt.EH.g}, {"èr", MRt.EL.g},
			{"ài", MRt.AI.g}, {"èi", MRt.EI.g}, {"ì", MRt.Y.g}, {"ào", MRt.AU.g}, {"òu", MRt.OU.g}, {"ù", MRt.ZERO.U.g},
			{"àn", MRt.AN.g}, {"èn", MRt.EN.g}, {"àng", MRt.ANG.g}, {"èng", MRt.ENG.g}, {"àm", MRt.AM.g},
			{"èm", MRt.EM.g},
			{"òng", MRt.ONG.g},

			{"ah", MRt.A.e}, {"oh", MRt.O.e}, {"eh", MRt.E.e}, {"êh", MRt.EH.e},
			{"h", MRt.ZERO.e},

		};
		protected static readonly string[] 
			_ui = {"ī", "í", "ǐ", "ì", "", "i"},
			_iu = {"ū", "ú", "ǔ", "ù", "", "u"};

		protected static readonly Dictionary<INIT, string> _miTrans = new Dictionary<INIT,string>{
			{INIT.Void, ""},
			{INIT.B, "b"},
			{INIT.P, "p"},
			{INIT.M, "m"},
			{INIT.F, "f"},
			{INIT.V, "v"},
			{INIT.D, "d"},
			{INIT.T, "t"},
			{INIT.N, "n"},
			{INIT.L, "l"},
			{INIT.G, "g"},
			{INIT.K, "k"},
			{INIT.NG, "ng"},
			{INIT.H, "h"},
			{INIT.J, "zh"},
			{INIT.CH, "ch"},
			{INIT.SH, "sh"},
			{INIT.R, "r"},
			{INIT.TZ, "z"},
			{INIT.TS, "c"},
			{INIT.S, "s"},
		};

		protected static readonly Dictionary<INIT, string> _specInit = new Dictionary<INIT,string>{
			{INIT.G, "j"}, {INIT.K, "q"}, {INIT.NG, "gn"}, {INIT.H, "x"}, {INIT.TZ, "zy"}, {INIT.TS, "cy"}, {INIT.S, "sy"},
		};
		

		protected static readonly Dictionary<MRt, string> _m0Trans = new Dictionary<MRt, string> {
			{MRt.ZERO.I.n, "yi"}, {MRt.A.I.n, "ya"}, {MRt.EH.I.n, "ye"}, {MRt.O.I.n, "yo"},
			{MRt.AI.I.n, "yai"}, {MRt.AU.I.n, "yao"}, {MRt.OU.I.n, "you"},
			{MRt.AN.I.n, "yan"}, {MRt.EN.I.n, "yin"}, {MRt.ANG.I.n, "yang"}, {MRt.ENG.I.n, "ying"},
			{MRt.AM.I.n, "yam"}, {MRt.EM.I.n, "yim"},
			{MRt.ZERO.U.n, "wu"}, {MRt.A.U.n, "wa"}, {MRt.O.U.n, "wo"},
			{MRt.AI.U.n, "wai"}, {MRt.EI.U.n, "wei"}, {MRt.AN.U.n, "wan"}, {MRt.EN.U.n, "wen"},
			{MRt.ANG.U.n, "wang"}, {MRt.ONG.n, "weng"},
			{MRt.ZERO.IU.n, "yu"}, {MRt.O.IU.n, "yuo"}, {MRt.EH.IU.n, "yue"},
			{MRt.AN.IU.n, "yuan"}, {MRt.EN.IU.n, "yun"}, {MRt.ENG.IU.n, "yong"},

			{MRt.ZERO.I.c, "yī"}, {MRt.A.I.c, "yā"}, {MRt.EH.I.c, "yē"}, {MRt.O.I.c, "yō"},
			{MRt.AI.I.c, "yāi"}, {MRt.AU.I.c, "yāo"}, {MRt.OU.I.c, "yōu"},
			{MRt.AN.I.c, "yān"}, {MRt.EN.I.c, "yīn"}, {MRt.ANG.I.c, "yāng"}, {MRt.ENG.I.c, "yīng"},
			{MRt.AM.I.c, "yām"}, {MRt.EM.I.c, "yīm"},
			{MRt.ZERO.U.c, "wū"}, {MRt.A.U.c, "wā"}, {MRt.O.U.c, "wō"},
			{MRt.AI.U.c, "wāi"}, {MRt.EI.U.c, "wēi"}, {MRt.AN.U.c, "wān"}, {MRt.EN.U.c, "wēn"}, 
			{MRt.ANG.U.c, "wāng"}, {MRt.ONG.c, "wēng"},
			{MRt.ZERO.IU.c, "yū"}, {MRt.O.IU.c, "yuō"}, {MRt.EH.IU.c, "yuē"}, {MRt.AN.IU.c, "yuān"}, 
			{MRt.EN.IU.c, "yūn"}, {MRt.ENG.IU.c, "yōng"},

			{MRt.ZERO.I.m, "yí"}, {MRt.A.I.m, "yá"}, {MRt.EH.I.m, "yé"}, {MRt.O.I.m, "yó"},
			{MRt.AI.I.m, "yái"}, {MRt.AU.I.m, "yáo"}, {MRt.OU.I.m, "yóu"},
			{MRt.AN.I.m, "yán"}, {MRt.EN.I.m, "yín"}, {MRt.ANG.I.m, "yáng"}, {MRt.ENG.I.m, "yíng"},
			{MRt.AM.I.m, "yám"}, {MRt.EM.I.m, "yím"},
			{MRt.ZERO.U.m, "wú"}, {MRt.A.U.m, "wá"}, {MRt.O.U.m, "wó"},
			{MRt.AI.U.m, "wái"}, {MRt.EI.U.m, "wéi"}, {MRt.AN.U.m, "wán"}, {MRt.EN.U.m, "wén"}, 
			{MRt.ANG.U.m, "wáng"}, {MRt.ONG.m, "wéng"},
			{MRt.ZERO.IU.m, "yú"}, {MRt.O.IU.m, "yuó"}, {MRt.EH.IU.m, "yué"}, {MRt.AN.IU.m, "yuán"},
			{MRt.EN.IU.m, "yún"}, {MRt.ENG.IU.m, "yóng"},

			{MRt.ZERO.I.r, "yǐ"}, {MRt.A.I.r, "yǎ"}, {MRt.EH.I.r, "yě"}, {MRt.O.I.r, "yǒ"},
			{MRt.AI.I.r, "yǎi"}, {MRt.AU.I.r, "yǎo"}, {MRt.OU.I.r, "yǒu"},
			{MRt.AN.I.r, "yǎn"}, {MRt.EN.I.r, "yǐn"}, {MRt.ANG.I.r, "yǎng"}, {MRt.ENG.I.r, "yǐng"},
			{MRt.AM.I.r, "yǎm"}, {MRt.EM.I.r, "yǐm"},
			{MRt.ZERO.U.r, "wǔ"}, {MRt.A.U.r, "wǎ"}, {MRt.O.U.r, "wǒ"},
			{MRt.AI.U.r, "wǎi"}, {MRt.EI.U.r, "wěi"}, {MRt.AN.U.r, "wǎn"}, {MRt.EN.U.r, "wěn"},
			{MRt.ANG.U.r, "wǎng"}, {MRt.ONG.r, "wěng"},
			{MRt.ZERO.IU.r, "yǔ"}, {MRt.O.IU.r, "yuǒ"}, {MRt.EH.IU.r, "yuě"}, {MRt.AN.IU.r, "yuǎn"},
			{MRt.EN.IU.r, "yǔn"}, {MRt.ENG.IU.r, "yǒng"},

			{MRt.ZERO.I.g, "yì"}, {MRt.A.I.g, "yà"}, {MRt.EH.I.g, "yè"}, {MRt.O.I.g, "yò"},
			{MRt.AI.I.g, "yài"}, {MRt.AU.I.g, "yào"}, {MRt.OU.I.g, "yòu"},
			{MRt.AN.I.g, "yàn"}, {MRt.EN.I.g, "yìn"}, {MRt.ANG.I.g, "yàng"}, {MRt.ENG.I.g, "yìng"},
			{MRt.AM.I.g, "yàm"}, {MRt.EM.I.g, "yìm"},
			{MRt.ZERO.U.g, "wù"}, {MRt.A.U.g, "wà"}, {MRt.O.U.g, "wò"},
			{MRt.AI.U.g, "wài"}, {MRt.EI.U.g, "wèi"}, {MRt.AN.U.g, "wàn"}, {MRt.EN.U.g, "wèn"},
			{MRt.ANG.U.g, "wàng"}, {MRt.ONG.g, "wèng"},
			{MRt.ZERO.IU.g, "yù"}, {MRt.O.IU.g, "yuò"}, {MRt.EH.IU.g, "yuè"}, {MRt.AN.IU.g, "yuàn"}, 
			{MRt.EN.IU.g, "yùn"}, {MRt.ENG.IU.g, "yòng"},

			{MRt.ZERO.I.e, "yih"}, {MRt.A.I.e, "yah"}, {MRt.EH.I.e, "yeh"}, {MRt.O.I.e, "yoh"},
			{MRt.AI.I.e, "yaih"}, {MRt.AU.I.e, "yaoh"}, {MRt.OU.I.e, "youh"},
			{MRt.ZERO.U.e, "wuh"}, {MRt.A.U.e, "wah"}, {MRt.O.U.e, "woh"},
			{MRt.AI.U.e, "waih"}, {MRt.EI.U.e, "weih"},
			{MRt.ZERO.IU.e, "yuh"}, {MRt.O.IU.e, "yuoh"}, {MRt.EH.IU.e, "yueh"},
		};

		protected static readonly Dictionary<MRt, string> _mfTrans = new Dictionary<MRt,string>{
			{MRt.ZERO.n, ""},
			{MRt.A.n, "a"}, {MRt.O.n, "o"}, {MRt.E.n, "e"}, {MRt.EH.n, "ê"},
			{MRt.Y.n, "i"}, {MRt.EL.n, "er"},
			{MRt.AI.n, "ai"}, {MRt.EI.n, "ei"}, {MRt.AU.n, "ao"}, {MRt.OU.n, "ou"},
			{MRt.AN.n, "an"}, {MRt.EN.n, "en"}, {MRt.ANG.n, "ang"}, {MRt.ENG.n, "eng"},
			{MRt.AM.n, "am"}, {MRt.EM.n, "em"},
			{MRt.ZERO.I.n, "i"}, {MRt.A.I.n, "ia"}, {MRt.EH.I.n, "ie"}, {MRt.O.I.n, "io"},
			{MRt.AI.I.n, "iai"}, {MRt.AU.I.n, "iao"}, {MRt.OU.I.n, "iu"},
			{MRt.AN.I.n, "ian"}, {MRt.EN.I.n, "in"}, {MRt.ANG.I.n, "iang"}, {MRt.ENG.I.n, "ing"},
			{MRt.AM.I.n, "iam"}, {MRt.EM.I.n, "im"},
			{MRt.ZERO.U.n, "u"}, {MRt.A.U.n, "ua"}, {MRt.O.U.n, "uo"},
			{MRt.AI.U.n, "uai"}, {MRt.EI.U.n, "ui"}, {MRt.AN.U.n, "uan"}, {MRt.EN.U.n, "un"},
			{MRt.EH.U.n, "ue"},
			{MRt.ANG.U.n, "uang"}, {MRt.ONG.n, "ong"},
			{MRt.ZERO.IU.n, "ü"}, {MRt.O.IU.n, "üo"}, {MRt.EH.IU.n, "üe"},
			{MRt.AN.IU.n, "üan"}, {MRt.EN.IU.n, "ün"}, {MRt.ENG.IU.n, "iong"},

			{MRt.ZERO.c, ""},
			{MRt.A.c, "ā"}, {MRt.O.c, "ō"}, {MRt.E.c, "ē"}, {MRt.EH.c, "êˉ"},
			{MRt.Y.c, "ī"}, {MRt.EL.c, "ēr"},
			{MRt.AI.c, "āi"}, {MRt.EI.c, "ēi"}, {MRt.AU.c, "āo"}, {MRt.OU.c, "ōu"},
			{MRt.AN.c, "ān"}, {MRt.EN.c, "ēn"}, {MRt.ANG.c, "āng"}, {MRt.ENG.c, "ēng"},
			{MRt.AM.c, "ām"}, {MRt.EM.c, "ēm"},
			{MRt.ZERO.I.c, "ī"}, {MRt.A.I.c, "iā"}, {MRt.EH.I.c, "iē"}, {MRt.O.I.c, "iō"},
			{MRt.AI.I.c, "iāi"}, {MRt.AU.I.c, "iāo"}, {MRt.OU.I.c, "iū"},
			{MRt.AN.I.c, "iān"}, {MRt.EN.I.c, "īn"}, {MRt.ANG.I.c, "iāng"}, {MRt.ENG.I.c, "īng"},
			{MRt.AM.I.c, "iām"}, {MRt.EM.I.c, "īm"},
			{MRt.ZERO.U.c, "ū"}, {MRt.A.U.c, "uā"}, {MRt.O.U.c, "uō"},
			{MRt.AI.U.c, "uāi"}, {MRt.EI.U.c, "uī"}, {MRt.AN.U.c, "uān"}, {MRt.EN.U.c, "ūn"},
			{MRt.EH.U.c, "uē"},
			{MRt.ANG.U.c, "uāng"}, {MRt.ONG.c, "ōng"},
			{MRt.ZERO.IU.c, "ǖ"}, {MRt.O.IU.c, "üō"}, {MRt.EH.IU.c, "üē"},
			{MRt.AN.IU.c, "üān"}, {MRt.EN.IU.c, "ǖn"}, {MRt.ENG.IU.c, "iōng"},

			{MRt.A.m, "á"}, {MRt.O.m, "ó"}, {MRt.E.m, "é"}, {MRt.EH.m, "ê´"},
			{MRt.Y.m, "í"}, {MRt.EL.m, "ér"},
			{MRt.AI.m, "ái"}, {MRt.EI.m, "éi"}, {MRt.AU.m, "áo"}, {MRt.OU.m, "óu"},
			{MRt.AN.m, "án"}, {MRt.EN.m, "én"}, {MRt.ANG.m, "áng"}, {MRt.ENG.m, "éng"},
			{MRt.AM.m, "ám"}, {MRt.EM.m, "ém"},
			{MRt.ZERO.I.m, "í"}, {MRt.A.I.m, "iá"}, {MRt.EH.I.m, "ié"}, {MRt.O.I.m, "ió"},
			{MRt.AI.I.m, "iái"}, {MRt.AU.I.m, "iáo"}, {MRt.OU.I.m, "iú"},
			{MRt.AN.I.m, "ián"}, {MRt.EN.I.m, "ín"}, {MRt.ANG.I.m, "iáng"}, {MRt.ENG.I.m, "íng"},
			{MRt.AM.I.m, "iám"}, {MRt.EM.I.m, "ím"},
			{MRt.ZERO.U.m, "ú"}, {MRt.A.U.m, "uá"}, {MRt.O.U.m, "uó"},
			{MRt.AI.U.m, "uái"}, {MRt.EI.U.m, "uí"}, {MRt.AN.U.m, "uán"}, {MRt.EN.U.m, "ún"},
			{MRt.EH.U.m, "ué"},
			{MRt.ANG.U.m, "uáng"}, {MRt.ONG.m, "óng"},
			{MRt.ZERO.IU.m, "ǘ"}, {MRt.O.IU.m, "üó"}, {MRt.EH.IU.m, "üé"},
			{MRt.AN.IU.m, "üán"}, {MRt.EN.IU.m, "ǘn"}, {MRt.ENG.IU.m, "ióng"},

			{MRt.A.r, "ǎ"}, {MRt.O.r, "ǒ"}, {MRt.E.r, "ě"}, {MRt.EH.r, "êˇ"},
			{MRt.Y.r, "ǐ"}, {MRt.EL.r, "ěr"},
			{MRt.AI.r, "ǎi"}, {MRt.EI.r, "ěi"}, {MRt.AU.r, "ǎo"}, {MRt.OU.r, "ǒu"},
			{MRt.AN.r, "ǎn"}, {MRt.EN.r, "ěn"}, {MRt.ANG.r, "ǎng"}, {MRt.ENG.r, "ěng"},
			{MRt.AM.r, "ǎm"}, {MRt.EM.r, "ěm"},
			{MRt.ZERO.I.r, "ǐ"}, {MRt.A.I.r, "iǎ"}, {MRt.EH.I.r, "iě"}, {MRt.O.I.r, "iǒ"},
			{MRt.AI.I.r, "iǎi"}, {MRt.AU.I.r, "iǎo"}, {MRt.OU.I.r, "iǔ"},
			{MRt.AN.I.r, "iǎn"}, {MRt.EN.I.r, "ǐn"}, {MRt.ANG.I.r, "iǎng"}, {MRt.ENG.I.r, "ǐng"},
			{MRt.AM.I.r, "iǎm"}, {MRt.EM.I.r, "ǐm"},
			{MRt.ZERO.U.r, "ǔ"}, {MRt.A.U.r, "uǎ"}, {MRt.O.U.r, "uǒ"},
			{MRt.AI.U.r, "uǎi"}, {MRt.EI.U.r, "uǐ"}, {MRt.AN.U.r, "uǎn"}, {MRt.EN.U.r, "ǔn"},
			{MRt.EH.U.r, "uě"},
			{MRt.ANG.U.r, "uǎng"}, {MRt.ONG.r, "ǒng"},
			{MRt.ZERO.IU.r, "ǚ"}, {MRt.O.IU.r, "üǒ"}, {MRt.EH.IU.r, "üě"},
			{MRt.AN.IU.r, "üǎn"}, {MRt.EN.IU.r, "ǚn"}, {MRt.ENG.IU.r, "iǒng"},

			{MRt.A.g, "à"}, {MRt.O.g, "ò"}, {MRt.E.g, "è"}, {MRt.EH.g, "ê`"},
			{MRt.Y.g, "ì"}, {MRt.EL.g, "èr"},
			{MRt.AI.g, "ài"}, {MRt.EI.g, "èi"}, {MRt.AU.g, "ào"}, {MRt.OU.g, "òu"},
			{MRt.AN.g, "àn"}, {MRt.EN.g, "èn"}, {MRt.ANG.g, "àng"}, {MRt.ENG.g, "èng"},
			{MRt.AM.g, "àm"}, {MRt.EM.g, "èm"},
			{MRt.ZERO.I.g, "ì"}, {MRt.A.I.g, "ià"}, {MRt.EH.I.g, "iè"}, {MRt.O.I.g, "iò"},
			{MRt.AI.I.g, "iài"}, {MRt.AU.I.g, "iào"}, {MRt.OU.I.g, "iù"},
			{MRt.AN.I.g, "iàn"}, {MRt.EN.I.g, "ìn"}, {MRt.ANG.I.g, "iàng"}, {MRt.ENG.I.g, "ìng"},
			{MRt.AM.I.g, "iàm"}, {MRt.EM.I.g, "ìm"},
			{MRt.ZERO.U.g, "ù"}, {MRt.A.U.g, "uà"}, {MRt.O.U.g, "uò"},
			{MRt.AI.U.g, "uài"}, {MRt.EI.U.g, "uì"}, {MRt.AN.U.g, "uàn"}, {MRt.EN.U.g, "ùn"},
			{MRt.EH.U.g, "uè"},
			{MRt.ANG.U.g, "uàng"}, {MRt.ONG.g, "òng"},
			{MRt.ZERO.IU.g, "ǜ"}, {MRt.O.IU.g, "üò"}, {MRt.EH.IU.g, "üè"},
			{MRt.AN.IU.g, "üàn"}, {MRt.EN.IU.g, "ǜn"}, {MRt.ENG.IU.g, "iòng"},

			{MRt.A.e, "ah"}, {MRt.O.e, "oh"}, {MRt.E.e, "eh"}, {MRt.EH.e, "êh"},
			{MRt.Y.e, "ih"},
			{MRt.AI.e, "aih"}, {MRt.EI.e, "eih"}, {MRt.AU.e, "aoh"}, {MRt.OU.e, "ouh"},
			{MRt.ZERO.I.e, "ih"}, {MRt.A.I.e, "iah"}, {MRt.EH.I.e, "ieh"}, {MRt.O.I.e, "ioh"},
			{MRt.AU.I.e, "iaoh"}, {MRt.OU.I.e, "iuh"},
			{MRt.ZERO.U.e, "uh"}, {MRt.A.U.e, "uah"}, {MRt.O.U.e, "uoh"},
			{MRt.AI.U.e, "uaih"}, {MRt.EI.U.e, "uih"},
			{MRt.EH.U.e, "ueh"},
			{MRt.ZERO.IU.e, "üh"}, {MRt.O.IU.e, "üoh"}, {MRt.EH.IU.e, "üeh"}

		} ;

		#endregion

		static public readonly Pinyin Inst = new Pinyin();
		public Pinyin ()
		{
		}

		#region IMTransliterator implementation
		public virtual MSyl 
		MunchSyllable (string s, ref int idx)
		{

			MImt imt;
			MRt rt;

			INIT init = 0;
			MED med = 0;
			FIN fin = 0;
			TONE t1 = TONE.NEUTRAL, t2 = TONE.NEUTRAL;

			bool match = _miDict.TryMatchStart(s, ref idx, out imt );

			do {
				if(match) 
				{
					init = imt.Initial;
					med = imt.Medial;
					t1 = imt.Tone;
				}

				var iMark = idx;
				match = _mfDict.TryMatchStart(s, ref idx,out rt );

				if(match) {
					t2 = rt.Tone;

					if(t2 != TONE.NEUTRAL && t1 != TONE.NEUTRAL) 
					{
						idx = iMark;
						break;
					}

					if( rt.Medial == MED.U)  //IU + ONG
					{
						if(rt.Final == FIN.ZERO && med == MED.I) //IU
							fin = FIN.OU;
						else if(rt.Final == FIN.ENG) //ONG
						{
							med = med | MED.U;
							fin = rt.Final;
						}
						else 
							idx = iMark;
					}
					else 
					{
						fin = rt.Final;
						if(fin == FIN.Y && med == MED.U)//UI
						{
							fin = FIN.EI;
						}
						else if(fin == FIN.E && ((med & MED.I) == MED.I))       //ie,üe
						{
							fin = FIN.È;
						}

					}
				}

			} while(false);

			if(init == 0 && med == 0 && fin == 0)return MSyl.Default;

			return new MSyl(init, med, fin, (t1 == TONE.NEUTRAL ? t2 : t1));
		}

		public virtual IList<AltMSyl> 
		Parse (string s)
		{
			throw new System.NotImplementedException ();
		}

		public virtual string 
		Transcribe (MSyl syl)
		{

			string start, end = "";
			INIT i = syl.Initial;
			TONE t = syl.Tone;
			MED m = syl.Medial;
			RIME r = syl.Rime;


			//initial
			if(i == INIT.Void && m != MED._)//y w yu
			{ 
				return _m0Trans[new MRt(r,t)];
			}
			else if(syl.InitialGroup ==INITGROUP.GGRP && syl.IsPalatized) //j q x
			{
				start = _specInit[i];
				if(m == MED.IU && syl.Final != FIN.ENG) //ü -> u but not iong 
				{
					r =  (RIME)((byte)MED.U | (byte)syl.Final); //ju qu xu
				}
			}
			else if(syl.InitialGroup == INITGROUP.TZGRP && m == MED.I && syl.Final == FIN.ZERO) //zyi cyi syi
			{ 
				start = _specInit[i];
			} 
			else
				start = _miTrans[i];

			//final
			_mfTrans.TryGetValue( new MRt(r,t),out end);

			return start + end;
		}
		#endregion

	}
}

