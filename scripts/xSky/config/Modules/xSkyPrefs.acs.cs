// xSkyPrefs by Smokey
// xLoader required
// xSky.dll required
// xPrefs required
// v0.7

$xSky::skyCount = 0; // Skies loaded from base/skies
$xSky::pref::Count = 0; // Loaded prefs

function xSky::xPrefs_Init() {

    if ($xLoader::xSky != True && $LoaderPlugin::xSky != True) {
        echoc(1, "xSky.dll is not loaded. xLoader is required to use xSkyPrefs.");
        return;
    }

    if ($xSky::xPrefs_Loaded)
        return;

    $xSky::xPrefs_Loaded = true;

    // Load sky volumes
    %file = File::findFirst("skies/*.zip");
    %i = 0;
    while (%file != "") {
        %id = newObject("Sky"~%i, SimVolume, %file);

        if (%id != 0) {
            //echoc(1, "[xSky] Adding volume #" ~ %i ~ ": " ~ %file);
            %i++;
        }

        %file = File::findNext("skies/*.zip");
    }

    // Default (None)
    $xSky::Sky[$xSky::skyCount] = "None";
    $xSky::File["None"] = "None";
    $xSky::Settings["None"] = "";
    $xSky::skyCount++;

    // Build sky list
    %file = File::findFirst("*_sky.dml");
    while (%file != "") {
        %name = String::Replace(%file, "_sky.dml", "");

        //echoc(1, "[xSky] Adding sky: " ~ %name);
        $xSky::Sky[$xSky::skyCount] = %name;
        $xSky::File[%name] = %file;
        $xSky::Settings[%name] = "";
        $xSky::skyCount++;

        %file = File::findNext("*_sky.dml");
    }

    // Load sky settings
    for (%i = 0; %i < $xSky::skyCount; %i++) {
        %name = $xSky::Sky[%i];
        %file = %name ~ "_sky.cs";

        //echoc(1, "[xSky] Loading settings for sky: " ~ %name);

        if (File::findFirst(%file) != "") {
            deleteVariables("$xSky::Settings::*");
            %result = exec(%file);

            $xSky::Settings::Rotation = $xSky::Settings::Rotation != "" ? $xSky::Settings::Rotation : "0";
            $xSky::Settings::Speed = $xSky::Settings::Speed != "" ? $xSky::Settings::Speed : "0";

            %settings = $xSky::Settings::Rotation ~ "|" ~ $xSky::Settings::Speed;
            $xSky::Settings[%name] = %settings;
            //echoc(1, "[xSky] Loading custom settings " ~ %name ~ ": " ~ %settings);
        } else {
            %settings = "0|0"; // Default settings
            $xSky::Settings[%name] = %settings;
            //echoc(1, "[xSky] Loading default settings " ~ %name ~ ": " ~ %settings);
        }
    }

    // Load the prefs
    %default = false;
    for (%i = 0; %i < $pref::xSky::Count; %i++) {

        //echoc(1, "[xSky] Trying Pref: " ~ %i);

        %mission = $pref::xSky::Mission[%i];
        %sky = $pref::xSky::Sky[%i];
        %settings = $pref::xSky::Settings[%i];

        if (String::explode(%settings, "|", "settings") != 2) {
            $pref::xSky::Settings[%i] = "";
            %settings = "";
        }

        if (%mission == "")
            continue;

        if (%mission == "Default")
            %default = true;

        if ($xSky::File[%sky] == "") // File does not exist anymore
            %sky = "None";

        $xSky::pref::Mission[$xSky::pref::Count] = %mission;
        $xSky::pref::Sky[$xSky::pref::Count] = %sky;
        $xSky::pref::Settings[$xSky::pref::Count] = %settings;
        $xSky::pref::Count++;

        $xSky::pref::Mission[%mission] = %sky;

        //echoc(1, "[xSky] Loading Pref " ~ %mission ~ ": " ~ %sky);

    }

    if (!%default) {
        $xSky::pref::Mission[$xSky::pref::Count] = "Default";
        $xSky::pref::Sky[$xSky::pref::Count] = "None";
        $xSky::pref::Settings[$xSky::pref::Count] = "";
        $xSky::pref::Count++;

        $xSky::pref::Mission["Default"] = "None";
    }

    // Delete client prefs
    deleteVariables("$pref::xSky::Mission*");
    deleteVariables("$pref::xSky::Sky*");
    deleteVariables("$pref::xSky::Settings*");
    $pref::xSky::Count = 0;

    Event::Attach(eventExit, xSky::xPrefs_onExit);
    Event::Attach(eventMissionInfo, xSky::xPrefs_eventMissionInfo);

}

function xSky::xPrefs_onExit() {

	for (%i = 0; %i < $xSky::pref::Count; %i++) {

        %mission = $xSky::pref::Mission[%i];
        %sky = $xSky::pref::Sky[%i];
        %settings = $xSky::pref::Settings[%i];

        if (%sky == "None")
            continue;

        $pref::xSky::Mission[$pref::xSky::Count] = %mission;
        $pref::xSky::Sky[$pref::xSky::Count] = %sky;

        if (%settings != "" && %settings != $xSky::Settings[%sky]) { // Don't save default settings
            $pref::xSky::Settings[$pref::xSky::Count] = %settings;
        }

        $pref::xSky::Count++;

	}
}

function xSky::xPrefs_eventMissionInfo() {
    if (!isObject(729)) // Object 729 is the Sky
        return;

    if ($ServerMission != "") {
        // Backup map's default sky settings
        $xSky::DefaultDML = xSky::getDML();
        $xSky::DefaultRotation = xSky::getRotation();
    }

    xSky::xPrefs_loadSky();
}

