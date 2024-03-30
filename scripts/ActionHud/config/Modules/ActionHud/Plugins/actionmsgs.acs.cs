// ActionMsgs for 1.40/1.41
// Uses ActionHUD to display messages
// Uses Match.cs from PrestoPack
// Captures the following actions and adds to ActionHUD
// 1) Midairs
// 2) Nade jumps
// Preferences (to change defaults, edit in config/ClientPrefs.cs or in console):
//	$pref::ActionMsgs::ClientOnly = "false"; 		// Only display messages that include ourselves (Default: false)
// Smokey 2023

// Preferences. Do not edit here - to change defaults, edit in config/ClientPrefs.cs or in console
$pref::ActionMsgs::ClientOnly = $pref::ActionMsgs::ClientOnly == "" ? false : $pref::ActionMsgs::ClientOnly;

function ActionMsgs::onMidAir(%killer, %victim, %distance) {
	if ($pref::ActionMsgs::ClientOnly && %killer != getManagerId() && %victim != getManagerId())
		return;

    %kname = String::escape(Client::GetName(%killer));
	%vname = String::escape(Client::GetName(%victim));

	%afont = $ActionHUD::WhiteFont;
	if(Client::GetTeam(%killer) == Team::Friendly())
		%kfont = $ActionHUD::GreenFont;
	else
		%kfont = $ActionHUD::RedFont;
	
	if(Client::GetTeam(%victim) == Team::Friendly())
		%vfont = $ActionHUD::GreenFont;
	else
		%vfont = $ActionHUD::RedFont;
  
	if (isfunction(ActionHUD::Add)) {
		ActionHUD::Add(%kfont@%kname@" "@%afont@"[midair] "@%vfont@%vname@%afont@" ["@%distance@" meter]");
	}
}

function ActionMsgs::onNadeJump(%player, %speed) {
	if ($pref::ActionMsgs::ClientOnly && %player != getManagerId())
		return;

	%pname = String::escape(Client::GetName(%player));

	%afont = $ActionHUD::WhiteFont;
	if(Client::GetTeam(%player) == Team::Friendly())
		%pfont = $ActionHUD::GreenFont;
	else
		%pfont = $ActionHUD::RedFont;

	if (isfunction(ActionHUD::Add)) {
		ActionHUD::Add(%pfont@%pname@" "@%afont@"[nadejump]"@%afont@" ["@%speed@" m/s]");
	}
}

function ActionMsgs::filter(%cl, %msg, %type) before onClientMessage {
	%ret = true;

	%message = %msg;
	%index = String::findSubStr(%msg, "~" );
	if (%index != -1) {
		%tags = String::getSubStr(%msg, %index + 1, 10000);
		%message = String::getSubStr(%msg, 0, %index);
	}

	if (Match::ParamString(String::trim(%msg), "%k lands [ %d meter ] mid-air on %v!") || Match::ParamString(String::trim(%msg), "%k mid-aired %v from %d meters away with a disc!!")) {
		%kname = Match::Result("k");
		%vname = Match::Result("v");
		%distance = Match::Result("d");

		for (%i = 2049; %i <= 2080; %i++) {
			if ($Team::Client::Name[%i] == %kname) {
				%killer = %i;
				break;
			}
		}

		for (%i = 2049; %i <= 2080; %i++) {
			if ($Team::Client::Name[%i] == %vname) {
				%victim = %i;
				break;
			}
		}

		ActionMsgs::onMidAir(%killer, %victim, %distance);

		%ret = false;
	}

	if (Match::ParamString(String::trim(%msg), "%p hit a nade jump going [ %s meters / second ] !")) {
		%pname = Match::Result("p");
		%speed = Match::Result("s");

		for (%i = 2049; %i <= 2080; %i++) {
			if ($Team::Client::Name[%i] == %pname) {
				%player = %i;
				break;
			}
		}

		ActionMsgs::onNadeJump(%player, %speed);

		%ret = false;
	}

	if (!%ret) {
		halt "0";
	} else {
		return;
	}
}
