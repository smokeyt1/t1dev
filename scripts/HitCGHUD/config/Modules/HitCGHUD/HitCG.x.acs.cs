// HitCGHUD by killer2001
// Modified by Smokey (incl. support for Binds and xPrefs)
// v0.1

function HitCGHUD::Binds() after GameBinds::Init {
	if ($xPrefs::Installed)
		return;

	$GameBinds::CurrentMapHandle = GameBinds::GetActionMap2( "playMap.sae");
	$GameBinds::CurrentMap = "playMap.sae";
	GameBinds::addBindCommand("HitCGHUD Toggle", "HitCGHUD::Toggle();");
}

$pref::HitCGHUD::Enabled = $pref::HitCGHUD::Enabled == "" ? "True" : $pref::HitCGHUD::Enabled;
$pref::HitCGHUD::Hitmark = $pref::HitCGHUD::Hitmark == "" ? "1" : $pref::HitCGHUD::Hitmark;

function HitCGHUD::Init() {
    if ($HitCGHUD::Loaded)
        return;

	$HitCGHUD::Loaded = true;

    %w = getWord(String::replace($pref::VideoFullScreenRes, "x", " "), 0);
    %h = getWord(String::replace($pref::VideoFullScreenRes, "x", " "), 1);
    
    //dimension of hitmarker image file is 30x30
    %hm = 30;
    
    %x = ((%w/2) - (%hm/2));
    %y = ((%h/2) - (%hm/2));
    
    $pref::hudPositionsHitCGHUD::Container = "";
	Hud::New( "HitCGHUD::Container", %x, %y, %hm, %hm, HitCGHUD::Wake, HitCGHUD::Sleep );

	newObject("HitCGHUD::Marker100", FearGuiFormattedText, 0, 0, %hm, %hm);
    newObject("HitCGHUD::Marker75", FearGuiFormattedText, 0, 0, %hm, %hm);
    newObject("HitCGHUD::Marker50", FearGuiFormattedText, 0, 0, %hm, %hm);
    newObject("HitCGHUD::Marker25", FearGuiFormattedText, 0, 0, %hm, %hm);
    
	Hud::Add( "HitCGHUD::Container", "HitCGHUD::Marker100" );
    Hud::Add( "HitCGHUD::Container", "HitCGHUD::Marker75" );
    Hud::Add( "HitCGHUD::Container", "HitCGHUD::Marker50" );
    Hud::Add( "HitCGHUD::Container", "HitCGHUD::Marker25" );
    
    Control::SetVisible("HitCGHUD::Marker100", false);
    Control::SetVisible("HitCGHUD::Marker75", false);
    Control::SetVisible("HitCGHUD::Marker50", false);
    Control::SetVisible("HitCGHUD::Marker25", false);

    HitCGHUD::UpdateMarker();

}

function HitCGHUD::UpdateMarker() {

    %file = "Modules/HitCGHUD/hitmark"~$pref::HitCGHUD::Hitmark~"_100.png";

    if (File::FindFirst(%file) == "") {
        $pref::HitCGHUD::Hitmark = 1;
    }

    %marker100 = "<B0,0:Modules/HitCGHUD/hitmark"~$pref::HitCGHUD::Hitmark~"_100.png>";
    %marker75 = "<B0,0:Modules/HitCGHUD/hitmark"~$pref::HitCGHUD::Hitmark~"_75.png>";
    %marker50 = "<B0,0:Modules/HitCGHUD/hitmark"~$pref::HitCGHUD::Hitmark~"_50.png>";
    %marker25 = "<B0,0:Modules/HitCGHUD/hitmark"~$pref::HitCGHUD::Hitmark~"_25.png>";

    control::setValue( "HitCGHUD::Marker100", %marker100 );
    control::setValue( "HitCGHUD::Marker75", %marker75 );
    control::setValue( "HitCGHUD::Marker50", %marker50 );
    control::setValue( "HitCGHUD::Marker25", %marker25 );
}

function HitCGHUD::Wake() {
}

function HitCGHUD::Sleep() {
}

function HitCGHUD::Toggle() {
    if ($pref::HitCGHUD::Enabled) {
        $pref::HitCGHUD::Enabled = false;
        remoteEP("<f2>HitCG HUD: <f1>OFF", 3, 2, 2, 10, 300);
        HitCGHUD::ClearAll();
    } else {
        $pref::HitCGHUD::Enabled = true;
        remoteEP("<f2>HitCG HUD: <f1>ON", 3, 2, 2, 10, 300);
        HitCGHUD::ClearAll();
        Control::SetVisible("HitCGHUD::Marker100", true);
        Schedule::Add("HitCGHUD::ClearAll();", 3);
    }
}

