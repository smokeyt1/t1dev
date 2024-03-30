// KillMsgs for 1.40/1.41
// Uses ActionHUD to display messages
// Based on killhud.acs
// Be sure to include ServerMessageFilter.acs.cs in config/Modules to filter out kill messages from the chat hud
// Preferences (to change defaults, edit in config/ClientPrefs.cs or in console):
//	$pref::KillMsgs::ClientOnly = "false"; 		// Only display messages that include ourselves (Default: false)
// Smokey 2023

// Preferences. Do not edit here - to change defaults, edit in config/ClientPrefs.cs or in console
$pref::KillMsgs::ClientOnly = $pref::KillMsgs::ClientOnly == "" ? false : $pref::KillMsgs::ClientOnly;

function KillMsgs::onClientTeamKilled(%killer, %victim, %damagetype) {
	if ($pref::KillMsgs::ClientOnly && %killer != getManagerId() && %victim != getManagerId())
		return;

	%kname = String::escape(Client::GetName(%killer));
	%vname = String::escape(Client::GetName(%victim));

	%afont = $ActionHUD::WhiteFont;
	if(Client::GetTeam(%killer) == Team::Friendly()) {
		%kfont = $ActionHUD::GreenFont;
	} else {
		%kfont = $ActionHUD::RedFont;
	}

	if(Client::GetTeam(%victim) == Team::Friendly()) {
		%vfont = $ActionHUD::GreenFont;
	} else {
		%vfont = $ActionHUD::RedFont;
	}

    if (isfunction(ActionHUD::Add)) {
        if((%killer == %victim) || (%kname == ""))
            ActionHUD::Add(%vfont@%vname@%afont@" : "@"[tk]");
        else
            ActionHUD::Add(%kfont@%kname@" "@%afont@"[tk]"@" "@%vfont@%vname);
    }
}

function KillMsgs::onClientKilled(%killer, %victim, %damagetype) {
	if ($pref::KillMsgs::ClientOnly && %killer != getManagerId() && %victim != getManagerId())
		return;

	%kname = String::escape(Client::GetName(%killer));
	%vname = String::escape(Client::GetName(%victim));

	%afont = $ActionHUD::WhiteFont;
	if(Client::GetTeam(%killer) == Team::Friendly()) {
		%kfont = $ActionHUD::GreenFont;
	} else {
		%kfont = $ActionHUD::RedFont;
	}

	if(Client::GetTeam(%victim) == Team::Friendly()) {
		%vfont = $ActionHUD::GreenFont;
	} else {
		%vfont = $ActionHUD::RedFont;
	}

    if (isfunction(ActionHUD::Add)) {
        if((%killer == %victim) || (%kname == ""))
            ActionHUD::Add(%vfont@%vname@%afont@" : "@KillMsgs::getname(%damageType));
        else
            ActionHUD::Add(%kfont@%kname@" "@%afont@KillMsgs::getname(%damageType)@" "@%vfont@%vname);
    }
}

function KillMsgs::getname(%damageType) {
	if (%damagetype == "Blaster") { return "[blstr]"; }
	if (%damagetype == "Chaingun") { return "[cg]"; }
	if (%damagetype == "Disc") { return "[disc]"; }
	if (%damagetype == "ELF") { return "[elf]"; }
	if (%damagetype == "Explosives") { return "[nade]"; }
	if (%damagetype == "Explosive") { return "[gl]"; }
	if (%damagetype == "Laser") { return "[lsr]"; }
	if (%damagetype == "Mortar") { return "[mrtr]"; }
	if (%damagetype == "Plasma") { return "[plas]"; }
	if (%damagetype == "Suicide") { return "[xxx]"; }
	if (%damagetype == "Turret") { return "[trrt]"; }
	return "[xxx]";
}

Event::Attach(eventClientKilled, KillMsgs::onClientKilled);
Event::Attach(eventClientTeamKilled, KillMsgs::onClientTeamKilled );
Event::Attach(eventClientSuicided, KillMsgs::onClientKilled );
