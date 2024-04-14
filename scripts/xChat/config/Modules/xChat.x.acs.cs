// xChat.dll Config
// Updated to support xPrefs
// Configure the preferences in the Options->Scripts menu in-game
// Restart Tribes for changes to take effect

//=======================================================================================================
// Configuration variables
//=======================================================================================================

$pref::xChat::HiderEnabled = $pref::xChat::HiderEnabled == "" ? "True" : $pref::xChat::HiderEnabled; // (Default: True; Will display messages for a specific period of time)
$pref::xChat::HiderTimeout = $pref::xChat::HiderTimeout == "" ? "10" : $pref::xChat::HiderTimeout; // (Default: 10; Seconds to display each message)
$pref::xChat::ScrollTimeout = $pref::xChat::ScrollTimeout == "" ? "5" : $pref::xChat::ScrollTimeout; // (Default: 5; Seconds that hider turns off when you scroll message history with PgUp/PgDn)
$pref::xChat::ChatOnly = $pref::xChat::ChatOnly == "" ? "False" : $pref::xChat::ChatOnly; // (Default: False; Only display Global or Team chat messages. Scroll (PgUp/PgDn) to see all messages)
$pref::xChat::HideCmdMsg = $pref::xChat::HideCmdMsg == "" ? "True" : $pref::xChat::HideCmdMsg; // (Default: True; Hide command messages at bottom of Chat Hud in playGui)
$pref::xChat::TransChat = $pref::xChat::TransChat == "" ? "True" : $pref::xChat::TransChat; // (Default: True; Make the Chat Hud transparent)
$pref::xChat::TransInput = $pref::xChat::TransInput == "" ? "True" : $pref::xChat::TransInput; // (Default: True; Make the Chat Input box transparent)
$pref::xChat::IconsEnabled = $pref::xChat::IconsEnabled == "" ? "False" : $pref::xChat::IconsEnabled; // (Default: False; Display custom icons next to each message based on message type)

$xChat::HiderEnabled = $pref::xChat::HiderEnabled;
$xChat::HiderTimeout = $pref::xChat::HiderTimeout;
$xChat::ScrollTimeout = $pref::xChat::ScrollTimeout;
$xChat::ChatOnly = $pref::xChat::ChatOnly;
$xChat::HideCmdMsg = $pref::xChat::HideCmdMsg;
$xChat::TransChat = $pref::xChat::TransChat;
$xChat::TransInput = $pref::xChat::TransInput;
$xChat::IconsEnabled = $pref::xChat::IconsEnabled;

//=======================================================================================================
// Custom Fonts
//
// Place custom fonts in the [Tribes/base/fonts] folder and use the .pft filenames
//=======================================================================================================

// Default Chat Fonts (msgType: 0-4)

$pref::xChat::SystemFont = $pref::xChat::SystemFont == "" ? "if_w_10b.pft" : $pref::xChat::SystemFont;         // MsgType: 0   Default: if_w_10b.pft (White)
$pref::xChat::GameFont = $pref::xChat::GameFont == "" ? "sf_red_10b.pft" : $pref::xChat::GameFont;             // MsgType: 1   Default: sf_red_10b.pft (Red)
$pref::xChat::MsgFont = $pref::xChat::MsgFont == "" ? "sf_yellow_10b.pft" : $pref::xChat::MsgFont;          // MsgType: 2   Default: sf_yellow_10b.pft (Yellow)
$pref::xChat::TeamMsgFont = $pref::xChat::TeamMsgFont == "" ? "if_g_10b.pft" : $pref::xChat::TeamMsgFont;   // MsgType: 3   Default: if_g_10b.pft (Green)
$pref::xChat::CommandFont = $pref::xChat::CommandFont == "" ? "sf_red_10b.pft" : $pref::xChat::CommandFont; // MsgType: 4   Default: sf_red_10b.pft (Red)
$pref::xChat::VChatFont = $pref::xChat::VChatFont == "" ? "if_g_10b.pft" : $pref::xChat::VChatFont;         // Default: if_g_10b.pft (Green)

$xChat::SystemFont = $pref::xChat::SystemFont;
$xChat::GameFont = $pref::xChat::GameFont;
$xChat::MsgFont = $pref::xChat::MsgFont;
$xChat::TeamMsgFont = $pref::xChat::TeamMsgFont;
$xChat::CommandFont = $pref::xChat::CommandFont;
$xChat::VChatFont = $pref::xChat::VChatFont;

//=======================================================================================================
// Chat Icons
//
// Place custom chat icons in [Tribes/base/huds] folder and use the .png filenames
//
// Enable these by changing IconsEnabled preference
//=======================================================================================================

$pref::xChat::SystemFont::Icon = $pref::xChat::SystemFont::Icon == "" ? "chat_white.png" : $pref::xChat::SystemFont::Icon;
$pref::xChat::GameFont::Icon = $pref::xChat::GameFont::Icon == "" ? "chat_red.png" : $pref::xChat::GameFont::Icon;
$pref::xChat::MsgFont::Icon = $pref::xChat::MsgFont::Icon == "" ? "chat_yellow.png" : $pref::xChat::MsgFont::Icon;
$pref::xChat::TeamMsgFont::Icon = $pref::xChat::TeamMsgFont::Icon == "" ? "chat_green.png" : $pref::xChat::TeamMsgFont::Icon;
$pref::xChat::CommandFont::Icon = $pref::xChat::CommandFont::Icon == "" ? "chat_red.png" : $pref::xChat::CommandFont::Icon;

