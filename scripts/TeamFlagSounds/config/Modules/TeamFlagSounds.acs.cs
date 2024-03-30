// TeamFlagSounds for 1.40/1.41
// Plays flag sounds based on friendly or enemy team
// Optionally filters out flag messages from Chat HUD. To enable:
//   1) Use bindable command in Options, or
//   2) Add $pref::TeamFlagSounds::ChatFilter = true; to config/ClientPrefs.cs
// By Smokey
// v0.6

function TeamFlagSounds::GameBinds::Init() after GameBinds::Init {
	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "actionMap.sae");
	$GameBinds::CurrentMap = "actionMap.sae";
	GameBinds::addBindCommand( "TeamFlagSounds Chat Filter", "TeamFlagSounds::Toggle();");
}

if ($pref::TeamFlagSounds::ChatFilter == "")
	$pref::TeamFlagSounds::ChatFilter = false;


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

// Flag Captured
$TeamFlagSounds::CapFriendly = "flag_cap_friendly.wav";
$TeamFlagSounds::CapEnemy = "flag_cap_enemy.wav";

// Flag Taken
$TeamFlagSounds::TakenFriendly = "flag_taken_friendly.wav";
$TeamFlagSounds::TakenEnemy = "flag_taken_enemy.wav";

// Flag Returned
$TeamFlagSounds::ReturnFriendly = "flag_return_friendly.wav";
$TeamFlagSounds::ReturnEnemy = "flag_return_enemy.wav";

// Flag Dropped
$TeamFlagSounds::DropFriendly = "flag_drop_friendly.wav";
$TeamFlagSounds::DropEnemy = "flag_drop_enemy.wav";


function TeamFlag::Captured(%team, %cl) {
	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		localSound($TeamFlagSounds::CapFriendly);
	} else {
		localSound($TeamFlagSounds::CapEnemy);
	}
}

function TeamFlag::Taken(%team, %cl) {
	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		localSound($TeamFlagSounds::TakenFriendly);
	} else {
		localSound($TeamFlagSounds::TakenEnemy);
	}
}

function TeamFlag::Returned(%team, %cl) {
	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		localSound($TeamFlagSounds::ReturnFriendly);
	} else {
		localSound($TeamFlagSounds::ReturnEnemy);
	}
}

function TeamFlag::Dropped(%team, %cl) {
	if (Client::GetTeam(getManagerID()) == Client::getTeam(%cl)) {
		localSound($TeamFlagSounds::DropFriendly);
	} else {
		localSound($TeamFlagSounds::DropEnemy);
	}
}

Event::Attach(eventFlagCap, TeamFlag::Captured);
Event::Attach(eventFlagPickup, TeamFlag::Taken);
Event::Attach(eventFlagGrab, TeamFlag::Taken);
Event::Attach(eventFlagDrop, TeamFlag::Dropped);
Event::Attach(eventFlagReturn, TeamFlag::Returned);


function flagfilter(%cl, %msg, %type) before onClientMessage {

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

function TeamFlagSounds::Toggle() {
	$pref::TeamFlagSounds::ChatFilter = !$pref::TeamFlagSounds::ChatFilter;
	%status = $pref::TeamFlagSounds::ChatFilter ? "ON" : "OFF";
	remoteEP("<f2>TeamFlagSounds Chat Filter: <f1>" @ %status, 3, 2, 2, 10, 300);
}
