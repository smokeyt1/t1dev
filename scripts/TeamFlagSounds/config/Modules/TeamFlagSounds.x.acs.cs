// TeamFlagSounds for 1.40/1.41
// Plays flag sounds based on friendly or enemy team
// Optionally filters out flag messages from Chat HUD with Binds or xPrefs
// Added xPref support
// By Smokey
// v0.8

function TeamFlagSounds::GameBinds::Init() after GameBinds::Init {
	if ($xPrefs::Installed)
		return;

	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "actionMap.sae");
	$GameBinds::CurrentMap = "actionMap.sae";
	GameBinds::addBindCommand( "TeamFlagSounds Toggle", "TeamFlagSounds::ToggleEnabled();");
	GameBinds::addBindCommand( "TeamFlagSounds Chat Filter", "TeamFlagSounds::ToggleFilter();");
}

// Prefs
$pref::TeamFlagSounds::Enabled = $pref::TeamFlagSounds::Enabled == "" ? "True" : $pref::TeamFlagSounds::Enabled;
$pref::TeamFlagSounds::ChatFilter = $pref::TeamFlagSounds::ChatFilter == "" ? "False" : $pref::TeamFlagSounds::ChatFilter;

// Flag Captured
$pref::TeamFlagSounds::CapFriendly = $pref::TeamFlagSounds::CapFriendly == "" ? "flag_cap_friendly.wav" : $pref::TeamFlagSounds::CapFriendly;
$pref::TeamFlagSounds::CapEnemy = $pref::TeamFlagSounds::CapEnemy == "" ? "flag_cap_enemy.wav" : $pref::TeamFlagSounds::CapEnemy;

// Flag Taken
$pref::TeamFlagSounds::TakenFriendly = $pref::TeamFlagSounds::TakenFriendly == "" ? "flag_taken_friendly.wav" : $pref::TeamFlagSounds::TakenFriendly;
$pref::TeamFlagSounds::TakenEnemy = $pref::TeamFlagSounds::TakenEnemy == "" ? "flag_taken_enemy.wav" : $pref::TeamFlagSounds::TakenEnemy;

// Flag Returned
$pref::TeamFlagSounds::ReturnFriendly = $pref::TeamFlagSounds::ReturnFriendly == "" ? "flag_return_friendly.wav" : $pref::TeamFlagSounds::ReturnFriendly;
$pref::TeamFlagSounds::ReturnEnemy = $pref::TeamFlagSounds::ReturnEnemy == "" ? "flag_return_enemy.wav" : $pref::TeamFlagSounds::ReturnEnemy;

// Flag Dropped
$pref::TeamFlagSounds::DropFriendly = $pref::TeamFlagSounds::DropFriendly == "" ? "flag_drop_friendly.wav" : $pref::TeamFlagSounds::DropFriendly;
$pref::TeamFlagSounds::DropEnemy = $pref::TeamFlagSounds::DropEnemy == "" ? "flag_drop_enemy.wav" : $pref::TeamFlagSounds::DropEnemy;


// Filter chat messages
$TeamFlagSounds::Filterid = -1;
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "* captured the * flag!";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "* took the * flag!";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "* returned the * flag!";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "* dropped the * flag!";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "Your team's flag was captured.";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "Your team captured the flag.";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "Your team has the * flag.";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "Your team's flag has been taken.";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "You returned the * flag!";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "The * flag was returned to base.";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "Your flag was returned to base.";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "You dropped the * flag!";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "Your flag was dropped in the field.";
$TeamFlagSounds::Filter[$TeamFlagSounds::Filterid++] = "The * flag was dropped in the field.";


function TeamFlag::Captured(%team, %cl) {
	if (!$pref::TeamFlagSounds::Enabled)
		return;

	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		if (File::findFirst($pref::TeamFlagSounds::CapFriendly) != "") {
			localSound($pref::TeamFlagSounds::CapFriendly);
		} else {
			localSound("flagcapture.ogg");
		}
	} else {
		if (File::findFirst($pref::TeamFlagSounds::CapEnemy) != "") {
			localSound($pref::TeamFlagSounds::CapEnemy);
		} else {
			localSound("flagcapture.ogg");
		}
	}
}

function TeamFlag::Taken(%team, %cl) {
	if (!$pref::TeamFlagSounds::Enabled)
		return;

	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		if (File::findFirst($pref::TeamFlagSounds::TakenFriendly) != "") {
			localSound($pref::TeamFlagSounds::TakenFriendly);
		} else {
			localSound("flag1.ogg");
		}
	} else {
		if (File::findFirst($pref::TeamFlagSounds::TakenEnemy) != "") {
			localSound($pref::TeamFlagSounds::TakenEnemy);
		} else {
			localSound("flag1.ogg");
		}
	}
}