function HitCGHUD::Update(%cl) {
    
    if(!$pref::HitCGHUD::Enabled)
        return;
    
    %myid = getManagerId();
    if(%cl != %myid) { return; }
   
    //when cg event occurs set visible
    Control::SetVisible("HitCGHUD::Marker100", true);
    schedule("HitCGHUD::GoInvisible(100);", 0.05);

}

function HitCGHUD::GoInvisible(%x)
{
    if (%x == 100) {
        Control::SetVisible("HitCGHUD::Marker100", false);
        Control::SetVisible("HitCGHUD::Marker75", true);
        schedule("HitCGHUD::GoInvisible(75);", 0.05);
    }
    else if (%x == 75) {
        Control::SetVisible("HitCGHUD::Marker75", false);
        Control::SetVisible("HitCGHUD::Marker50", true);
        schedule("HitCGHUD::GoInvisible(50);", 0.05);
    }
    else if (%x == 50) {
        Control::SetVisible("HitCGHUD::Marker50", false);
        Control::SetVisible("HitCGHUD::Marker25", true);
        schedule("HitCGHUD::GoInvisible(25);", 0.05);
        
    }
    else if (%x == 25) {
        Control::SetVisible("HitCGHUD::Marker25", false);
    }
    else { }
    
}

function HitCGHUD::ClearAll()
{
    
    Control::SetVisible("HitCGHUD::Marker100", false);
    Control::SetVisible("HitCGHUD::Marker75", false);
    Control::SetVisible("HitCGHUD::Marker50", false);
    Control::SetVisible("HitCGHUD::Marker25", false);

}


// suicides - clientsuicided
function HitCGHUD::clientSuicide( %v, %w ) 
{

    HitCGHUD::ClearAll();

}

// deaths and team kills - clientkilled - clientteamkilled
function HitCGHUD::clientKilled( %k, %v, %w )
{

    %myid = getManagerId();
    if (%v == %myid) { HitCGHUD::ClearAll(); }

}

HitCGHUD::Init();

Event::Attach( eventHitCG, HitCGHUD::Update );
Event::Attach( eventClientKilled, HitCGHUD::clientKilled );
Event::Attach( eventClientTeamKilled, HitCGHUD::clientKilled );
Event::Attach( eventClientSuicided, HitCGHUD::clientSuicide );

// ================================================================================
// xPrefs Support
// ================================================================================

function HitCGHUD::xSetup() after xPrefs::Setup {
    xPrefs::Create("HitCGHUD", "HitCGHUD::xInit");
}

function HitCGHUD::xInit() {
    xPrefs::addText("HitCGHUD::Header1", "HitCGHUD");
	xPrefs::addCheckbox("HitCGHUD::Checkbox", "Enabled", "$pref::HitCGHUD::Enabled");

    xPrefs::addText("HitCGHUD::Header2", "Image");
    for (%i = 1; true; %i++) {
        %file = "Modules/HitCGHUD/hitmark"~%i~"_100.png";

        if (File::FindFirst(%file) != "") {
            %x = %i-1;
            $HitCGHUD::Hitmark[%x] = %i;
        } else {
            break;
        }
    }

    xPrefs::addComboBox("HitCGHUD::Combo", "Hitmark", "", "HitCGHUD::ComboChange();", True, "$HitCGHUD::Hitmark");
    xPrefs::AddTextFormat("HitCGHUD::Image", "", 45, False, "", 20);

    FGCombo::setSelected("HitCGHUD::Combo", $pref::HitCGHUD::Hitmark - 1);
    HitCGHUD::ComboChange();

    xPrefs::addHelpSection("HitCGHUD", "HUD renders a hitmark image around the weapon reticle when a chaingun bullet hits an enemy target. Requires server side support.");

	xPrefs::addBindCommand("playMap.sae", "HitCGHUD Toggle", "HitCGHUD::Toggle();");
}

function HitCGHUD::ComboChange() {
    %selected = FGCombo::getSelectedText("HitCGHUD::Combo");
    $pref::HitCGHUD::Hitmark = %selected;

    %image = "<jc><B0,0:Modules/HitCGHUD/hitmark"~%selected~"_100.png>";
    Control::setValue("HitCGHUD::Image", %image);

    HitCGHUD::UpdateMarker();
}
