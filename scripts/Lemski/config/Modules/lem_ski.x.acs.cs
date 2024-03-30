// Lemon's JumpJet for 1.40+
// requires unhappyjump.dll
// Modified to support xPrefs

$pref::Lem::JumpDelay = $pref::Lem::JumpDelay == "" ? 0.04 : $pref::Lem::JumpDelay;
$pref::Lem::Enabled = $pref::Lem::Enabled == "" ? "True" : $pref::Lem::Enabled;

function LemJumpJet::addBindsToMenu() after GameBinds::Init {
	if ($xPrefs::Installed)
		return;

	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "playMap.sae");
	$GameBinds::CurrentMap = "playMap.sae";
	GameBinds::addBindCommand( "Lemon's JumpJet", "Lem::JumpJet(1);", "Lem::JumpJet(0);" );
	GameBinds::addBindCommand( "Lemon's Ski", "Lem::Ski(1);", "Lem::Ski(0);" );
}

function Lem::JumpJet( %val ) {
	if ($pref::Lem::Enabled) {
		switch ( %val ) {
			case "0": { postAction( 2048, IDACTION_JET, 0 ); break; }
			case "1": { postAction( 2048, IDACTION_MOVEUP, 1 ); postAction( 2048, IDACTION_JET, 1 ); }
		}
	} else {
		switch ( %val ) {
			case "0": { postAction( 2048, IDACTION_JET, 0 ); break; }
			case "1": { postAction( 2048, IDACTION_JET, 1 ); }
		}
	}
}

function Lem::Ski( %val ) {
	if ($pref::Lem::Enabled) {
		switch ( %val ) {
			case "0": { postAction( 2048, IDACTION_MOVEUP, 0 ); schedule::cancel( "lemski" ); break; }
			case "1": { postAction( 2048, IDACTION_MOVEUP, 1 ); schedule::add( "Lem::Ski( 1 );", $pref::Lem::JumpDelay, "lemski" ); }
		}
	} else {
		switch ( %val ) {
			case "0": { postAction( 2048, IDACTION_MOVEUP, 0 ); break; }
			case "1": { postAction( 2048, IDACTION_MOVEUP, 1 ); }
		}
	}
}


// ================================================================================
// xPrefs Support
// ================================================================================

function LemJumpJet::xSetup() after xPrefs::Setup {
    xPrefs::Create("Lem Ski", "Lem::xInit");
}

function Lem::xInit() {
	xPrefs::addText("Lem::Header", "Lem Ski");

	xPrefs::addCheckbox("Lem::Checkbox", "Enabled", "$pref::Lem::Enabled");
    xPrefs::addTextEdit("Lem::TextEdit", "Jump Delay", "$pref::Lem::JumpDelay", "True", "10", $pref::Lem::Enabled);

	xPrefs::addBindCommand("playMap.sae", "Lemon's JumpJet", "Lem::JumpJet(1);", "Lem::JumpJet(0);");
	xPrefs::addBindCommand("playMap.sae", "Lemon's Ski", "Lem::Ski(1);", "Lem::Ski(0);");
}
