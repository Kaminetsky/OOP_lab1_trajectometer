using UnityEngine;
using System.Collections;

// Implementation of the Google Color palette
// https://material.io/guidelines/style/color.html#color-color-palette

public class GColor
	{
	public static Color Alpha (Color col, float alpha)
		{
		col.a = alpha;
		return col;
		}

	// Primary colors (hue 500)

	public static Color red				= CommonTools.ParseColor("#e51c23");
	public static Color pink			= CommonTools.ParseColor("#e91e63");
	public static Color purple			= CommonTools.ParseColor("#9c27b0");
	public static Color deepPurple		= CommonTools.ParseColor("#673ab7");
	public static Color indigo			= CommonTools.ParseColor("#3f51b5");
	public static Color blue			= CommonTools.ParseColor("#5677fc");
	public static Color lightBlue		= CommonTools.ParseColor("#03a9f4");
	public static Color cyan 			= CommonTools.ParseColor("#00bcd4");
	public static Color teal 			= CommonTools.ParseColor("#009688");
	public static Color green 			= CommonTools.ParseColor("#259b24");
	public static Color lightGreen 		= CommonTools.ParseColor("#8bc34a");
	public static Color lime 			= CommonTools.ParseColor("#cddc39");
	public static Color yellow 			= CommonTools.ParseColor("#ffeb3b");
	public static Color amber 			= CommonTools.ParseColor("#ffc107");
	public static Color orange 			= CommonTools.ParseColor("#ff9800");
	public static Color deepOrange		= CommonTools.ParseColor("#ff5722");
	public static Color brown			= CommonTools.ParseColor("#795548");
	public static Color grey			= CommonTools.ParseColor("#9e9e9e");
	public static Color blueGrey		= CommonTools.ParseColor("#607d8b");

	// Strongest versions of the primary colors (hue 900)

	public static Color solidRed			= CommonTools.ParseColor("#b0120a");
	public static Color solidPink			= CommonTools.ParseColor("#880e4f");
	public static Color solidPurple			= CommonTools.ParseColor("#4a148c");
	public static Color solidDeepPurple		= CommonTools.ParseColor("#311b92");
	public static Color solidIndigo			= CommonTools.ParseColor("#1a237e");
	public static Color solidBlue			= CommonTools.ParseColor("#2a36b1");
	public static Color solidLightBlue		= CommonTools.ParseColor("#01579b");
	public static Color solidCyan 			= CommonTools.ParseColor("#006064");
	public static Color solidTeal 			= CommonTools.ParseColor("#004d40");
	public static Color solidGreen 			= CommonTools.ParseColor("#0d5302");
	public static Color solidLightGreen 	= CommonTools.ParseColor("#33691e");
	public static Color solidLime 			= CommonTools.ParseColor("#827717");
	public static Color solidYellow 		= CommonTools.ParseColor("#f57f17");
	public static Color solidAmber 			= CommonTools.ParseColor("#ff6f00");
	public static Color solidOrange 		= CommonTools.ParseColor("#e65100");
	public static Color solidDeepOrange		= CommonTools.ParseColor("#bf360c");
	public static Color solidBrown			= CommonTools.ParseColor("#3e2723");
	public static Color solidGrey			= CommonTools.ParseColor("#212121");
	public static Color solidBlueGrey		= CommonTools.ParseColor("#263238");

	// Accented versions of the primary colors (hue A400)

	public static Color accentRed			= CommonTools.ParseColor("#ff2d6f");
	public static Color accentPink			= CommonTools.ParseColor("#f50057");
	public static Color accentPurple		= CommonTools.ParseColor("#d500f9");
	public static Color accentDeepPurple	= CommonTools.ParseColor("#651fff");
	public static Color accentBlue			= CommonTools.ParseColor("#4d73ff");
	public static Color accentLightBlue		= CommonTools.ParseColor("#00b0ff");
	public static Color accentCyan			= CommonTools.ParseColor("#00e5ff");
	public static Color accentTeal			= CommonTools.ParseColor("#1de9b6");
	public static Color accentGreen			= CommonTools.ParseColor("#14e715");
	public static Color accentLightGreen	= CommonTools.ParseColor("#76ff03");
	public static Color accentLime			= CommonTools.ParseColor("#c6ff00");
	public static Color accentYellow		= CommonTools.ParseColor("#ffea00");
	public static Color accentAmber			= CommonTools.ParseColor("#ffc400");
	public static Color accentOrange		= CommonTools.ParseColor("#ff9100");
	public static Color accentDeepOrange	= CommonTools.ParseColor("#ff3d00");

	// Just for convenience

	public static Color black = Color.black;
	public static Color white = Color.white;
	public static Color gray = grey;

	// All color hues and accents.
	// Can be disabled by conditional define.

	#if !GCOLOR_DISABLE_HUES
	public static Color red50			= CommonTools.ParseColor("#fde0dc");
	public static Color red100			= CommonTools.ParseColor("#f9bdbb");
	public static Color red200			= CommonTools.ParseColor("#f69988");
	public static Color red300			= CommonTools.ParseColor("#f36c60");
	public static Color red400			= CommonTools.ParseColor("#e84e40");
	public static Color red500			= CommonTools.ParseColor("#e51c23");
	public static Color red600			= CommonTools.ParseColor("#dd191d");
	public static Color red700			= CommonTools.ParseColor("#d01716");
	public static Color red800			= CommonTools.ParseColor("#c41411");
	public static Color red900			= CommonTools.ParseColor("#b0120a");

	public static Color pink50			= CommonTools.ParseColor("#fce4ec");
	public static Color pink100			= CommonTools.ParseColor("#f8bbd0");
	public static Color pink200			= CommonTools.ParseColor("#f48fb1");
	public static Color pink300			= CommonTools.ParseColor("#f06292");
	public static Color pink400			= CommonTools.ParseColor("#ec407a");
	public static Color pink500			= CommonTools.ParseColor("#e91e63");
	public static Color pink600			= CommonTools.ParseColor("#d81b60");
	public static Color pink700			= CommonTools.ParseColor("#c2185b");
	public static Color pink800			= CommonTools.ParseColor("#ad1457");
	public static Color pink900			= CommonTools.ParseColor("#880e4f");

	public static Color purple50		= CommonTools.ParseColor("#f3e5f5");
	public static Color purple100		= CommonTools.ParseColor("#e1bee7");
	public static Color purple200		= CommonTools.ParseColor("#ce93d8");
	public static Color purple300		= CommonTools.ParseColor("#ba68c8");
	public static Color purple400		= CommonTools.ParseColor("#ab47bc");
	public static Color purple500		= CommonTools.ParseColor("#9c27b0");
	public static Color purple600		= CommonTools.ParseColor("#8e24aa");
	public static Color purple700		= CommonTools.ParseColor("#7b1fa2");
	public static Color purple800		= CommonTools.ParseColor("#6a1b9a");
	public static Color purple900		= CommonTools.ParseColor("#4a148c");

	public static Color deepPurple50	= CommonTools.ParseColor("#ede7f6");
	public static Color deepPurple100	= CommonTools.ParseColor("#d1c4e9");
	public static Color deepPurple200	= CommonTools.ParseColor("#b39ddb");
	public static Color deepPurple300	= CommonTools.ParseColor("#9575cd");
	public static Color deepPurple400	= CommonTools.ParseColor("#7e57c2");
	public static Color deepPurple500	= CommonTools.ParseColor("#673ab7");
	public static Color deepPurple600	= CommonTools.ParseColor("#5e35b1");
	public static Color deepPurple700	= CommonTools.ParseColor("#512da8");
	public static Color deepPurple800	= CommonTools.ParseColor("#4527a0");
	public static Color deepPurple900	= CommonTools.ParseColor("#311b92");

	public static Color indigo50		= CommonTools.ParseColor("#e8eaf6");
	public static Color indigo100		= CommonTools.ParseColor("#c5cae9");
	public static Color indigo200		= CommonTools.ParseColor("#9fa8da");
	public static Color indigo300		= CommonTools.ParseColor("#7986cb");
	public static Color indigo400		= CommonTools.ParseColor("#5c6bc0");
	public static Color indigo500		= CommonTools.ParseColor("#3f51b5");
	public static Color indigo600		= CommonTools.ParseColor("#3949ab");
	public static Color indigo700		= CommonTools.ParseColor("#303f9f");
	public static Color indigo800		= CommonTools.ParseColor("#283593");
	public static Color indigo900		= CommonTools.ParseColor("#1a237e");

	public static Color blue50			= CommonTools.ParseColor("#e7e9fd");
	public static Color blue100			= CommonTools.ParseColor("#d0d9ff");
	public static Color blue200			= CommonTools.ParseColor("#afbfff");
	public static Color blue300			= CommonTools.ParseColor("#91a7ff");
	public static Color blue400			= CommonTools.ParseColor("#738ffe");
	public static Color blue500			= CommonTools.ParseColor("#5677fc");
	public static Color blue600			= CommonTools.ParseColor("#4e6cef");
	public static Color blue700			= CommonTools.ParseColor("#455ede");
	public static Color blue800			= CommonTools.ParseColor("#3b50ce");
	public static Color blue900			= CommonTools.ParseColor("#2a36b1");

	public static Color lightBlue50		= CommonTools.ParseColor("#e1f5fe");
	public static Color lightBlue100	= CommonTools.ParseColor("#b3e5fc");
	public static Color lightBlue200	= CommonTools.ParseColor("#81d4fa");
	public static Color lightBlue300	= CommonTools.ParseColor("#4fc3f7");
	public static Color lightBlue400	= CommonTools.ParseColor("#29b6f6");
	public static Color lightBlue500	= CommonTools.ParseColor("#03a9f4");
	public static Color lightBlue600	= CommonTools.ParseColor("#039be5");
	public static Color lightBlue700	= CommonTools.ParseColor("#0288d1");
	public static Color lightBlue800	= CommonTools.ParseColor("#0277bd");
	public static Color lightBlue900	= CommonTools.ParseColor("#01579b");

	public static Color cyan50			= CommonTools.ParseColor("#e0f7fa");
	public static Color cyan100			= CommonTools.ParseColor("#b2ebf2");
	public static Color cyan200			= CommonTools.ParseColor("#80deea");
	public static Color cyan300			= CommonTools.ParseColor("#4dd0e1");
	public static Color cyan400			= CommonTools.ParseColor("#26c6da");
	public static Color cyan500			= CommonTools.ParseColor("#00bcd4");
	public static Color cyan600			= CommonTools.ParseColor("#00acc1");
	public static Color cyan700			= CommonTools.ParseColor("#0097a7");
	public static Color cyan800			= CommonTools.ParseColor("#00838f");
	public static Color cyan900			= CommonTools.ParseColor("#006064");

	public static Color teal50			= CommonTools.ParseColor("#e0f2f1");
	public static Color teal100			= CommonTools.ParseColor("#b2dfdb");
	public static Color teal200			= CommonTools.ParseColor("#80cbc4");
	public static Color teal300			= CommonTools.ParseColor("#4db6ac");
	public static Color teal400			= CommonTools.ParseColor("#26a69a");
	public static Color teal500			= CommonTools.ParseColor("#009688");
	public static Color teal600			= CommonTools.ParseColor("#00897b");
	public static Color teal700			= CommonTools.ParseColor("#00796b");
	public static Color teal800			= CommonTools.ParseColor("#00695c");
	public static Color teal900			= CommonTools.ParseColor("#004d40");

	public static Color green50			= CommonTools.ParseColor("#d0f8ce");
	public static Color green100		= CommonTools.ParseColor("#a3e9a4");
	public static Color green200		= CommonTools.ParseColor("#72d572");
	public static Color green300		= CommonTools.ParseColor("#42bd41");
	public static Color green400		= CommonTools.ParseColor("#2baf2b");
	public static Color green500		= CommonTools.ParseColor("#259b24");
	public static Color green600		= CommonTools.ParseColor("#0a8f08");
	public static Color green700		= CommonTools.ParseColor("#0a7e07");
	public static Color green800		= CommonTools.ParseColor("#056f00");
	public static Color green900		= CommonTools.ParseColor("#0d5302");

	public static Color lightGreen50	= CommonTools.ParseColor("#f1f8e9");
	public static Color lightGreen100	= CommonTools.ParseColor("#dcedc8");
	public static Color lightGreen200	= CommonTools.ParseColor("#c5e1a5");
	public static Color lightGreen300	= CommonTools.ParseColor("#aed581");
	public static Color lightGreen400	= CommonTools.ParseColor("#9ccc65");
	public static Color lightGreen500	= CommonTools.ParseColor("#8bc34a");
	public static Color lightGreen600	= CommonTools.ParseColor("#7cb342");
	public static Color lightGreen700	= CommonTools.ParseColor("#689f38");
	public static Color lightGreen800	= CommonTools.ParseColor("#558b2f");
	public static Color lightGreen900	= CommonTools.ParseColor("#33691e");

	public static Color lime50			= CommonTools.ParseColor("#f9fbe7");
	public static Color lime100			= CommonTools.ParseColor("#f0f4c3");
	public static Color lime200			= CommonTools.ParseColor("#e6ee9c");
	public static Color lime300			= CommonTools.ParseColor("#dce775");
	public static Color lime400			= CommonTools.ParseColor("#d4e157");
	public static Color lime500			= CommonTools.ParseColor("#cddc39");
	public static Color lime600			= CommonTools.ParseColor("#c0ca33");
	public static Color lime700			= CommonTools.ParseColor("#afb42b");
	public static Color lime800			= CommonTools.ParseColor("#9e9d24");
	public static Color lime900			= CommonTools.ParseColor("#827717");

	public static Color yellow50		= CommonTools.ParseColor("#fffde7");
	public static Color yellow100		= CommonTools.ParseColor("#fff9c4");
	public static Color yellow200		= CommonTools.ParseColor("#fff59d");
	public static Color yellow300		= CommonTools.ParseColor("#fff176");
	public static Color yellow400		= CommonTools.ParseColor("#ffee58");
	public static Color yellow500		= CommonTools.ParseColor("#ffeb3b");
	public static Color yellow600		= CommonTools.ParseColor("#fdd835");
	public static Color yellow700		= CommonTools.ParseColor("#fbc02d");
	public static Color yellow800		= CommonTools.ParseColor("#f9a825");
	public static Color yellow900		= CommonTools.ParseColor("#f57f17");

	public static Color amber50			= CommonTools.ParseColor("#fff8e1");
	public static Color amber100		= CommonTools.ParseColor("#ffecb3");
	public static Color amber200		= CommonTools.ParseColor("#ffe082");
	public static Color amber300		= CommonTools.ParseColor("#ffd54f");
	public static Color amber400		= CommonTools.ParseColor("#ffca28");
	public static Color amber500		= CommonTools.ParseColor("#ffc107");
	public static Color amber600		= CommonTools.ParseColor("#ffb300");
	public static Color amber700		= CommonTools.ParseColor("#ffa000");
	public static Color amber800		= CommonTools.ParseColor("#ff8f00");
	public static Color amber900		= CommonTools.ParseColor("#ff6f00");

	public static Color orange50		= CommonTools.ParseColor("#fff3e0");
	public static Color orange100		= CommonTools.ParseColor("#ffe0b2");
	public static Color orange200		= CommonTools.ParseColor("#ffcc80");
	public static Color orange300		= CommonTools.ParseColor("#ffb74d");
	public static Color orange400		= CommonTools.ParseColor("#ffa726");
	public static Color orange500		= CommonTools.ParseColor("#ff9800");
	public static Color orange600		= CommonTools.ParseColor("#fb8c00");
	public static Color orange700		= CommonTools.ParseColor("#f57c00");
	public static Color orange800		= CommonTools.ParseColor("#ef6c00");
	public static Color orange900		= CommonTools.ParseColor("#e65100");

	public static Color deepOrange50	= CommonTools.ParseColor("#fbe9e7");
	public static Color deepOrange100	= CommonTools.ParseColor("#ffccbc");
	public static Color deepOrange200	= CommonTools.ParseColor("#ffab91");
	public static Color deepOrange300	= CommonTools.ParseColor("#ff8a65");
	public static Color deepOrange400	= CommonTools.ParseColor("#ff7043");
	public static Color deepOrange500	= CommonTools.ParseColor("#ff5722");
	public static Color deepOrange600	= CommonTools.ParseColor("#f4511e");
	public static Color deepOrange700	= CommonTools.ParseColor("#e64a19");
	public static Color deepOrange800	= CommonTools.ParseColor("#d84315");
	public static Color deepOrange900	= CommonTools.ParseColor("#bf360c");

	public static Color brown50			= CommonTools.ParseColor("#efebe9");
	public static Color brown100		= CommonTools.ParseColor("#d7ccc8");
	public static Color brown200		= CommonTools.ParseColor("#bcaaa4");
	public static Color brown300		= CommonTools.ParseColor("#a1887f");
	public static Color brown400		= CommonTools.ParseColor("#8d6e63");
	public static Color brown500		= CommonTools.ParseColor("#795548");
	public static Color brown600		= CommonTools.ParseColor("#6d4c41");
	public static Color brown700		= CommonTools.ParseColor("#5d4037");
	public static Color brown800		= CommonTools.ParseColor("#4e342e");
	public static Color brown900		= CommonTools.ParseColor("#3e2723");

	public static Color grey50			= CommonTools.ParseColor("#fafafa");
	public static Color grey100			= CommonTools.ParseColor("#f5f5f5");
	public static Color grey200			= CommonTools.ParseColor("#eeeeee");
	public static Color grey300			= CommonTools.ParseColor("#e0e0e0");
	public static Color grey400			= CommonTools.ParseColor("#bdbdbd");
	public static Color grey500			= CommonTools.ParseColor("#9e9e9e");
	public static Color grey600			= CommonTools.ParseColor("#757575");
	public static Color grey700			= CommonTools.ParseColor("#616161");
	public static Color grey800			= CommonTools.ParseColor("#424242");
	public static Color grey900			= CommonTools.ParseColor("#212121");

	public static Color blueGrey50		= CommonTools.ParseColor("#eceff1");
	public static Color blueGrey100		= CommonTools.ParseColor("#cfd8dc");
	public static Color blueGrey200		= CommonTools.ParseColor("#b0bec5");
	public static Color blueGrey300		= CommonTools.ParseColor("#90a4ae");
	public static Color blueGrey400		= CommonTools.ParseColor("#78909c");
	public static Color blueGrey500		= CommonTools.ParseColor("#607d8b");
	public static Color blueGrey600		= CommonTools.ParseColor("#546e7a");
	public static Color blueGrey700		= CommonTools.ParseColor("#455a64");
	public static Color blueGrey800		= CommonTools.ParseColor("#37474f");
	public static Color blueGrey900		= CommonTools.ParseColor("#263238");
	#endif

	#if !GCOLOR_DISABLE_ACCENTS
	public static Color redA100			= CommonTools.ParseColor("#ff7997");
	public static Color redA200			= CommonTools.ParseColor("#ff5177");
	public static Color redA400			= CommonTools.ParseColor("#ff2d6f");
	public static Color redA700			= CommonTools.ParseColor("#e00032");

	public static Color pinkA100		= CommonTools.ParseColor("#ff80ab");
	public static Color pinkA200		= CommonTools.ParseColor("#ff4081");
	public static Color pinkA400		= CommonTools.ParseColor("#f50057");
	public static Color pinkA700		= CommonTools.ParseColor("#c51162");

	public static Color purpleA100		= CommonTools.ParseColor("#ea80fc");
	public static Color purpleA200		= CommonTools.ParseColor("#e040fb");
	public static Color purpleA400		= CommonTools.ParseColor("#d500f9");
	public static Color purpleA700		= CommonTools.ParseColor("#aa00ff");

	public static Color deepPurpleA100	= CommonTools.ParseColor("#b388ff");
	public static Color deepPurpleA200	= CommonTools.ParseColor("#7c4dff");
	public static Color deepPurpleA400	= CommonTools.ParseColor("#651fff");
	public static Color deepPurpleA700	= CommonTools.ParseColor("#6200ea");

	public static Color indigoA100		= CommonTools.ParseColor("#8c9eff");
	public static Color indigoA200		= CommonTools.ParseColor("#536dfe");
	public static Color indigoA400		= CommonTools.ParseColor("#3d5afe");
	public static Color indigoA700		= CommonTools.ParseColor("#304ffe");

	public static Color blueA100		= CommonTools.ParseColor("#a6baff");
	public static Color blueA200		= CommonTools.ParseColor("#6889ff");
	public static Color blueA400		= CommonTools.ParseColor("#4d73ff");
	public static Color blueA700		= CommonTools.ParseColor("#4d69ff");

	public static Color lightBlueA100	= CommonTools.ParseColor("#80d8ff");
	public static Color lightBlueA200	= CommonTools.ParseColor("#40c4ff");
	public static Color lightBlueA400	= CommonTools.ParseColor("#00b0ff");
	public static Color lightBlueA700	= CommonTools.ParseColor("#0091ea");

	public static Color cyanA100		= CommonTools.ParseColor("#84ffff");
	public static Color cyanA200		= CommonTools.ParseColor("#18ffff");
	public static Color cyanA400		= CommonTools.ParseColor("#00e5ff");
	public static Color cyanA700		= CommonTools.ParseColor("#00b8d4");

	public static Color tealA100		= CommonTools.ParseColor("#a7ffeb");
	public static Color tealA200		= CommonTools.ParseColor("#64ffda");
	public static Color tealA400		= CommonTools.ParseColor("#1de9b6");
	public static Color tealA700		= CommonTools.ParseColor("#00bfa5");

	public static Color greenA100		= CommonTools.ParseColor("#a2f78d");
	public static Color greenA200		= CommonTools.ParseColor("#5af158");
	public static Color greenA400		= CommonTools.ParseColor("#14e715");
	public static Color greenA700		= CommonTools.ParseColor("#12c700");

	public static Color lightGreenA100	= CommonTools.ParseColor("#ccff90");
	public static Color lightGreenA200	= CommonTools.ParseColor("#b2ff59");
	public static Color lightGreenA400	= CommonTools.ParseColor("#76ff03");
	public static Color lightGreenA700	= CommonTools.ParseColor("#64dd17");

	public static Color limeA100		= CommonTools.ParseColor("#f4ff81");
	public static Color limeA200		= CommonTools.ParseColor("#eeff41");
	public static Color limeA400		= CommonTools.ParseColor("#c6ff00");
	public static Color limeA700		= CommonTools.ParseColor("#aeea00");

	public static Color yellowA100		= CommonTools.ParseColor("#ffff8d");
	public static Color yellowA200		= CommonTools.ParseColor("#ffff00");
	public static Color yellowA400		= CommonTools.ParseColor("#ffea00");
	public static Color yellowA700		= CommonTools.ParseColor("#ffd600");

	public static Color amberA100		= CommonTools.ParseColor("#ffe57f");
	public static Color amberA200		= CommonTools.ParseColor("#ffd740");
	public static Color amberA400		= CommonTools.ParseColor("#ffc400");
	public static Color amberA700		= CommonTools.ParseColor("#ffab00");

	public static Color orangeA100		= CommonTools.ParseColor("#ffd180");
	public static Color orangeA200		= CommonTools.ParseColor("#ffab40");
	public static Color orangeA400		= CommonTools.ParseColor("#ff9100");
	public static Color orangeA700		= CommonTools.ParseColor("#ff6d00");

	public static Color deepOrangeA100	= CommonTools.ParseColor("#ff9e80");
	public static Color deepOrangeA200	= CommonTools.ParseColor("#ff6e40");
	public static Color deepOrangeA400	= CommonTools.ParseColor("#ff3d00");
	public static Color deepOrangeA700	= CommonTools.ParseColor("#dd2c00");
	#endif
	}