$xChat::SystemFont::Icon = $pref::xChat::SystemFont::Icon;
$xChat::GameFont::Icon = $pref::xChat::GameFont::Icon;
$xChat::MsgFont::Icon = $pref::xChat::MsgFont::Icon;
$xChat::TeamMsgFont::Icon = $pref::xChat::TeamMsgFont::Icon;
$xChat::CommandFont::Icon = $pref::xChat::CommandFont::Icon;

$pref::xChat::Icon::xOffset = $pref::xChat::Icon::xOffset == "" ? "8" : $pref::xChat::Icon::xOffset; // (Default: 8; x offset from left of message text)
$pref::xChat::Icon::yOffset = $pref::xChat::Icon::yOffset == "" ? "3" : $pref::xChat::Icon::yOffset; // (Default: 3; y offset from top of message text)

$xChat::Icon::yOffset = $pref::xChat::Icon::yOffset;
$xChat::Icon::xOffset = $pref::xChat::Icon::xOffset;

// ================================================================================
// xPrefs Support
// ================================================================================

function xChat::xSetup() after xPrefs::Setup {
    xPrefs::Create("xChat", "xChat::xInit");
}

function xChat::xInit() {

    xPrefs::AddTextFormat("xChat::Warning", "<jc><f2>Restart Tribes to take effect", "", False, "", 4);

    xPrefs::addText("xChat::Header1", "xChat Options");
    xPrefs::addCheckbox("xChat::Checkbox1", "Hider Enabled", "$pref::xChat::HiderEnabled");
    xPrefs::addCheckbox("xChat::Checkbox2", "Chat Only", "$pref::xChat::ChatOnly");
    xPrefs::addCheckbox("xChat::Checkbox3", "Hide Cmd Msg", "$pref::xChat::HideCmdMsg");
    xPrefs::addCheckbox("xChat::Checkbox4", "Trans Chat", "$pref::xChat::TransChat");
    xPrefs::addCheckbox("xChat::Checkbox5", "Trans Input", "$pref::xChat::TransInput");
    xPrefs::addCheckbox("xChat::Checkbox6", "Icons Enabled", "$pref::xChat::IconsEnabled");

    xPrefs::addText("xChat::Header2", "Timeouts (seconds)");
    xPrefs::addTextEdit("xChat::TextEdit1", "Hider Timeout", "$pref::xChat::HiderTimeout", "True", "10");
    xPrefs::addTextEdit("xChat::TextEdit2", "Scroll Timeout", "$pref::xChat::ScrollTimeout", "True", "10");

    xPrefs::addText("xChat::Header3", "Fonts");
    xPrefs::addTextEdit("xChat::TextEdit3", "System Font", "$pref::xChat::SystemFont", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit4", "Game Font", "$pref::xChat::GameFont", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit5", "Msg Font", "$pref::xChat::MsgFont", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit6", "Team Msg Font", "$pref::xChat::TeamMsgFont", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit7", "Command Font", "$pref::xChat::CommandFont", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit8", "VChat Font", "$pref::xChat::VChatFont", "False", "255");

    xPrefs::addText("xChat::Header4", "Icons");
    xPrefs::addTextEdit("xChat::TextEdit9", "System Icon", "$pref::xChat::SystemFont::Icon", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit10", "Game Icon", "$pref::xChat::GameFont::Icon", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit11", "Msg Icon", "$pref::xChat::MsgFont::Icon", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit12", "Team Msg Icon", "$pref::xChat::TeamMsgFont::Icon", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit13", "Command Icon", "$pref::xChat::CommandFont::Icon", "False", "255");
    xPrefs::addTextEdit("xChat::TextEdit14", "Icon X Offset", "$pref::xChat::Icon::xOffset", "True", "10");
    xPrefs::addTextEdit("xChat::TextEdit15", "Icon Y Offset", "$pref::xChat::Icon::yOffset", "True", "10");

    xPrefs::addHelpSection("xChat", "Plugin that enhances the Chat HUD with a chat hider, custom fonts/icons, and additional functions.");
    xPrefs::addHelpSection("Hider Enabled", "Will display messages for a specific period of time.");
    xPrefs::addHelpSection("Chat Only", "Only display Global or Team chat messages. Scroll (PgUp/PgDn) to see all messages.");
    xPrefs::addHelpSection("Hide Cmd Msg", "Hide command messages at bottom of Chat Hud in playGui.");
    xPrefs::addHelpSection("Trans Chat", "Make the Chat Hud transparent.");
    xPrefs::addHelpSection("Trans Input", "Make the Chat Input box transparent.");
    xPrefs::addHelpSection("Icons Enabled", "Display custom icons next to each message based on message type.");
    xPrefs::addHelpSection("Hider Timeout", "Seconds to display each message.");
    xPrefs::addHelpSection("Scroll Timeout", "Seconds that hider turns off when you scroll message history with PgUp/PgDn.");
    xPrefs::addHelpSection("Fonts", "Place custom fonts in the [Tribes/base/fonts] folder and use the .pft filenames.");
    xPrefs::addHelpSection("Icons", "Place custom chat icons in [Tribes/base/huds] folder and use the .png filenames.");
    xPrefs::addHelpSection("Icon X Offset", "X offset from left of message text.");
    xPrefs::addHelpSection("Icon Y Offset", "Y offset from top of message text.");

}