function xSky::xPrefs_loadSky() {

    if (!isObject(729)) // Object 729 is the Sky
        return;

    if ($ServerMission != "") {
        %sky = $xSky::pref::Mission[$ServerMission];

        if (%sky == "" || %sky == "None") {
            %sky = $xSky::pref::Mission["Default"];
        }

        %file = $xSky::File[%sky];
        %settings = $xSky::Settings[%sky];

        //echoc(1, "[xSky] loadSky file: " ~ %file);

        if (%sky == "" || %sky == "None") {
            // Load map default
            xSky::setDML($xSky::DefaultDML);
            xSky::setRotation($xSky::DefaultRotation);
            xSky::setSpeed(0);
            xSky::Disable();
        } else {
            if (%file != "" && %file != "None") {

                //echoc(1, "[xSky] Set DML: " ~ %file);

                xSky::Enable();
                xSky::setDML(%file);

                if (%settings != "" && String::explode(%settings, "|", "settings") == 2) {

                    %rotation = $settings[0];
                    %speed = $settings[1];

                    //echoc(1, "[xSky] Settings:");
                    //echoc(1, "[xSky] Rotation: " ~ %rotation);
                    //echoc(1, "[xSky] Speed: " ~ %speed);

                    xSky::setRotation(%rotation);
                    xSky::setSpeed(%speed);

                }

                if ($xSky::Rotation != "") {
                    xSky::setRotation($xSky::Rotation);
                }

                if ($xSky::Speed != "") {
                    xSky::setSpeed($xSky::Speed);
                }
            }
        }
    }
}

xSky::xPrefs_Init();

// ================================================================================
// xPrefs Support
// ================================================================================

$xSky::Mission[0] = "Default";
$xSky::Mission[1] = "Current Mission";

function xSky::xSetup() after xPrefs::Setup {
    xPrefs::Create("xSky", "xSky::xInit");
}

function xSky::xInit() {
    if ($xLoader::xSky == True || $LoaderPlugin::xSky == True) {

        xPrefs::addText("xSky::Header1", "xSky");

        xPrefs::addText("xSky::Header2", "Mission");
        xPrefs::addComboBox("xSky::ComboMission", "Mission", "", "xSky::MissionComboChange();", True, "$xSky::Mission");

        xPrefs::addText("xSky::Header3", "Sky Setting");
        xPrefs::addText("xSky::MissionName", "Mission: ", "False", 34, 0, 8);
        xPrefs::addComboBox("xSky::ComboSky", "Sky", "", "xSky::SkyComboChange();", True, "$xSky::Sky");

        xPrefs::AddTextFormat("xSky::Image", "", 275, False, 28, 20, "");

        FGCombo::setSelected("xSky::ComboMission", 0);
        xSky::MissionComboChange();

    } else {
        xPrefs::addText("xSky::Header1", "xSky.dll Not Loaded.");
    }
}

function xSky::MissionComboChange() {

	%selectedMission = FGCombo::getSelectedText("xSky::ComboMission");

    if (%selectedMission == "Default") {
        %mission = "Default";
    } else if (%selectedMission == "Current Mission") {
        if ($ServerMission != "" && isObject(729)) {
            %mission = $ServerMission;
        } else {
            %mission = "";
        }
    }

    if (%mission != "") {
        Control::setActive("xSky::ComboSky", true);
        Control::setValue("xSky::MissionName", "Mission: " ~ %mission);
    } else {
        Control::setActive("xSky::ComboSky", false);
        Control::setValue("xSky::MissionName", "Mission: Not Connected");
    }

    %sky = $xSky::pref::Mission[%mission] != "" ? $xSky::pref::Mission[%mission] : "None";

    %select = 0;
    for (%i = 0; %i < $xSky::skyCount; %i++) {
        if ($xSky::Sky[%i] == %sky) {
            %select = %i;
        }
    }

	FGCombo::setSelected("xSky::ComboSky",  %select);
	xSky::SkyComboChange();

}

function xSky::SkyComboChange() {

	%selectedMission = FGCombo::getSelectedText("xSky::ComboMission");
    %sky = FGCombo::getSelectedText("xSky::ComboSky");

    if (%selectedMission == "Default") {
        %mission = "Default";
    } else if (%selectedMission == "Current Mission") {
        if ($ServerMission != "" && isObject(729)) {
            %mission = $ServerMission;
        } else {
            %mission = "";
        }
    }

    %found = false;
    for (%i = 0; %i < $xSky::pref::Count; %i++) {
        if ($xSky::pref::Mission[%i] == %mission) {
            $xSky::pref::Sky[%i] = %sky;
            $xSky::pref::Mission[%mission] = %sky;

            %found = true;
            break;
        }
    }

    if (!%found && %mission != "") {
        $xSky::pref::Mission[$xSky::pref::Count] = %mission;
        $xSky::pref::Sky[$xSky::pref::Count] = %sky;
        $xSky::pref::Mission[%mission] = %sky;
        $xSky::pref::Count++;
    }

    // Check for preview image and display here
    %imgFile = %sky ~ "_sky.png";
    if (File::findFirst(%imgFile) != "") {
        %image = "<jc><B0,0:"~%imgFile~">";
    } else {
        %image = "<jc>No Preview";
    }
    Control::setValue("xSky::Image", %image);

    xSky::xPrefs_loadSky();

}

function xSky::OnGuiOpen(%gui) {
    if (%gui == "OptionsGui") {
        xSky::MissionComboChange();
    }
}

Event::Attach(eventGuiOpen, xSky::OnGuiOpen);