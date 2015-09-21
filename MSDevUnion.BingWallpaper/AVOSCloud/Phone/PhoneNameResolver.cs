using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AVOSCloud.Phone
{
	internal static class PhoneNameResolver
	{
		private static Dictionary<string, CanonicalPhoneName> huaweiLookupTable;

		private static Dictionary<string, CanonicalPhoneName> lgLookupTable;

		private static Dictionary<string, CanonicalPhoneName> samsungLookupTable;

		private static Dictionary<string, CanonicalPhoneName> htcLookupTable;

		private static Dictionary<string, CanonicalPhoneName> nokiaLookupTable;

		static PhoneNameResolver()
		{
			Dictionary<string, CanonicalPhoneName> dictionary = new Dictionary<string, CanonicalPhoneName>();
			dictionary.Add("HUAWEI H883G", new CanonicalPhoneName()
			{
				CanonicalModel = "Ascend W1"
			});
			dictionary.Add("HUAWEI W1", new CanonicalPhoneName()
			{
				CanonicalModel = "Ascend W1"
			});
			dictionary.Add("HUAWEI W2", new CanonicalPhoneName()
			{
				CanonicalModel = "Ascend W2"
			});
			PhoneNameResolver.huaweiLookupTable = dictionary;
			Dictionary<string, CanonicalPhoneName> dictionary1 = new Dictionary<string, CanonicalPhoneName>();
			dictionary1.Add("LG-C900", new CanonicalPhoneName()
			{
				CanonicalModel = "Optimus 7Q/Quantum"
			});
			dictionary1.Add("LG-E900", new CanonicalPhoneName()
			{
				CanonicalModel = "Optimus 7"
			});
			dictionary1.Add("LG-E906", new CanonicalPhoneName()
			{
				CanonicalModel = "Jil Sander"
			});
			PhoneNameResolver.lgLookupTable = dictionary1;
			Dictionary<string, CanonicalPhoneName> dictionary2 = new Dictionary<string, CanonicalPhoneName>();
			dictionary2.Add("GT-I8350", new CanonicalPhoneName()
			{
				CanonicalModel = "Omnia W"
			});
			dictionary2.Add("GT-I8350T", new CanonicalPhoneName()
			{
				CanonicalModel = "Omnia W"
			});
			dictionary2.Add("OMNIA W", new CanonicalPhoneName()
			{
				CanonicalModel = "Omnia W"
			});
			dictionary2.Add("GT-I8700", new CanonicalPhoneName()
			{
				CanonicalModel = "Omnia 7"
			});
			dictionary2.Add("OMNIA7", new CanonicalPhoneName()
			{
				CanonicalModel = "Omnia 7"
			});
			dictionary2.Add("GT-S7530", new CanonicalPhoneName()
			{
				CanonicalModel = "Omnia 7"
			});
			dictionary2.Add("I917", new CanonicalPhoneName()
			{
				CanonicalModel = "Focus"
			});
			dictionary2.Add("SGH-I917", new CanonicalPhoneName()
			{
				CanonicalModel = "Focus"
			});
			dictionary2.Add("SGH-I667", new CanonicalPhoneName()
			{
				CanonicalModel = "Focus 2"
			});
			dictionary2.Add("SGH-I677", new CanonicalPhoneName()
			{
				CanonicalModel = "Focus Flash"
			});
			dictionary2.Add("HADEN", new CanonicalPhoneName()
			{
				CanonicalModel = "Focus S"
			});
			dictionary2.Add("SGH-I937", new CanonicalPhoneName()
			{
				CanonicalModel = "Focus S"
			});
			dictionary2.Add("GT-I8750", new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV S"
			});
			dictionary2.Add("SGH-T899M", new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV S"
			});
			dictionary2.Add("SCH-I930", new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV Odyssey"
			});
			CanonicalPhoneName canonicalPhoneName = new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV Odyssey",
				Comments = "US Cellular"
			};
			dictionary2.Add("SCH-R860U", canonicalPhoneName);
			CanonicalPhoneName canonicalPhoneName1 = new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV S Neo",
				Comments = "Sprint"
			};
			dictionary2.Add("SPH-I800", canonicalPhoneName1);
			CanonicalPhoneName canonicalPhoneName2 = new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV S Neo",
				Comments = "AT&T"
			};
			dictionary2.Add("SGH-I187", canonicalPhoneName2);
			dictionary2.Add("GT-I8675", new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV S Neo"
			});
			CanonicalPhoneName canonicalPhoneName3 = new CanonicalPhoneName()
			{
				CanonicalModel = "ATIV SE",
				Comments = "Verizon"
			};
			dictionary2.Add("SM-W750V", canonicalPhoneName3);
			PhoneNameResolver.samsungLookupTable = dictionary2;
			Dictionary<string, CanonicalPhoneName> dictionary3 = new Dictionary<string, CanonicalPhoneName>();
			dictionary3.Add("7 MONDRIAN T8788", new CanonicalPhoneName()
			{
				CanonicalModel = "Surround"
			});
			dictionary3.Add("T8788", new CanonicalPhoneName()
			{
				CanonicalModel = "Surround"
			});
			dictionary3.Add("SURROUND", new CanonicalPhoneName()
			{
				CanonicalModel = "Surround"
			});
			dictionary3.Add("SURROUND T8788", new CanonicalPhoneName()
			{
				CanonicalModel = "Surround"
			});
			dictionary3.Add("7 MOZART", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("7 MOZART T8698", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("HTC MOZART", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("MERSAD 7 MOZART T8698", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("MOZART", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("MOZART T8698", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("PD67100", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("T8697", new CanonicalPhoneName()
			{
				CanonicalModel = "Mozart"
			});
			dictionary3.Add("7 PRO T7576", new CanonicalPhoneName()
			{
				CanonicalModel = "7 Pro"
			});
			dictionary3.Add("MWP6885", new CanonicalPhoneName()
			{
				CanonicalModel = "7 Pro"
			});
			dictionary3.Add("USCCHTC-PC93100", new CanonicalPhoneName()
			{
				CanonicalModel = "7 Pro"
			});
			CanonicalPhoneName canonicalPhoneName4 = new CanonicalPhoneName()
			{
				CanonicalModel = "Arrive",
				Comments = "Sprint"
			};
			dictionary3.Add("PC93100", canonicalPhoneName4);
			CanonicalPhoneName canonicalPhoneName5 = new CanonicalPhoneName()
			{
				CanonicalModel = "Arrive",
				Comments = "Sprint"
			};
			dictionary3.Add("T7575", canonicalPhoneName5);
			dictionary3.Add("HD2", new CanonicalPhoneName()
			{
				CanonicalModel = "HD2"
			});
			dictionary3.Add("HD2 LEO", new CanonicalPhoneName()
			{
				CanonicalModel = "HD2"
			});
			dictionary3.Add("LEO", new CanonicalPhoneName()
			{
				CanonicalModel = "HD2"
			});
			dictionary3.Add("7 SCHUBERT T9292", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			dictionary3.Add("GOLD", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			dictionary3.Add("HD7", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			dictionary3.Add("HD7 T9292", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			dictionary3.Add("MONDRIAN", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			dictionary3.Add("SCHUBERT", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			dictionary3.Add("Schubert T9292", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			CanonicalPhoneName canonicalPhoneName6 = new CanonicalPhoneName()
			{
				CanonicalModel = "HD7",
				Comments = "Telstra, AU"
			};
			dictionary3.Add("T9296", canonicalPhoneName6);
			dictionary3.Add("TOUCH-IT HD7", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7"
			});
			dictionary3.Add("T9295", new CanonicalPhoneName()
			{
				CanonicalModel = "HD7S"
			});
			dictionary3.Add("7 TROPHY", new CanonicalPhoneName()
			{
				CanonicalModel = "Trophy"
			});
			dictionary3.Add("7 TROPHY T8686", new CanonicalPhoneName()
			{
				CanonicalModel = "Trophy"
			});
			CanonicalPhoneName canonicalPhoneName7 = new CanonicalPhoneName()
			{
				CanonicalModel = "Trophy",
				Comments = "Verizon"
			};
			dictionary3.Add("PC40100", canonicalPhoneName7);
			dictionary3.Add("SPARK", new CanonicalPhoneName()
			{
				CanonicalModel = "Trophy"
			});
			dictionary3.Add("TOUCH-IT TROPHY", new CanonicalPhoneName()
			{
				CanonicalModel = "Trophy"
			});
			dictionary3.Add("MWP6985", new CanonicalPhoneName()
			{
				CanonicalModel = "Trophy"
			});
			dictionary3.Add("A620", new CanonicalPhoneName()
			{
				CanonicalModel = "8S"
			});
			dictionary3.Add("WINDOWS PHONE 8S BY HTC", new CanonicalPhoneName()
			{
				CanonicalModel = "8S"
			});
			dictionary3.Add("C620", new CanonicalPhoneName()
			{
				CanonicalModel = "8X"
			});
			dictionary3.Add("C625", new CanonicalPhoneName()
			{
				CanonicalModel = "8X"
			});
			CanonicalPhoneName canonicalPhoneName8 = new CanonicalPhoneName()
			{
				CanonicalModel = "8X",
				Comments = "Verizon"
			};
			dictionary3.Add("HTC6990LVW", canonicalPhoneName8);
			CanonicalPhoneName canonicalPhoneName9 = new CanonicalPhoneName()
			{
				CanonicalModel = "8X",
				Comments = "AT&T"
			};
			dictionary3.Add("PM23300", canonicalPhoneName9);
			dictionary3.Add("WINDOWS PHONE 8X BY HTC", new CanonicalPhoneName()
			{
				CanonicalModel = "8X"
			});
			CanonicalPhoneName canonicalPhoneName10 = new CanonicalPhoneName()
			{
				CanonicalModel = "8XT",
				Comments = "Sprint"
			};
			dictionary3.Add("HTCPO881 SPRINT", canonicalPhoneName10);
			CanonicalPhoneName canonicalPhoneName11 = new CanonicalPhoneName()
			{
				CanonicalModel = "Titan",
				Comments = "China"
			};
			dictionary3.Add("ETERNITY", canonicalPhoneName11);
			CanonicalPhoneName canonicalPhoneName12 = new CanonicalPhoneName()
			{
				CanonicalModel = "Titan",
				Comments = "AT&T"
			};
			dictionary3.Add("PI39100", canonicalPhoneName12);
			dictionary3.Add("TITAN X310E", new CanonicalPhoneName()
			{
				CanonicalModel = "Titan"
			});
			dictionary3.Add("ULTIMATE", new CanonicalPhoneName()
			{
				CanonicalModel = "Titan"
			});
			dictionary3.Add("X310E", new CanonicalPhoneName()
			{
				CanonicalModel = "Titan"
			});
			dictionary3.Add("X310E TITAN", new CanonicalPhoneName()
			{
				CanonicalModel = "Titan"
			});
			CanonicalPhoneName canonicalPhoneName13 = new CanonicalPhoneName()
			{
				CanonicalModel = "Titan II",
				Comments = "AT&T"
			};
			dictionary3.Add("PI86100", canonicalPhoneName13);
			dictionary3.Add("RADIANT", new CanonicalPhoneName()
			{
				CanonicalModel = "Titan II"
			});
			dictionary3.Add("RADAR", new CanonicalPhoneName()
			{
				CanonicalModel = "Radar"
			});
			CanonicalPhoneName canonicalPhoneName14 = new CanonicalPhoneName()
			{
				CanonicalModel = "Radar",
				Comments = "T-Mobile USA"
			};
			dictionary3.Add("RADAR 4G", canonicalPhoneName14);
			dictionary3.Add("RADAR C110E", new CanonicalPhoneName()
			{
				CanonicalModel = "Radar"
			});
			PhoneNameResolver.htcLookupTable = dictionary3;
			Dictionary<string, CanonicalPhoneName> dictionary4 = new Dictionary<string, CanonicalPhoneName>();
			dictionary4.Add("LUMIA 505", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 505"
			});
			dictionary4.Add("LUMIA 510", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 510"
			});
			dictionary4.Add("NOKIA 510", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 510"
			});
			dictionary4.Add("LUMIA 610", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 610"
			});
			CanonicalPhoneName canonicalPhoneName15 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 610",
				Comments = "NFC"
			};
			dictionary4.Add("LUMIA 610 NFC", canonicalPhoneName15);
			dictionary4.Add("NOKIA 610", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 610"
			});
			dictionary4.Add("NOKIA 610C", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 610"
			});
			dictionary4.Add("LUMIA 620", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 620"
			});
			dictionary4.Add("RM-846", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 620"
			});
			dictionary4.Add("LUMIA 710", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 710"
			});
			dictionary4.Add("NOKIA 710", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 710"
			});
			dictionary4.Add("LUMIA 800", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 800"
			});
			dictionary4.Add("LUMIA 800C", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 800"
			});
			dictionary4.Add("NOKIA 800", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 800"
			});
			CanonicalPhoneName canonicalPhoneName16 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 800",
				Comments = "China"
			};
			dictionary4.Add("NOKIA 800C", canonicalPhoneName16);
			dictionary4.Add("RM-878", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 810"
			});
			dictionary4.Add("RM-824", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 820"
			});
			dictionary4.Add("RM-825", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 820"
			});
			dictionary4.Add("RM-826", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 820"
			});
			CanonicalPhoneName canonicalPhoneName17 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 822",
				Comments = "Verizon"
			};
			dictionary4.Add("RM-845", canonicalPhoneName17);
			dictionary4.Add("LUMIA 900", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 900"
			});
			dictionary4.Add("NOKIA 900", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 900"
			});
			dictionary4.Add("RM-820", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 920"
			});
			dictionary4.Add("RM-821", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 920"
			});
			dictionary4.Add("RM-822", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 920"
			});
			CanonicalPhoneName canonicalPhoneName18 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 920",
				Comments = "920T"
			};
			dictionary4.Add("RM-867", canonicalPhoneName18);
			dictionary4.Add("NOKIA 920", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 920"
			});
			dictionary4.Add("LUMIA 920", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 920"
			});
			dictionary4.Add("RM-914", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 520"
			});
			dictionary4.Add("RM-915", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 520"
			});
			CanonicalPhoneName canonicalPhoneName19 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 520",
				Comments = "520T"
			};
			dictionary4.Add("RM-913", canonicalPhoneName19);
			CanonicalPhoneName canonicalPhoneName20 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 521",
				Comments = "T-Mobile 520"
			};
			dictionary4.Add("RM-917", canonicalPhoneName20);
			dictionary4.Add("RM-885", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 720"
			});
			CanonicalPhoneName canonicalPhoneName21 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 720",
				Comments = "China 720T"
			};
			dictionary4.Add("RM-887", canonicalPhoneName21);
			dictionary4.Add("RM-860", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 928"
			});
			dictionary4.Add("RM-892", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 925"
			});
			dictionary4.Add("RM-893", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 925"
			});
			dictionary4.Add("RM-910", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 925"
			});
			CanonicalPhoneName canonicalPhoneName22 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 925",
				Comments = "China 925T"
			};
			dictionary4.Add("RM-955", canonicalPhoneName22);
			dictionary4.Add("RM-875", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1020"
			});
			dictionary4.Add("RM-876", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1020"
			});
			dictionary4.Add("RM-877", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1020"
			});
			dictionary4.Add("RM-941", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 625"
			});
			dictionary4.Add("RM-942", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 625"
			});
			dictionary4.Add("RM-943", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 625"
			});
			dictionary4.Add("RM-937", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1520"
			});
			CanonicalPhoneName canonicalPhoneName23 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1520",
				Comments = "AT&T"
			};
			dictionary4.Add("RM-938", canonicalPhoneName23);
			dictionary4.Add("RM-939", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1520"
			});
			CanonicalPhoneName canonicalPhoneName24 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1520",
				Comments = "AT&T"
			};
			dictionary4.Add("RM-940", canonicalPhoneName24);
			dictionary4.Add("RM-998", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 525"
			});
			dictionary4.Add("RM-994", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1320"
			});
			dictionary4.Add("RM-995", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1320"
			});
			dictionary4.Add("RM-996", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 1320"
			});
			CanonicalPhoneName canonicalPhoneName25 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia Icon",
				Comments = "Verizon"
			};
			dictionary4.Add("RM-927", canonicalPhoneName25);
			dictionary4.Add("RM-976", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 630"
			});
			dictionary4.Add("RM-977", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 630"
			});
			dictionary4.Add("RM-978", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 630"
			});
			dictionary4.Add("RM-979", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 630"
			});
			dictionary4.Add("RM-974", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 635"
			});
			dictionary4.Add("RM-975", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 635"
			});
			CanonicalPhoneName canonicalPhoneName26 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 526",
				Comments = "China Mobile"
			};
			dictionary4.Add("RM-997", canonicalPhoneName26);
			dictionary4.Add("RM-1045", new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 930"
			});
			CanonicalPhoneName canonicalPhoneName27 = new CanonicalPhoneName()
			{
				CanonicalModel = "Lumia 636",
				Comments = "China"
			};
			dictionary4.Add("RM-1010", canonicalPhoneName27);
			PhoneNameResolver.nokiaLookupTable = dictionary4;
		}

		public static CanonicalPhoneName Resolve(string manufacturer, string model)
		{
			string upper = manufacturer.Trim().ToUpper();
			string str = upper;
			if (upper != null)
			{
				if (str == "NOKIA")
				{
					return PhoneNameResolver.ResolveNokia(manufacturer, model);
				}
				if (str == "HTC")
				{
					return PhoneNameResolver.ResolveHtc(manufacturer, model);
				}
				if (str == "SAMSUNG")
				{
					return PhoneNameResolver.ResolveSamsung(manufacturer, model);
				}
				if (str == "LG")
				{
					return PhoneNameResolver.ResolveLg(manufacturer, model);
				}
				if (str == "HUAWEI")
				{
					return PhoneNameResolver.ResolveHuawei(manufacturer, model);
				}
			}
			CanonicalPhoneName canonicalPhoneName = new CanonicalPhoneName()
			{
				ReportedManufacturer = manufacturer,
				ReportedModel = model,
				CanonicalManufacturer = manufacturer,
				CanonicalModel = model,
				IsResolved = false
			};
			return canonicalPhoneName;
		}

		private static CanonicalPhoneName ResolveHtc(string manufacturer, string model)
		{
			string upper = model.Trim().ToUpper();
			CanonicalPhoneName canonicalPhoneName = new CanonicalPhoneName()
			{
				ReportedManufacturer = manufacturer,
				ReportedModel = model,
				CanonicalManufacturer = "HTC",
				CanonicalModel = model,
				IsResolved = false
			};
			CanonicalPhoneName canonicalModel = canonicalPhoneName;
			string str = upper;
			if (str.StartsWith("A620"))
			{
				str = "A620";
			}
			if (str.StartsWith("C625"))
			{
				str = "C625";
			}
			if (str.StartsWith("C620"))
			{
				str = "C620";
			}
			if (PhoneNameResolver.htcLookupTable.ContainsKey(str))
			{
				CanonicalPhoneName item = PhoneNameResolver.htcLookupTable[str];
				canonicalModel.CanonicalModel = item.CanonicalModel;
				canonicalModel.Comments = item.Comments;
				canonicalModel.IsResolved = true;
			}
			return canonicalModel;
		}

		private static CanonicalPhoneName ResolveHuawei(string manufacturer, string model)
		{
			string upper = model.Trim().ToUpper();
			CanonicalPhoneName canonicalPhoneName = new CanonicalPhoneName()
			{
				ReportedManufacturer = manufacturer,
				ReportedModel = model,
				CanonicalManufacturer = "HUAWEI",
				CanonicalModel = model,
				IsResolved = false
			};
			CanonicalPhoneName canonicalModel = canonicalPhoneName;
			string str = upper;
			if (str.StartsWith("HUAWEI H883G"))
			{
				str = "HUAWEI H883G";
			}
			if (str.StartsWith("HUAWEI W1"))
			{
				str = "HUAWEI W1";
			}
			if (upper.StartsWith("HUAWEI W2"))
			{
				str = "HUAWEI W2";
			}
			if (PhoneNameResolver.huaweiLookupTable.ContainsKey(str))
			{
				CanonicalPhoneName item = PhoneNameResolver.huaweiLookupTable[str];
				canonicalModel.CanonicalModel = item.CanonicalModel;
				canonicalModel.Comments = item.Comments;
				canonicalModel.IsResolved = true;
			}
			return canonicalModel;
		}

		private static CanonicalPhoneName ResolveLg(string manufacturer, string model)
		{
			string upper = model.Trim().ToUpper();
			CanonicalPhoneName canonicalPhoneName = new CanonicalPhoneName()
			{
				ReportedManufacturer = manufacturer,
				ReportedModel = model,
				CanonicalManufacturer = "LG",
				CanonicalModel = model,
				IsResolved = false
			};
			CanonicalPhoneName canonicalModel = canonicalPhoneName;
			string str = upper;
			if (str.StartsWith("LG-C900"))
			{
				str = "LG-C900";
			}
			if (str.StartsWith("LG-E900"))
			{
				str = "LG-E900";
			}
			if (PhoneNameResolver.lgLookupTable.ContainsKey(str))
			{
				CanonicalPhoneName item = PhoneNameResolver.lgLookupTable[str];
				canonicalModel.CanonicalModel = item.CanonicalModel;
				canonicalModel.Comments = item.Comments;
				canonicalModel.IsResolved = true;
			}
			return canonicalModel;
		}

		private static CanonicalPhoneName ResolveNokia(string manufacturer, string model)
		{
			string upper = model.Trim().ToUpper();
			CanonicalPhoneName canonicalPhoneName = new CanonicalPhoneName()
			{
				ReportedManufacturer = manufacturer,
				ReportedModel = model,
				CanonicalManufacturer = "NOKIA",
				CanonicalModel = model,
				IsResolved = false
			};
			CanonicalPhoneName canonicalModel = canonicalPhoneName;
			string value = upper;
			if (upper.StartsWith("RM-"))
			{
				value = Regex.Match(upper, "(RM-)([0-9]+)").Value;
			}
			if (PhoneNameResolver.nokiaLookupTable.ContainsKey(value))
			{
				CanonicalPhoneName item = PhoneNameResolver.nokiaLookupTable[value];
				canonicalModel.CanonicalModel = item.CanonicalModel;
				canonicalModel.Comments = item.Comments;
				canonicalModel.IsResolved = true;
			}
			return canonicalModel;
		}

		private static CanonicalPhoneName ResolveSamsung(string manufacturer, string model)
		{
			string upper = model.Trim().ToUpper();
			CanonicalPhoneName canonicalPhoneName = new CanonicalPhoneName()
			{
				ReportedManufacturer = manufacturer,
				ReportedModel = model,
				CanonicalManufacturer = "SAMSUNG",
				CanonicalModel = model,
				IsResolved = false
			};
			CanonicalPhoneName canonicalModel = canonicalPhoneName;
			string str = upper;
			if (str.StartsWith("GT-S7530"))
			{
				str = "GT-S7530";
			}
			if (str.StartsWith("SGH-I917"))
			{
				str = "SGH-I917";
			}
			if (PhoneNameResolver.samsungLookupTable.ContainsKey(str))
			{
				CanonicalPhoneName item = PhoneNameResolver.samsungLookupTable[str];
				canonicalModel.CanonicalModel = item.CanonicalModel;
				canonicalModel.Comments = item.Comments;
				canonicalModel.IsResolved = true;
			}
			return canonicalModel;
		}
	}
}