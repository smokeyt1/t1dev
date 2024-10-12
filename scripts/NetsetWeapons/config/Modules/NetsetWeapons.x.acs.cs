// NetsetWeapons
// Change netset settings based on mounted weapon
// Requires xPrefs and xEvent
// By Smokey
// v0.4

$NetsetWep::Weapon[0] = "Blaster";
$NetsetWep::Weapon[1] = "Chaingun";
$NetsetWep::Weapon[2] = "Disc Launcher";
$NetsetWep::Weapon[3] = "ELF Gun";
$NetsetWep::Weapon[4] = "Grenade Launcher";
$NetsetWep::Weapon[5] = "Laser Rifle";
$NetsetWep::Weapon[6] = "Mortar";
$NetsetWep::Weapon[7] = "Plasma Gun";
$NetsetWep::Weapon[8] = "Targeting Laser";
$NetsetWep::Weapon[9] = "Default";
$NetsetWep::WeaponCount = 10;

$pref::NetsetWep::Popup = $pref::NetsetWep::Popup == "" ? False : $pref::NetsetWep::Popup;

function NetsetWep::Init() {
    if ($NetsetWep::Loaded)
        return;

    $NetsetWep::Loaded = true;

    for (%i = 0; %i < $NetsetWep::WeaponCount; %i++) {
        %weapon = String::Replace($NetsetWep::Weapon[%i], " ", "_");

        $pref::NetsetWep::TERP[%weapon] = $pref::NetsetWep::TERP[%weapon] == "" ? 32 : $pref::NetsetWep::TERP[%weapon];
        $pref::NetsetWep::PFT[%weapon] = $pref::NetsetWep::PFT[%weapon] == "" ? 64 : $pref::NetsetWep::PFT[%weapon];
    }

}

function NetsetWep::Update(%slot, %item) {
    if ($xLoader::netset != True && $LoaderPlugin::netset != True) return;

    if (%slot != 0 || %item == -1)
        return;

    %desc = getItemDesc(%item);
    %weapon = String::Replace(%desc, " ", "_");

    if ($pref::NetsetWep::TERP[%weapon] != "" && $pref::NetsetWep::PFT[%weapon] != "") {
        $net::interpolateTime = $pref::NetsetWep::TERP[%weapon];
        $net::predictForwardTime = $pref::NetsetWep::PFT[%weapon];
    } else {
        $net::interpolateTime = $pref::NetsetWep::TERP["Default"];
        $net::predictForwardTime = $pref::NetsetWep::PFT["Default"];
    }

    if ($pref::NetsetWep::Popup) {
        %msg = "<jc><f2>Weapon: <f1>" ~ %desc;
        %msg = %msg ~ "\n<f2>Interpolate: <f1>" ~ $net::interpolateTime;
        %msg = %msg ~ "\n<f2>Predict Forward: <f1>" ~ $net::predictForwardTime;

        if (isFunction(remoteEP)) {
            remoteEP(%msg, 3, true, 3, 16, 250);
        } else {
            remoteBP(2048, %msg, 3);
        }
    }
}

function NetsetWep::ControlObjectChange() {
    %item = getMountedItem(0);

    if (PSC::getControlMode() == "playing" && %item != "" && %item != -1) {
        NetsetWep::Update(0, %item);
    }
}

Event::Attach(eventItemMountUpdate, NetsetWep::Update); // Requires xEvent.dll
Event::Attach(eventControlObjectChange, NetsetWep::ControlObjectChange); // Requires xEvent.dll

NetsetWep::Init();

// ================================================================================
// xPrefs Support
// ================================================================================

function NetsetWep::xSetup() after xPrefs::Setup {
    xPrefs::Create("NetsetWeapons", "NetsetWep::xInit");
}

function NetsetWep::xInit() {
    if ($xLoader::netset == True || $LoaderPlugin::netset == True) {
        xPrefs::addText("NetsetWep::Header_Netset", "Netset Settings");

        xPrefs::addComboBox("NetsetWep::NetCombo", "Weapon", "", "NetsetWep::NetComboChange();");
        for (%i = 0; %i < $NetsetWep::WeaponCount; %i++) {
            FGCombo::addEntry("NetsetWep::NetCombo", $NetsetWep::Weapon[%i], %i);
        }

        xPrefs::addTextEdit("NetsetWep::TERP", "Interpolate Time", "", "True", "10", True, "NetsetWep::NetTextChange();");
        xPrefs::addTextEdit("NetsetWep::PFT", "Predict Forward Time", "", "True", "10", True, "NetsetWep::NetTextChange();");

        xPrefs::addText("NetsetWep::Header_Other", "Other");
        xPrefs::addCheckbox("NetsetWep::Checkbox1", "Popup on change", "$pref::NetsetWep::Popup");

        FGCombo::setSelected("NetsetWep::NetCombo", 0);
        NetsetWep::NetComboChange();
    } else {
         xPrefs::addText("NetsetWep::Header", "Netset Not Loaded");
    }
}

function NetsetWep::NetComboChange() {
    %weapon = String::Replace(FGCombo::getSelectedText("NetsetWep::NetCombo"), " ", "_");

	%terp = $pref::NetsetWep::TERP[%weapon];
    %pft = $pref::NetsetWep::PFT[%weapon];

	Control::setValue("NetsetWep::TERP", %terp);
    Control::setValue("NetsetWep::PFT", %pft);
}

function NetsetWep::NetTextChange() {
	%weapon = String::Replace(FGCombo::getSelectedText("NetsetWep::NetCombo"), " ", "_");

	%terp = Control::getValue("NetsetWep::TERP");
    %pft = Control::getValue("NetsetWep::PFT");

	$pref::NetsetWep::TERP[%weapon] = %terp;
	$pref::NetsetWep::PFT[%weapon] = %pft;
}