function TeamFlag::Returned(%team, %cl) {
	if (!$pref::TeamFlagSounds::Enabled)
		return;

	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		if (File::findFirst($pref::TeamFlagSounds::ReturnFriendly) != "") {
			localSound($pref::TeamFlagSounds::ReturnFriendly);
		} else {
			localSound("flagreturn.ogg");
		}
	} else {
		if (File::findFirst($pref::TeamFlagSounds::ReturnEnemy) != "") {
			localSound($pref::TeamFlagSounds::ReturnEnemy);
		} else {
			localSound("flagreturn.ogg");
		}
	}
}

function TeamFlag::Dropped(%team, %cl) {
	if (!$pref::TeamFlagSounds::Enabled)
		return;

	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		localSound($pref::TeamFlagSounds::DropFriendly);
	} else {
		localSound($pref::TeamFlagSounds::DropEnemy);
	}
}

Event::Attach(eventFlagCap, TeamFlag::Captured);
Event::Attach(eventFlagPickup, TeamFlag::Taken);
Event::Attach(eventFlagGrab, TeamFlag::Taken);
Event::Attach(eventFlagDrop, TeamFlag::Dropped);
Event::Attach(eventFlagReturn, TeamFlag::Returned);


function flagfilter(%cl, %msg, %type) before onClientMessage {
	if (!$pref::TeamFlagSounds::Enabled)
		return;

	if ($TeamFlag::muteVoices == true) {
		$TeamFlag::muteVoices = false;
		$pref::playVoices = $TeamFlag::prefPlayVoices;
	} else {
		$TeamFlag::prefPlayVoices = $pref::playVoices;
	}

	%ret = true;

	%index = String::findSubStr(%msg, "~" );
	if (%index != -1) {
		%tags = String::getSubStr(%msg, %index + 1, 10000);
		%msg = String::getSubStr(%msg, 0, %index);
	}

	// Filter messages
	for (%i = 0; %i <= $TeamFlagSounds::Filterid; %i++) {
		if (Match::String(String::trim(%msg), $TeamFlagSounds::Filter[%i])) {

			if ($pref::TeamFlagSounds::ChatFilter) {
				%ret = false;
				break;
			} else {
				$pref::playVoices = false;
				$TeamFlag::muteVoices = true;
				Schedule::Add("$pref::playVoices = $TeamFlag::prefPlayVoices;", 0.01);
			}
		}
	}

	if (!%ret) {
		halt "0";
	} else {
		return;
	}
}

function TeamFlagSounds::ToggleFilter() {
	$pref::TeamFlagSounds::ChatFilter = !$pref::TeamFlagSounds::ChatFilter;
	%status = $pref::TeamFlagSounds::ChatFilter ? "ON" : "OFF";
	remoteEP("<jc><f2>TeamFlagSounds Chat Filter: <f1>" @ %status, 3, 2, 2, 10, 300);
}

function TeamFlagSounds::ToggleEnabled() {
	$pref::TeamFlagSounds::Enabled = !$pref::TeamFlagSounds::Enabled;
	%status = $pref::TeamFlagSounds::Enabled ? "ENABLED" : "DISABLED";
	remoteEP("<jc><f2>TeamFlagSounds <f1>" @ %status, 3, 2, 2, 10, 300);
}

// ================================================================================
// xPrefs Support
// ================================================================================

function TeamFlagSounds::xSetup() after xPrefs::Setup {
    xPrefs::Create("TeamFlagSounds", "TeamFlagSounds::xInit");
}

function TeamFlagSounds::xInit() {
	xPrefs::addText("TeamFlagSounds::Header1", "TeamFlagSounds");
	xPrefs::addCheckbox("TeamFlagSounds::Checkbox1", "Enabled", "$pref::TeamFlagSounds::Enabled");
	xPrefs::addCheckbox("TeamFlagSounds::Checkbox2", "Filter Flag Chat Messages", "$pref::TeamFlagSounds::ChatFilter");

	xPrefs::addText("TeamFlagSounds::Header2", "Flag Sounds");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit1", "Capture (Friendly)", "$pref::TeamFlagSounds::CapFriendly", "False", "255");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit2", "Capture (Enemy)", "$pref::TeamFlagSounds::CapEnemy", "False", "255");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit3", "Taken (Friendly)", "$pref::TeamFlagSounds::TakenFriendly", "False", "255");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit4", "Taken (Enemy)", "$pref::TeamFlagSounds::TakenEnemy", "False", "255");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit5", "Returned (Friendly)", "$pref::TeamFlagSounds::ReturnFriendly", "False", "255");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit6", "Returned (Enemy)", "$pref::TeamFlagSounds::ReturnEnemy", "False", "255");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit7", "Drop (Friendly)", "$pref::TeamFlagSounds::DropFriendly", "False", "255");
	xPrefs::addTextEdit("TeamFlagSounds::TextEdit8", "Drop (Enemy)", "$pref::TeamFlagSounds::DropEnemy", "False", "255");

	xPrefs::addBindCommand("actionMap.sae", "Toggle", "TeamFlagSounds::ToggleEnabled();");
	xPrefs::addBindCommand("actionMap.sae", "Chat Filter", "TeamFlagSounds::ToggleFilter();");
}
