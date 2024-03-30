// ActionHUD for 1.40/1.41
// Creates a generic transparent message hud with auto rotate based on a delay on a per line basis
// Bindable command in settings to toggle size of hud / number of lines. Suggest bind to Shift+U
// Preferences (to change defaults, edit in config/ClientPrefs.cs or in console):
//	$pref::ActionHUD::Delay = "5"; 		// Display each line for this number of seconds (Default: 5)
// Smokey 2023
// v1.2

// ActionHUD Font System
//
// ActionHUD will provide the following variables to tag fonts based on whether xFont is loaded:
//	Variable					DLL Loaded				DLL Not Loaded
//	$ActionHUD::WhiteFont		<f:if_w_10b.pft>		<f2>
//	$ActionHUD::RedFont			<f:sf_red_10b.pft>		<f0>
//	$ActionHUD::YellowFont		<f:sf_yellow_10b.pft>	<f0>
//	$ActionHUD::GreenFont		<f:if_g_10b.pft>		<f0>

// Preferences. Do not edit here - to change defaults, edit in config/ClientPrefs.cs or in console
$pref::ActionHUD::Delay = $pref::ActionHUD::Delay == "" ? "5" : $pref::ActionHUD::Delay;


function ActionHUD::GameBinds::Init() after GameBinds::Init
{
	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "actionMap.sae");
	$GameBinds::CurrentMap = "actionMap.sae";
	GameBinds::addBindCommand( "ActionHUD Size", "ActionHUD::Resize();");
}

if ($pref::ActionHUD::Lines == "")
	$pref::ActionHUD::Lines = 10;

$ActionHUD::MaxLines = 10;

function ActionHUD::SetFont() {
	if ($ActionHUD::FontLoaded)
		return;

	if ($xFont::Loaded) {
		$ActionHUD::WhiteFont = "<f:if_w_10b.pft>";
		$ActionHUD::RedFont = "<f:sf_red_10b.pft>";
		$ActionHUD::YellowFont = "<f:sf_yellow_10b.pft>";
		$ActionHUD::GreenFont = "<f:if_g_10b.pft>";
	} else {
		$ActionHUD::WhiteFont = "<f2>";
		$ActionHUD::RedFont = "<f0>";
		$ActionHUD::YellowFont = "<f0>";
		$ActionHUD::GreenFont = "<f0>";
	}

	$ActionHUD::FontLoaded = true;

    ActionHUD::Clear();
}

function ActionHUD::Init() {
	if ($ActionHUD::Loaded)
		return;

	$ActionHUD::Loaded = true;

	HUD::New("ActionHUD::Container", 0, 0, 500, 150, ActionHUD::Wake, ActionHUD::Sleep);
    newObject("ActionHUD::Text", FearGuiFormattedText, 5, 5, 500, 20);
	HUD::Add("ActionHUD::Container", "ActionHUD::Text");
    ActionHUD::Clear();
}

function ActionHUD::Wake() { ActionHUD::Update(); }
function ActionHUD::Sleep() { }

function ActionHUD::Clear() {
	for(%i=0; %i < $ActionHUD::MaxLines; %i++) {
		$ActionHUD::Row[%i] = "";
		$ActionHUD::Time[%i] = "";
	}

	ActionHUD::Update();
}

function ActionHUD::Timer() {
	for(%i=0; %i < $pref::ActionHUD::Lines; %i++) {
		if ( (getSimTime() - $ActionHUD::Time[%i]) > $pref::ActionHUD::Delay) {
			$ActionHUD::Row[%i] = "";
			$ActionHUD::Time[%i] = "";
		}
	}

	ActionHUD::Update();
}

function ActionHUD::Update() {

	// Add padding
	if ($pref::ActionHUD::Lines < $ActionHUD::MaxLines) {
		%padding = $ActionHUD::MaxLines - $pref::ActionHUD::Lines;

		for (%i = 0; %i < %padding; %i++) {
			%display = %display @ "\n";
		}
	}

	for(%i=$pref::ActionHUD::Lines-1; %i >= 0; %i--)
		%display = %display @ $ActionHUD::Row[%i] @ "\n";

	Control::SetValue("ActionHUD::Text", %display);
    Schedule::Add("ActionHUD::Timer();", 0.5);
}

function ActionHUD::Add(%message) {
	for(%i=$pref::ActionHUD::Lines-1; %i > 0; %i--) {
		$ActionHUD::Row[%i] = $ActionHUD::Row[%i-1];
		$ActionHUD::Time[%i] = $ActionHUD::Time[%i-1];
	}

	$ActionHUD::Row[0] = "<f2>" @ %message;
	$ActionHUD::Time[0] = getSimTime();

	ActionHUD::Update();
}

function ActionHUD::Resize() {

	switch ($pref::ActionHUD::Lines) {
        case 3:
            $pref::ActionHUD::Lines = 5;
            break;
        case 5:
            $pref::ActionHUD::Lines = 10;
            break;
        case 10:
            $pref::ActionHUD::Lines = 3;
            break;
    }

    ActionHUD::Clear();

	remoteEP("<f2>ActionHUD Size: <f1>" @ $pref::ActionHUD::Lines, 3, 2, 2, 10, 300);
}

Event::Attach(eventConnectionAccepted, ActionHUD::Clear);
Event::Attach(eventChangeMission, ActionHUD::Clear);
Event::Attach(eventConnected, ActionHUD::SetFont);

ActionHUD::Init();
