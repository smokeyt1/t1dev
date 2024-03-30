// MultiBinds
// Supports xPrefs
// by Smokey
// v0.1

$pref::MultiBinds::Bind1A = $pref::MultiBinds::Bind1A == "" ? "Disc Launcher" : $pref::MultiBinds::Bind1A;
$pref::MultiBinds::Bind1B = $pref::MultiBinds::Bind1B == "" ? "Grenade Launcher" : $pref::MultiBinds::Bind1B;
$pref::MultiBinds::Bind2A = $pref::MultiBinds::Bind2A == "" ? "Chaingun" : $pref::MultiBinds::Bind2A;
$pref::MultiBinds::Bind2B = $pref::MultiBinds::Bind2B == "" ? "Plasma Gun" : $pref::MultiBinds::Bind2B;
$pref::MultiBinds::Bind3A = $pref::MultiBinds::Bind3A == "" ? "Laser Rifle" : $pref::MultiBinds::Bind3A;
$pref::MultiBinds::Bind3B = $pref::MultiBinds::Bind3B == "" ? "Blaster" : $pref::MultiBinds::Bind3B;
$pref::MultiBinds::Bind4A = $pref::MultiBinds::Bind4A == "" ? "Mortar" : $pref::MultiBinds::Bind4A;
$pref::MultiBinds::Bind4B = $pref::MultiBinds::Bind4B == "" ? "ELF Gun" : $pref::MultiBinds::Bind4B;
$pref::MultiBinds::Bind5A = $pref::MultiBinds::Bind5A == "" ? "Targeting Laser" : $pref::MultiBinds::Bind5A;
$pref::MultiBinds::Bind5B = $pref::MultiBinds::Bind5B == "" ? "None" : $pref::MultiBinds::Bind5B;
$MultiBinds::BindCount = 5;

$MultiBinds::Weapon[0] = "Blaster";
$MultiBinds::Weapon[1] = "Chaingun";
$MultiBinds::Weapon[2] = "Disc Launcher";
$MultiBinds::Weapon[3] = "ELF Gun";
$MultiBinds::Weapon[4] = "Grenade Launcher";
$MultiBinds::Weapon[5] = "Laser Rifle";
$MultiBinds::Weapon[6] = "Mortar";
$MultiBinds::Weapon[7] = "Plasma Gun";
$MultiBinds::Weapon[8] = "Targeting Laser";
$MultiBinds::Weapon[9] = "None";
$MultiBinds::WeaponCount = 10;

function MultiBinds::Use(%key) {

	%weapon = getitemdesc(getMountedItem(0));
	%weaponA = $pref::MultiBinds::Bind[%key ~ "A"];
	%weaponB = $pref::MultiBinds::Bind[%key ~ "B"];

	if (%weapon == %weaponA && GetItemCount(%weaponB) == 1) {
		use(%weaponB);
	} else if (%weap != %weaponA && GetItemCount(%weaponA) == 1) {
		use(%weaponA);
	} else if (%weap != %weaponB && GetItemCount(%weaponB) == 1) {
		use(%weaponB);
	}

}

function MultiBinds::Binds() after GameBinds::Init {

	if ($xPrefs::Installed)
		return;

	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2("playMap.sae");
	$GameBinds::CurrentMap = "playMap.sae";
	
	for (%i = 1; %i <= $MultiBinds::BindCount; %i++) {
		GameBinds::addBindCommand("MultiBind #" ~ %i, "MultiBinds::Use(" ~ %i ~ ");");
	}

}

// ================================================================================
// xPrefs Support
// ================================================================================

function MultiBinds::xSetup() after xPrefs::Setup {
    xPrefs::Create("MultiBinds", "MultiBinds::xInit");
}

function MultiBinds::xInit() {

	xPrefs::addText("MultiBinds::Header", "Weapon Selections");

	for (%i = 1; %i <= $MultiBinds::BindCount; %i++) {
		xPrefs::addText("MultiBinds::Header" ~ %i, "Bind #" ~ %i, "False");
		MultiBinds::AddComboBox("MultiBinds::Combo" ~ %i ~ "A", "Bind " ~ %i ~ "A", "$pref::MultiBinds::Bind" ~ %i ~ "A");
		MultiBinds::AddComboBox("MultiBinds::Combo" ~ %i ~ "B", "Bind " ~ %i ~ "B", "$pref::MultiBinds::Bind" ~ %i ~ "B");

		xPrefs::addBindCommand("playMap.sae", "Bind #" ~ %i, "MultiBinds::Use(" ~ %i ~ ");");
	}

}

function MultiBinds::AddComboBox(%objName, %title, %variable) {

	xPrefs::addComboBox(%objName, %title, %variable);

	for (%i = 0; %i < $MultiBinds::WeaponCount; %i++) {
		FGCombo::addEntry(%objName, $MultiBinds::Weapon[%i], %i);
	}

    FGCombo::setSelected(%objName, FGCombo::FindEntry(%objName, *%variable));

}
