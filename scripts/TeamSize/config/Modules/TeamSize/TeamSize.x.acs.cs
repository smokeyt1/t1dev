// TeamSize
// By Smokey
// Requires xFont.dll
// v0.1

$pref::TeamSize::WarningThreshold = $pref::TeamSize::WarningThreshold == "" ? "2" : $pref::TeamSize::WarningThreshold;

$TeamSize::OutlineAlpha = "80";
$TeamSize::TeamAlpha = "80";

function TeamSize::Init() {
	if ($xLoader::xFont != True && $LoaderPlugin::xFont != True) {
		echoc(1, "TeamSize: xFont.dll is required.");
		return;
	}

	if ($TeamSize::Loaded)
		return;

	$TeamSize::Loaded = true;

	HUD::New("TeamSize::Container", 0, 0, 50, 20, TeamSize::Wake, TeamSize::Sleep);

	newObject("TeamSize::Outline", FearGuiFormattedText, 0, 0, 50, 20);
	newObject("TeamSize::Friend_BG", FearGuiFormattedText, 0, 0, 50, 20);
	newObject("TeamSize::Foe_BG", FearGuiFormattedText, 0, 0, 50, 20);
	newObject("TeamSize::Friend_Num", FearGuiFormattedText, 9, 0, 22, 16);
	newObject("TeamSize::Foe_Num", FearGuiFormattedText, 35, 0, 22, 16);

	HUD::Add("TeamSize::Container","TeamSize::Outline");
	HUD::Add("TeamSize::Container","TeamSize::Friend_BG");
	HUD::Add("TeamSize::Container","TeamSize::Foe_BG");
	HUD::Add("TeamSize::Container", "TeamSize::Friend_Num");
	HUD::Add("TeamSize::Container", "TeamSize::Foe_Num");

	Control::SetValue("TeamSize::Outline", "<B0,0:Modules/TeamSize/Outline.png:" ~ $TeamSize::OutlineAlpha ~ ">");
	$TeamSize::Timer = xFont::NewTimer("TeamSize::Timer", 128, 255, 25, 0.05, 0, true);

	Event::Attach(eventClientsUpdated, TeamSize::Update);
}

function TeamSize::Update() {
	%friends = Team::Size(Team::Friendly());
	%foes = Team::Size(Team::Enemy());

	if (%friends == %foes) {
		%friend_color = "00FF00" ~ $TeamSize::TeamAlpha;
		%foe_color = "00FF00" ~ $TeamSize::TeamAlpha;
	} else if (%friends > %foes) {
		%variance = %friends - %foes;

		if (%variance < abs($pref::TeamSize::WarningThreshold)) {
			%foe_color = "FFFF00" ~ $TeamSize::TeamAlpha;
		} else {
			%foe_color = "FF0000" ~ $TeamSize::Timer;
		}

		%friend_color = "00FF00" ~ $TeamSize::TeamAlpha;
	} else {
		%variance = %foes - %friends;

		if (%variance < abs($pref::TeamSize::WarningThreshold)) {
			%friend_color = "FFFF00" ~ $TeamSize::TeamAlpha;
		} else {
			%friend_color = "FF0000" ~ $TeamSize::Timer;
		}

		%foe_color = "00FF00" ~ $TeamSize::TeamAlpha;
	}

	Control::SetValue("TeamSize::Friend_BG", "<B0,0:Modules/TeamSize/Friend.png:" ~ %friend_color ~ ">");
	Control::SetValue("TeamSize::Foe_BG", "<B0,0:Modules/TeamSize/Foe.png:" ~ %foe_color ~ ">");

	Control::SetValue("TeamSize::Friend_Num", "<f2>" ~ %friends);
	Control::SetValue("TeamSize::Foe_Num", "<f2>" ~ %foes);
}

function TeamSize::Wake() {}
function TeamSize::Sleep() {}

TeamSize::Init();

// ================================================================================
// xPrefs Support
// ================================================================================

function TeamSize::xSetup() after xPrefs::Setup {
    xPrefs::Create("TeamSize", "TeamSize::xInit");
}

function TeamSize::xInit() {
	if ($xLoader::xFont == True || $LoaderPlugin::xFont == True) {
		xPrefs::addText("TeamSize::Header", "TeamSize HUD");
		xPrefs::addTextEdit("TeamSize::TextEdit", "Warning Threshold", "$pref::TeamSize::WarningThreshold", "True", "5", "True", "TeamSize::Update();");
	} else {
		xPrefs::addText("TeamSize::Header", "xFont.dll Not Loaded");
	}
}
