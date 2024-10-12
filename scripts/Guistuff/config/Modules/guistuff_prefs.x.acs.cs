// guistuff prefs settings
// Updated to support xPrefs


//Patch green lines
$pref::mj::greenlines = $pref::mj::greenlines != "" ? $pref::mj::greenlines : false;
$mj::greenlines = $pref::mj::greenlines;

//show hp bars
$pref::mj::showhpbars = $pref::mj::showhpbars != "" ? $pref::mj::showhpbars : true;
$mj::showhpbars = $pref::mj::showhpbars;

//show jet bars
$pref::mj::showjetbars = $pref::mj::showjetbars != "" ? $pref::mj::showjetbars : false;
$mj::showjetbars = $pref::mj::showjetbars;

//only show when crouching
$pref::mj::barscrouch = $pref::mj::barscrouch != "" ? $pref::mj::barscrouch : false;
$mj::barscrouch = $pref::mj::barscrouch;

//show pink iff when team mate says "I have flag"
$pref::mj::passhelper = $pref::mj::passhelper != "" ? $pref::mj::passhelper : true;
$mj::passhelper = $pref::mj::passhelper;

//show pink minimap iff when team mate says "i have flag"
$pref::mj::passhelpermm = $pref::mj::passhelpermm != "" ? $pref::mj::passhelpermm : true;
$mj::passhelpermm = $pref::mj::passhelpermm;

//show name
$pref::mj::shownames = $pref::mj::shownames != "" ? $pref::mj::shownames : true;
$mj::shownames = $pref::mj::shownames;

//show hp percentage next to name
$pref::mj::showhptext = $pref::mj::showhptext != "" ? $pref::mj::showhptext : false;
$mj::showhptext = $pref::mj::showhptext;

//font for name, hp
$pref::mj::fontdefault = $pref::mj::fontdefault != "" ? $pref::mj::fontdefault : "sf_white_10b.pft";
$mj::fontdefault = $pref::mj::fontdefault;

//flag pass related font
$pref::mj::fontpass = $pref::mj::fontpass != "" ? $pref::mj::fontpass : "if_g_10b.pft";
$mj::fontpass = $pref::mj::fontpass;

//bar layout prefs
$pref::mj::bar_width = $pref::mj::bar_width != "" ? $pref::mj::bar_width : 20;
$mj::bar_width = $pref::mj::bar_width;

$pref::mj::bar_height = $pref::mj::bar_height != "" ? $pref::mj::bar_height : 5;
$mj::bar_height = $pref::mj::bar_height;

$pref::mj::bar_border_width = $pref::mj::bar_border_width != "" ? $pref::mj::bar_border_width : 1;
$mj::bar_border_width = $pref::mj::bar_border_width;

// ================================================================================
// xPrefs Support
// ================================================================================

function GUIStuff::xSetup() after xPrefs::Setup {

    xPrefs::Create("GUI Stuff", "GUIStuff::xInit");

}

function GUIStuff::xInit() {

    xPrefs::addText("GUIStuff::Header1", "GUI Stuff Options");
    xPrefs::addCheckbox("GUIStuff::Checkbox1", "Patch Green Lines", "$pref::mj::greenlines", "$mj::greenlines = $pref::mj::greenlines;");
    xPrefs::addCheckbox("GUIStuff::Checkbox2", "Show HP Bars", "$pref::mj::showhpbars", "$mj::showhpbars = $pref::mj::showhpbars;");
    xPrefs::addCheckbox("GUIStuff::Checkbox3", "Show Jet Bars", "$pref::mj::showjetbars", "$mj::showjetbars = $pref::mj::showjetbars;");
    xPrefs::addCheckbox("GUIStuff::Checkbox4", "Show Only When Crouching", "$pref::mj::barscrouch", "$mj::barscrouch = $pref::mj::barscrouch;");
    xPrefs::addCheckbox("GUIStuff::Checkbox5", "Pass Helper IFF", "$pref::mj::passhelper", "$mj::passhelper = $pref::mj::passhelper;");
    xPrefs::addCheckbox("GUIStuff::Checkbox6", "Pass Helper Minimap", "$pref::mj::passhelpermm", "$mj::passhelpermm = $pref::mj::passhelpermm;");
    xPrefs::addCheckbox("GUIStuff::Checkbox7", "Show Names", "$pref::mj::shownames", "$mj::shownames = $pref::mj::shownames;");
    xPrefs::addCheckbox("GUIStuff::Checkbox8", "Show HP Percentage", "$pref::mj::showhptext", "$mj::showhptext = $pref::mj::showhptext;");

    xPrefs::addText("GUIStuff::Header3", "Bar Layout");
    xPrefs::addTextEdit("GUIStuff::TextEdit3", "Bar Width", "$pref::mj::bar_width", "True", "10", "True", "$mj::bar_width = $pref::mj::bar_width;");
    xPrefs::addTextEdit("GUIStuff::TextEdit4", "Bar Height", "$pref::mj::bar_height", "True", "10", "True", "$mj::bar_height = $pref::mj::bar_height;");
    xPrefs::addTextEdit("GUIStuff::TextEdit5", "Bar Border Width", "$mj::bar_border_width", "True", "10", "True", "$mj::bar_border_width = $pref::mj::bar_border_width;");

    xPrefs::addText("GUIStuff::Header2", "Fonts");
    xPrefs::addTextEdit("GUIStuff::TextEdit1", "IFF Font", "$pref::mj::fontdefault", "False", "255", "True", "$mj::fontdefault = $pref::mj::fontdefault;");
    xPrefs::addTextEdit("GUIStuff::TextEdit2", "Flag Pass Font", "$pref::mj::fontpass", "False", "255", "True", "$mj::fontpass = $pref::mj::fontpass;");

}
