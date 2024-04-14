// WepRet
// Supports xEvent and xPrefs
// By Smokey
// v0.1


$pref::WepRet::Enabled = $pref::WepRet::Enabled == "" ? "True" : $pref::WepRet::Enabled;

$pref::WepRet::Reticle["Blaster"] = $pref::WepRet::Reticle["Blaster"] == "" ? "1" : $pref::WepRet::Reticle["Blaster"];
$pref::WepRet::Reticle["Chaingun"] = $pref::WepRet::Reticle["Chaingun"] == "" ? "3" : $pref::WepRet::Reticle["Chaingun"];
$pref::WepRet::Reticle["Disc_Launcher"] = $pref::WepRet::Reticle["Disc_Launcher"] == "" ? "1" : $pref::WepRet::Reticle["Disc_Launcher"];
$pref::WepRet::Reticle["ELF_Gun"] = $pref::WepRet::Reticle["ELF_Gun"] == "" ? "1" : $pref::WepRet::Reticle["ELF_Gun"];
$pref::WepRet::Reticle["Grenade_Launcher"] = $pref::WepRet::Reticle["Grenade_Launcher"] == "" ? "2" : $pref::WepRet::Reticle["Grenade_Launcher"];
$pref::WepRet::Reticle["Laser_Rifle"] = $pref::WepRet::Reticle["Laser_Rifle"] == "" ? "1" : $pref::WepRet::Reticle["Laser_Rifle"];
$pref::WepRet::Reticle["Mortar"] = $pref::WepRet::Reticle["Mortar"] == "" ? "1" : $pref::WepRet::Reticle["Mortar"];
$pref::WepRet::Reticle["Plasma_Gun"] = $pref::WepRet::Reticle["Plasma_Gun"] == "" ? "1" : $pref::WepRet::Reticle["Plasma_Gun"];
$pref::WepRet::Reticle["Targeting_Laser"] = $pref::WepRet::Reticle["Targeting_Laser"] == "" ? "1" : $pref::WepRet::Reticle["Targeting_Laser"];

$WepRet::Weapon[0] = "Blaster";
$WepRet::Weapon[1] = "Chaingun";
$WepRet::Weapon[2] = "Disc Launcher";
$WepRet::Weapon[3] = "ELF Gun";
$WepRet::Weapon[4] = "Grenade Launcher";
$WepRet::Weapon[5] = "Laser Rifle";
$WepRet::Weapon[6] = "Mortar";
$WepRet::Weapon[7] = "Plasma Gun";
$WepRet::Weapon[8] = "Targeting Laser";
$WepRet::WeaponCount = 9;

function WepRet::Init() {

	if ($WepRet::Loaded)
		return;

	$WepRet::Loaded = true;

	%w = getWord(String::replace($pref::VideoFullScreenRes, "x", " "), 0);
    %h = getWord(String::replace($pref::VideoFullScreenRes, "x", " "), 1);

    // Dimension of reticle images
    %hm = 128;

    %x = ((%w/2) - (%hm/2));
    %y = ((%h/2) - (%hm/2));

	$pref::hudPositionsWepRet::Reticle = "";
	HUD::New("WepRet::Reticle", %x, %y, %hm, %hm, WepRet::Wake, WepRet::Sleep);

	newObject("WepRet::Img", FearGuiFormattedText, 0, 0, %hm, %hm);
	HUD::Add("WepRet::Reticle", "WepRet::Img");

	if ($pref::WepRet::Enabled) {
		Control::setValue("WepRet::Img", "<B0,0:Modules/WepRet/Reticle1.png>");
	} else {
		Control::setValue("WepRet::Img", "<B0,0:Modules/WepRet/H_Reticle.png>");
	}
}

function WepRet::Wake() { WepRet::Update(); }
function WepRet::Sleep() { }

function WepRet::Update(%item) {

	if (!$pref::WepRet::Enabled) {
		Control::setValue("WepRet::Img", "<B0,0:Modules/WepRet/H_Reticle.png>");
		return;
	}

	%item = %item == "" ? getMountedItem(0) : %item;
	%weapon = String::Replace(getItemDesc(%item), " ", "_");

	if (%weapon != "" && $pref::WepRet::Reticle[%weapon] != "") {

		%file = "Modules/WepRet/Reticle"~$pref::WepRet::Reticle[%weapon]~".png";

		if (File::FindFirst(%file) != "") {
			%ret = "<B0,0:"~%file~">";
		} else {
			echoc(1, "WepRet: File ["~%file~"] not found.");
			$pref::WepRet::Reticle[%weapon] = 1;
			%ret = "<B0,0:Modules/WepRet/Reticle1.png>";
		}

	} else {
		%ret = "<B0,0:Modules/WepRet/Reticle1.png>";
	}

	Control::setValue("WepRet::Img", %ret);

	if (!$xEvent::Loaded)
		Schedule::Add("WepRet::Update();", 0.1); // Use Schedule if xEvent.dll is not loaded
}

function WepRet::GuiOpen(%gui) {
	if (%gui == "playGui")
		WepRet::Update();
}

function WepRet::ItemMountUpdate(%slot, %item) {
    if (%slot == 0)
		WepRet::Update(%item);
}


Event::Attach(eventGuiOpen, WepRet::GuiOpen);
Event::Attach(eventItemMountUpdate, WepRet::ItemMountUpdate); // Requires xEvent.dll
Event::Attach(eventControlObjectChange, WepRet::Update); // Requires xEvent.dll

WepRet::Init();


// ================================================================================
// xPrefs Support
// ================================================================================

function WepRet::xSetup() after xPrefs::Setup {
    xPrefs::Create("Weapon Reticles", "WepRet::xInit");
}

function WepRet::xInit() {
    xPrefs::addText("WepRet::Header1", "Weapon Reticles");
	xPrefs::addCheckbox("WepRet::Checkbox", "Enabled", "$pref::WepRet::Enabled");

    for (%i = 1; true; %i++) {
        %file = "Modules/WepRet/Reticle"~%i~".png";

        if (File::FindFirst(%file) != "") {
            %x = %i-1;
            $WepRet::Image[%x] = %i;
        } else {
            break;
        }
    }

	xPrefs::addText("WepRet::Header2", "Reticle Images");
    xPrefs::addComboBox("WepRet::ComboWeapon", "Weapon", "", "WepRet::WeaponComboChange();", True, "$WepRet::Weapon");
    xPrefs::addComboBox("WepRet::ComboReticle", "Reticle", "", "WepRet::ReticleComboChange();", True, "$WepRet::Image");
    xPrefs::AddTextFormat("WepRet::Image", "", 128, False, "", 40);

    FGCombo::setSelected("WepRet::ComboWeapon", 0);
    WepRet::WeaponComboChange();

}

function WepRet::WeaponComboChange() {
	%weapon = String::Replace(FGCombo::getSelectedText("WepRet::ComboWeapon"), " ", "_");

	if ($pref::WepRet::Reticle[%weapon] == "")
		$pref::WepRet::Reticle[%weapon] = 1;

	FGCombo::setSelected("WepRet::ComboReticle", $pref::WepRet::Reticle[%weapon] - 1);
	WepRet::ReticleComboChange();
}

function WepRet::ReticleComboChange() {
	%weapon = String::Replace(FGCombo::getSelectedText("WepRet::ComboWeapon"), " ", "_");
	%reticle = FGCombo::getSelectedText("WepRet::ComboReticle");

	$pref::WepRet::Reticle[%weapon] = %reticle;

    %image = "<jc><B0,0:Modules/WepRet/Reticle"~%reticle~".png>";
    Control::setValue("WepRet::Image", %image);
}
