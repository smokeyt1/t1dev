// FlagMsgs for 1.40/1.41
// Displays flag messages (capture/return/drop/grab) in ActionHUD and filters flag messages from the chat hud
// Uses ActionHUD to display messages
// Uses Match.cs from PrestoPack
// Preferences (to change defaults, edit in config/ClientPrefs.cs or in console):
//	$pref::FlagMsgs::ClientOnly = "false"; 		// Only display messages that include ourselves (Default: false)
//	$pref::FlagMsgs::FlagSounds = "true";		// Play flag sounds that are filtered out of the chat hud (Default: true)
// Smokey 2023

// Preferences. Do not edit here - to change defaults, edit in config/ClientPrefs.cs or in console
$pref::FlagMsgs::ClientOnly = $pref::FlagMsgs::ClientOnly == "" ? false : $pref::FlagMsgs::ClientOnly;
$pref::FlagMsgs::FlagSounds = $pref::FlagMsgs::FlagSounds == "" ? true : $pref::FlagMsgs::FlagSounds;

// Filter chat messages
$FlagMsgs::Filterid = -1;
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "* captured the * flag!";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "* took the * flag!";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "* returned the * flag!";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "* dropped the * flag!";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "Your team's flag was captured.";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "Your team captured the flag.";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "Your team has the * flag.";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "Your team's flag has been taken.";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "You returned the * flag!";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "The * flag was returned to base.";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "Your flag was returned to base.";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "You dropped the * flag!";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "Your flag was dropped in the field.";
$FlagMsgs::Filter[$FlagMsgs::Filterid++] = "The * flag was dropped in the field.";

function FlagMsgs::Action(%team, %cl, %action) {
	if ($pref::FlagMsgs::ClientOnly && %cl != getManagerId())
		return;

	%pname = Client::GetName(%cl);
    %tname = $Team::Name[%team];

	%afont = $ActionHUD::WhiteFont;
    if(Client::GetTeam(%cl) == Team::Friendly())
		%pfont = $ActionHUD::GreenFont;
	else
		%pfont = $ActionHUD::RedFont;

	if (Client::getTeam(getManagerId()) == %team) {
		%tfont = $ActionHUD::GreenFont;
	} else {
		%tfont = $ActionHUD::RedFont;
	}

	if (isfunction(ActionHUD::Add)) {
		ActionHUD::Add(%pfont@(%pname != ""?%pname@" ":"")@%afont@"[Flag "@%action@"] "@%tfont@%tname);
	}
}

function FlagMsgs::Captured(%team, %cl) {
    FlagMsgs::Action(%team, %cl, "Captured");
}

function FlagMsgs::Taken(%team, %cl) {
    FlagMsgs::Action(%team, %cl, "Taken");
}

function FlagMsgs::Returned(%team, %cl) {
    FlagMsgs::Action(%team, %cl, "Returned");
}

function FlagMsgs::Dropped(%team, %cl) {
    FlagMsgs::Action(%team, %cl, "Dropped");
}

Event::Attach(eventFlagCap, FlagMsgs::Captured);
Event::Attach(eventFlagPickup, FlagMsgs::Taken);
Event::Attach(eventFlagGrab, FlagMsgs::Taken);
Event::Attach(eventFlagDrop, FlagMsgs::Dropped);
Event::Attach(eventFlagReturn, FlagMsgs::Returned);

function flagmsgs(%cl, %msg, %type) before onClientMessage {
	%ret = true;

	%message = %msg;
	%index = String::findSubStr(%msg, "~" );
	if (%index != -1) {
		%tags = String::getSubStr(%msg, %index + 1, 10000);
		%message = String::getSubStr(%msg, 0, %index);
	}

	// Filter messages
	for (%i = 0; %i <= $FlagMsgs::Filterid; %i++) {
		if (Match::String(String::trim(%message), $FlagMsgs::Filter[%i])) {
            %ret = false;

			if ($pref::FlagMsgs::FlagSounds) {
				// Play sound in message
				if (String::ends(%tags, ".wav")) {
					%sound = String::getSubStr(%tags, 1, (String::len(%tags)-5));
					localSound(%sound);
				}
			}

            break;
		}
	}

	if (!%ret) {
		halt "0";
	} else {
		return;
	}
}

