// Modified to support xPrefs

// Offset for inserting server snapshots into the client interpolation buffer
$pref::net::timeNudge = $pref::net::timeNudge == "" ? 48 : $pref::net::timeNudge;
$net::timeNudge = $pref::net::timeNudge;

// How far to allow the client's synced clock to drift from the server before correcting
$pref::net::clientClockCorrection = $pref::net::clientClockCorrection == "" ? 8 : $pref::net::clientClockCorrection;
$net::clientClockCorrection = $pref::net::clientClockCorrection;

// Customize the size of chaingun tracer bullets
$pref::tracerWidth = $pref::tracerWidth == "" ? 1.0 : $pref::tracerWidth;
$pref::tracerLength = $pref::tracerLength == "" ? 1.0 : $pref::tracerLength;

// Center third person camera above head like 1.40
$pref::newThirdPerson = $pref::newThirdPerson == "" ? false : $pref::newThirdPerson;

// Change damage flash opacity (only on XT servers)
$pref::damageFlash = $pref::damageFlash == "" ? 0.35 : $pref::damageFlash;


// ================================================================================
// xPrefs Support
// ================================================================================

function TribesXT::xSetup() after xPrefs::Setup {
    xPrefs::Create("TribesXT", "TribesXT::xInit");
}

function TribesXT::xInit() {

    if ($xLoader::TribesXT == True || $LoaderPlugin::TribesXT == True) {
        xPrefs::addText("TribesXT::Header", "TribesXT Preferences");
        xPrefs::addCheckbox("TribesXT::newThirdPerson", "New Third Person", "$pref::newThirdPerson");
        xPrefs::addTextEdit("TribesXT::timeNudge", "Time Nudge", "$pref::net::timeNudge", "True", "10", "True", "$net::timeNudge = $pref::net::timeNudge;");
        xPrefs::addTextEdit("TribesXT::clientClockCorrection", "Clock Correction", "$pref::net::clientClockCorrection", "True", "10", "True", "$net::clientClockCorrection = $pref::net::clientClockCorrection;");
        xPrefs::addTextEdit("TribesXT::tracerWidth", "Tracer Width", "$pref::tracerWidth", "True", "10");
        xPrefs::addTextEdit("TribesXT::tracerLength", "Tracer Length", "$pref::tracerLength", "True", "10");
        xPrefs::addTextEdit("TribesXT::damageFlash", "Damage Flash", "$pref::damageFlash", "True", "10");
    } else {
         xPrefs::addText("TribesXT::Header", "TribesXT Not Loaded");
    }
}
