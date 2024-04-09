// FOV Changer
// Supports xPrefs
// By Smokey
// v0.3

$pref::FOV::Step = $pref::FOV::Step == "" ? 1 : $pref::FOV::Step;
$pref::FOV::Min = $pref::FOV::Min == "" ? 90 : $pref::FOV::Min;

function FOV::GameBinds::Init() after GameBinds::Init {
    if ($xPrefs::Installed)
        return;

	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "actionMap.sae");
	$GameBinds::CurrentMap = "actionMap.sae";
	GameBinds::addBindCommand( "Decrease FOV", "FOV::Decrease();");
	GameBinds::addBindCommand( "Increase FOV", "FOV::Increase();");
}

function FOV::Decrease() {
    FOV::Adjust(false);
}

function FOV::Increase() {
    FOV::Adjust(true);
}

function FOV::Adjust(%increase) {
    if (%increase)
        %fov = $pref::PlayerFov + $pref::FOV::Step;
    else
        %fov = $pref::PlayerFov - $pref::FOV::Step;

    if ($xLoader::fov == True || $LoaderPlugin::fov == True)
        %max = 160;
    else
        %max = 120;

    $pref::PlayerFov = clamp(%fov, max($pref::FOV::Min, 5.625), %max);

    %msg = "<jc><f2>Current FOV: <f1>" ~ $pref::PlayerFov;
    if (isFunction(remoteEP)) {
        remoteEP(%msg, 3, true, 1, 16, 250);
    } else {
        remoteBP(2048, %msg, 3);
    }
}

// ================================================================================
// xPrefs Support
// ================================================================================

function FOV::xSetup() after xPrefs::Setup {
    xPrefs::Create("FOV Changer", "FOV::xInit");
}

function FOV::xInit() {
	xPrefs::addText("FOV::Header", "FOV Changer");

    xPrefs::addTextEdit("FOV::Step", "Stepping Value", "$pref::FOV::Step", "True", "10");
    xPrefs::addTextEdit("FOV::Min", "Minimum Value", "$pref::FOV::Min", "True", "10");

    if ($xLoader::fov == True || $LoaderPlugin::fov == True)
        %max = 160;
    else
        %max = 120;

    xPrefs::addTextFormat("FOV::Max", "<f1>Maximum Value        <f0>" ~ %max, "", "", 19, 4);

	xPrefs::addBindCommand("actionMap.sae", "Decrease FOV", "FOV::Decrease();");
	xPrefs::addBindCommand("actionMap.sae", "Increase FOV", "FOV::Increase();");
}
