// QuickSwitch
// Toggle between two weapons (on base) or next weapon on other mods
// Intended to be used with 'q' key
// Supports xPrefs
// By Smokey
// v0.2

$pref::QuickSwitch::WepA = $pref::QuickSwitch::WepA == "" ? "Disc Launcher" : $pref::QuickSwitch::WepA;
$pref::QuickSwitch::WepB = $pref::QuickSwitch::WepB == "" ? "Grenade Launcher" : $pref::QuickSwitch::WepB;

$QuickSwitch::Weapon[0] = "Blaster";
$QuickSwitch::Weapon[1] = "Chaingun";
$QuickSwitch::Weapon[2] = "Disc Launcher";
$QuickSwitch::Weapon[3] = "ELF Gun";
$QuickSwitch::Weapon[4] = "Grenade Launcher";
$QuickSwitch::Weapon[5] = "Laser Rifle";
$QuickSwitch::Weapon[6] = "Mortar";
$QuickSwitch::Weapon[7] = "Plasma Gun";
$QuickSwitch::Weapon[8] = "Targeting Laser";

function QuickSwitch::addBindsToMenu() after GameBinds::Init {
	if ($xPrefs::Installed)
		return;

	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "playMap.sae");
	$GameBinds::CurrentMap = "playMap.sae";
	GameBinds::addBindCommand("QuickSwitch", "QuickSwitch::nextWeapon();");
}

function QuickSwitch::nextWeapon() {
    if ($servermod == "" || String::Starts($servermod, "base")) {
        %weap = getitemdesc(getMountedItem(0));

        if (%weap == $pref::QuickSwitch::WepA && GetItemCount($pref::QuickSwitch::WepB) == 1) {
            use($pref::QuickSwitch::WepB);
        } else if (%weap != $pref::QuickSwitch::WepA && GetItemCount($pref::QuickSwitch::WepA) == 1) {
            use($pref::QuickSwitch::WepA);
        } else {
            nextWeapon();
        }
    } else {
        nextWeapon();
    }
}

// ================================================================================
// xPrefs Support
// ================================================================================

function QuickSwitch::xSetup() after xPrefs::Setup {
    xPrefs::Create("QuickSwitch", "QuickSwitch::xInit");
}

function QuickSwitch::xInit() {
	xPrefs::addText("QuickSwitch::Header", "QuickSwitch");

    xPrefs::addComboBox("QuickSwitch::ComboA", "Weapon A", "$pref::QuickSwitch::WepA", "", true, "$QuickSwitch::Weapon");
    xPrefs::addComboBox("QuickSwitch::ComboB", "Weapon B", "$pref::QuickSwitch::WepB", "", true, "$QuickSwitch::Weapon");

	xPrefs::addBindCommand("playMap.sae", "QuickSwitch", "QuickSwitch::nextWeapon();");
}
