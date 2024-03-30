// xPrefs
// Script preference system for Tribes 1.40/1.41
// Install to config/Core
// By Smokey

$xPrefs::Version = "0.17";

// TODO: Test all error messages
// TODO: Use String::escapeFormatting on inputs
// TODO: Remove default instant data
// TODO: Add script name to user function error messages
// TODO: String length for text inputs
// TODO: Fix tabs/spaces

// =====================================================================================================
// Public Functions
// =====================================================================================================

/**
 * xPrefs::Create(scriptName, initFunc)
 *
 * Creates a new xPref script instance.
 *
 * Params:
 * scriptName - Unique name of the script that will appear in the script ComboBox.
 * initFunc - Initialization function that will be called for the script to configure itself.
 * 
 * Returns: None
 * 
 * Remarks:
 * This function should only be called in a setup function attached to xPref::Setup.
 * 
 * Example:
 * function CTFHud::xSetup() after xPrefs::Setup {
 *   xPrefs::Create("CTFHud", "CTFHud::xInit");
 * }
 * 
 */
function xPrefs::Create(%scriptName, %initFunc) {

    if ($xPrefs::Init != true) {
        echoc(1, "xPrefs is not initialized.");
        return;
    }

	if (%scriptName == "" || %scriptName == "None") {
		echoc(1, "xPrefs: scriptName cannot be blank or None.");
		return;
	}

	if ($xPrefs::Script[%scriptName] == true) {
		echoc(1, "xPrefs: Script [" ~ %scriptName ~ "] is already created.");
		return;
	}

    if (%initFunc == "" || !isFunction(%initFunc)) {
        echoc(1, "xPrefs: Function [" ~ %initFunc ~ "] is not valid.");
        return;
    }

    %panelName = "ScriptOptionsPanel_" ~ String::Replace(%scriptName, " ", "_");

    instant FearGui::FGControl %panelName {
        position = "0 0";
        extent = "316 327"; // Update y in _addObjectToPanel if changed
        horizSizing = "right";
        vertSizing = "bottom";
        consoleVariable = "";
        consoleCommand = "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "1 0 0";
        border = "False";
        borderColor = "0 0 0";
        selectBorderColor = "0.772549 0.796078 0.788235";
        ghostBorderColor = "0 0 0";
        visible = "True";
        tag = "";
    };

	$xPrefs::Script[%scriptName] = true;
	$xPrefs::Script[$xPrefs::scriptCount] = %scriptName;
	$xPrefs::scriptCount++;
	
	$xPrefs::Script[%scriptName, lineOffset] = 0;
	$xPrefs::Script[%scriptName, bindCount] = 0;
    $xPrefs::Script[%scriptName, objCount] = 0;
    $xPrefs::Script[%scriptName, panelName] = %panelName;
    $xPrefs::Script[%scriptName, helpText] = "";
	
    $xPrefs::scriptName = %scriptName;
	*%initFunc();
    $xPrefs::scriptName = "";

}

/**
 * xPrefs::addText(objName, text, [active], [offsetX], [preOffsetY], [postOffsetY])
 *
 * Add a text header to the script options page
 *
 * Params:
 * objName - Unique name of the object
 * text - Text to be displayed
 * [active] - True/False. Initialize the object as active. Optional parameter (Default: True).
 * [offsetX] - X-axis offset. Optional parameter (Default: 6).
 * [preOffsetY] - Y-axis offset before the object. Optional parameter (Default: 6).
 * [postOffsetY] - Y-axis offset after the object. Optional parameter (Default: 6).
 *
 * Returns: None
 * 
 * Remarks:
 * This function can only be run inside the initFunc provided to xPrefs::Create
 *
 * Example:
 * xPrefs::addText("MyHUD::Header1", "Header 1");
 * 
 */
function xPrefs::addText(%objName, %text, %active, %offsetX, %preOffsetY, %postOffsetY) {

    %scriptName = $xPrefs::scriptName;
    if (%scriptName == "") {
        echoc(1, "xPrefs: Function can only be called in initFunc.");
        return;
    }

	if (Control::getId(%objName) != -1) {
		echoc(1, "xPref: Object name [" ~ %objName ~ "] already exists.");
		return;
	}

    if (String::FindSubStr(%objName, " ") != -1) {
        echoc(1, "xPrefs: Object name cannot contain spaces.");
		return;
    }

    if (%text == "") {
        echoc(1, "xPrefs: Text cannot be blank.");
		return;
    }

    %offsetX = %offsetX == "" ? 6 : round(%offsetX, 0);
    %preOffsetY = %preOffsetY == "" ? 6 : round(%preOffsetY, 0);
    %postOffsetY = %postOffsetY == "" ? 6 : round(%postOffsetY, 0);

    %y = xPrefs:_lineOffset(%scriptName, %preOffsetY, %postOffsetY);

	instant FearGui::FGSimpleText %objName {
		position = %offsetX ~ " " ~ %y;
		extent = "345 " ~ $pref::xPrefs::lineHeight;
		horizSizing = "right";
		vertSizing = "bottom";
		consoleVariable = "";
		consoleCommand = "";
		altConsoleCommand = "";
		deleteOnLoseContent = "True";
		ownObjects = "True";
		opaque = "False";
		fillColor = "0 0 0";
		selectFillColor = "0 0 0";
		ghostFillColor = "0.819608 0.945098 0.952941";
		border = "False";
		borderColor = "0 0 0";
		selectBorderColor = "0.745098 0.811765 0.870588";
		ghostBorderColor = "0 0 0";
		visible = "True";
		tag = "";
		active = %active != "" ? %active : "True";
		messageTag = "";
		fontNameTag = "IDFNT_10_HILITE";
		fontNameTagHL = "IDFNT_10_SELECTED";
		fontNameTagDisabled = "IDFNT_10_DISABLED";
		textTag = "";
		text = %text;
		align = "left";
		textVPosDelta = "0";
	};

    xPrefs::_addObjectToPanel(%scriptName, %objName);

}

/**
 * xPrefs::AddTextFormat(objName, text, [height], [border], [offsetX], [preOffsetY], [postOffsetY])
 *
 * Add a formatted text object to the script options page
 *
 * Params:
 * objName - Unique name of the object
 * text - Text to be displayed
 * [height] - Height of the object. Optional parameter (Default: 17).
 * [border] - True/False. Render a border around the object. Optional parameter (Default: False).
 * [offsetX] - X-axis offset. Optional parameter (Default: 18).
 * [preOffsetY] - Y-axis offset before the object. Optional parameter (Default: 0).
 * [postOffsetY] - Y-axis offset after the object. Optional parameter (Default: 2).
 *
 * Returns: None
 *
 * Remarks:
 * This function can only be run inside the initFunc provided to xPrefs::Create
 *
 * Example:
 * xPrefs::AddTextFormat("MyHUD::TextFormat1", "<f2>Text Format");
 *
 */
function xPrefs::AddTextFormat(%objName, %text, %height, %border, %offsetX, %preOffsetY, %postOffsetY) {

    %scriptName = $xPrefs::scriptName;
    if (%scriptName == "") {
        echoc(1, "xPrefs: Function can only be called in initFunc.");
        return;
    }

	if (Control::getId(%objName) != -1) {
		echoc(1, "xPref: Object name [" ~ %objName ~ "] already exists.");
		return;
	}

    if (String::FindSubStr(%objName, " ") != -1) {
        echoc(1, "xPrefs: Object name cannot contain spaces.");
		return;
    }

    %height = %height == "" ? $pref::xPrefs::lineHeight : round(%height, 0);
    %border = %border == "" ? False : %border;
    %offsetX = %offsetX == "" ? 18 : round(%offsetX, 0);
    %preOffsetY = %preOffsetY == "" ? 0 : round(%preOffsetY, 0);
    %postOffsetY = %postOffsetY == "" ? 2 : round(%postOffsetY, 0);

    %y = xPrefs:_lineOffset(%scriptName, %preOffsetY, %postOffsetY);

    %borderName = %objName ~ "_Border";
    instant FearGui::FGControl %borderName {
        position = %offsetX ~ " " ~ %y;
        extent = "275 " ~ %height;
        horizSizing = "width";
        vertSizing = "height";
        consoleVariable = "";
        consoleCommand = "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "0.745098 0.811765 0.870588";
        border = %border;
        borderColor = "0.172549 0.6 0.070588";
        selectBorderColor = "0 0 0";
        ghostBorderColor = "0.333333 0.333333 0.333333";
        visible = "True";
        tag = "";
        instant FearGuiFormattedText %objName {
            position = %border ? "2 4" : "0 0";
            extent = "275 " ~ (%height - 1);
            horizSizing = "width";
            vertSizing = "height";
            consoleVariable = "";
            consoleCommand = "";
            altConsoleCommand = "";
            deleteOnLoseContent = "True";
            ownObjects = "True";
            opaque = "False";
            fillColor = "0 0 0";
            selectFillColor = "0 0 0";
            ghostFillColor = "0 0 0";
            border = "True";
            borderColor = "0 1 0";
            selectBorderColor = "0 0 0";
            ghostBorderColor = "0 0 0";
            visible = "True";
            tag = "";
            active = "False";
            messageTag = "";
        };
    };

    Control::SetValue(%objName, %text);

    xPrefs::_addObjectToPanel(%scriptName, %borderName);

}

/**
 * xPrefs::addCheckbox(objName, text, [variable], [command], [altCommand])
 *
 * Add a checkbox to the script options page
 *
 * Params:
 * objName - Unique name of the object
 * text - Text to be displayed
 * [variable] - Mirror the value of the object to the console variable. Optional parameter.
 * [command] - Console command to execute when the checkbox is clicked. Optional parameter.
 * [altCommand] - Console command to execute when the checkbox is unset. Optional parameter. (If defined, [command] is only executed when checkbox is set)
 *
 * Returns: None
 * 
 * Remarks:
 * This function can only be run inside the initFunc provided to xPrefs::Create
 * 
 * Example:
 * xPrefs::addCheckbox("MyHUD::Checkbox1", "Checkbox 1", "$MyHUD::Pref::Checkbox1");
 * xPrefs::addCheckbox("MyHUD::Checkbox2", "Checkbox 2", "$MyHUD::Pref::Checkbox2", "echoc(1, \"Checkbox2 Clicked!\");");
 * 
 */
function xPrefs::addCheckbox(%objName, %text, %variable, %command, %altCommand) {

	%scriptName = $xPrefs::scriptName;
    if (%scriptName == "") {
        echoc(1, "xPrefs: Function can only be called in initFunc.");
        return;
    }

	if (Control::getId(%objName) != -1) {
		echoc(1, "xPref: Object name [" ~ %objName ~ "] already exists.");
		return;
	}

    if (String::FindSubStr(%objName, " ") != -1) {
        echoc(1, "xPrefs: Object name cannot contain spaces.");
		return;
    }

	if (%text == "") {
        echoc(1, "xPrefs: Text cannot be blank.");
		return;
    }

    if (String::len(%command) > 80 || String::len(%altCommand) > 80) {
        echoc(1, "xPrefs: Command string length cannot be greater than 80.");
		return;
    }

	%y = xPrefs:_lineOffset(%scriptName, 0, 2);
    %textName = %objName ~ "_Text";

    instant FearGui::FGUniversalButton %objName {
        position = "18 " ~ (%y + 2);
        extent = "18 " ~ ($pref::xPrefs::lineHeight - 2);
        horizSizing = "right";
        vertSizing = "bottom";
        consoleVariable = %variable;
        consoleCommand = %command != "" ? %command : "";
        altConsoleCommand = %altCommand != "" ? %altCommand : (%command != "" ? %command : "");
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "0.772549 0.796078 0.788235";
        border = "False";
        borderColor = "0 0 0";
        selectBorderColor = "0.772549 0.796078 0.788235";
        ghostBorderColor = "0 0 0";
        visible = "True";
        tag = "";
        active = "True";
        messageTag = "";
        isCheckbox = "True";
        radioTag = "";
        bitmapRoot = "BTN_CheckBox";
        isSet = "False";
        mirrorConsoleVar = %variable != "" ? "True" : "False";
    };
    instant FearGui::FGSimpleText %textName {
        position = "44 " ~ %y;
        extent = "141 " ~ $pref::xPrefs::lineHeight;
        horizSizing = "right";
        vertSizing = "bottom";
        consoleVariable = "";
        consoleCommand = "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "0.819608 0.945098 0.952941";
        border = "False";
        borderColor = "0 0 0";
        selectBorderColor = "0.745098 0.811765 0.870588";
        ghostBorderColor = "0 0 0";
        visible = "True";
        tag = "";
        active = "False";
        messageTag = "";
        fontNameTag = "IDFNT_10_HILITE";
        fontNameTagHL = "IDFNT_10_SELECTED";
        fontNameTagDisabled = "IDFNT_10_DISABLED";
        textTag = "";
        text = %text;
        align = "left";
        textVPosDelta = "0";
    };

    xPrefs::_addObjectToPanel(%scriptName, %objName);
    xPrefs::_addObjectToPanel(%scriptName, %textName);

}

/**
 * xPrefs::addTextEdit(objName, text, [variable], [numbersOnly], [maxStrLen], [active], [command])
 *
 * Add a text edit object to the script options page
 *
 * Params:
 * objName - Unique name of the object
 * text - Text to be displayed
 * [variable] - Mirror the value of the object to the console variable. Optional parameter.
 * [numbersOnly] - True/False. Control only accepts numerical values. Optional parameter (Default: False).
 * [maxStrLen] - Numerical value of the maximum string length the object will accept. Optional parameter (Default: 255).
 * [active] - True/False. Initialize the object as active. Optional parameter (Default: True).
 * [command] - Console command to execute when the textedit is changed. Optional parameter.
 *
 * Returns: None
 * 
 * Remarks:
 * This function can only be run inside the initFunc provided to xPrefs::Create
 * 
 * Example:
 * xPrefs::addTextEdit("MyHUD::TextEdit1", "Text Edit", "$MyHUD::Pref::TextEdit1");
 * xPrefs::addTextEdit("MyHUD::TextEdit2", "Numeric Text Edit", "$MyHUD::Pref::TextEdit2", "True", "10");
 * 
 */
function xPrefs::addTextEdit(%objName, %text, %variable, %numbersOnly, %maxStrLen, %active, %command) {

	%scriptName = $xPrefs::scriptName;
    if (%scriptName == "") {
        echoc(1, "xPrefs: Function can only be called in initFunc.");
        return;
    }

	if (Control::getId(%objName) != -1) {
		echoc(1, "xPref: Object name [" ~ %objName ~ "] already exists.");
		return;
	}

    if (String::FindSubStr(%objName, " ") != -1) {
        echoc(1, "xPrefs: Object name cannot contain spaces.");
		return;
    }

	if (%text == "") {
        echoc(1, "xPrefs: Text cannot be blank.");
		return;
    }

    if (String::len(%command) > 80) {
        echoc(1, "xPrefs: Command string length cannot be greater than 80.");
		return;
    }

	%y = xPrefs:_lineOffset(%scriptName, 0, 2);
    %textName = %objName ~ "_Text";

    instant FearGui::FGSimpleText %textName {
        position = "18 " ~ (%y + 3);
        extent = "41 " ~ $pref::xPrefs::lineHeight;
        horizSizing = "right";
        vertSizing = "bottom";
        consoleVariable = "";
        consoleCommand = "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "0.819608 0.945098 0.952941";
        border = "False";
        borderColor = "0 0 0";
        selectBorderColor = "0.745098 0.811765 0.870588";
        ghostBorderColor = "0 0 0";
        visible = "True";
        tag = "";
        active = "False";
        messageTag = "";
        fontNameTag = "IDFNT_10_HILITE";
        fontNameTagHL = "IDFNT_10_SELECTED";
        fontNameTagDisabled = "IDFNT_10_DISABLED";
        textTag = "";
        text = %text;
        align = "left";
        textVPosDelta = "0";
    };
    instant FearGui::TestEdit %objName {
        position = "177 " ~ %y;
        extent = "120 " ~ $pref::xPrefs::lineHeight;
        horizSizing = "right";
        vertSizing = "bottom";
        consoleVariable = %variable;
        consoleCommand = %command != "" ? %command : "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "0 0 0";
        border = "False";
        borderColor = "0 0 0";
        selectBorderColor = "0 0 0";
        ghostBorderColor = "0 0 0";
        visible = "True";
        tag = "";
        active = %active != "" ? %active : "True";
        messageTag = "";
        fontNameTag = "IDFNT_10_STANDARD";
        fontNameTagHL = "IDFNT_10_SELECTED";
        fontNameTagDisabled = "IDFNT_10_DISABLED";
        textTag = "";
        text = "";
        align = "left";
        textVPosDelta = "0";
        numbersOnly = %numbersOnly != "" ? %numbersOnly : "False";
        maxStrLen = %maxStrLen != "" ? %maxStrLen : "255";
    };

    xPrefs::_addObjectToPanel(%scriptName, %objName);
    xPrefs::_addObjectToPanel(%scriptName, %textName);

}

/**
 * xPrefs::addComboBox(objName, [title], [variable], [command], [active], [values])
 *
 * Add a combobox object to the script options page
 *
 * Params:
 * objName - Unique name of the object
 * [title] - Title to be displayed in the combobox. Optional parameter.
 * [variable] - Mirror the value of the object to the console variable. Optional parameter.
 * [command] - Console command to execute when the combobox is changed. Optional parameter.
 * [active] - True/False. Initialize the object as active. Optional parameter (Default: True).
 * [values] - Variable array of values to populate into the combobox. Optional parameter. For example, an array of $MyHud::Item[0], $MyHud::Item[1], etc, should use value "$MyHud::Item" for this parameter.
 *
 * Returns: None
 * 
 * Remarks:
 * This function can only be run inside the initFunc provided to xPrefs::Create
 * 
 * Example:
 * xPrefs::addComboBox("MyHUD::Combo1", "Combo 1", "$MyHUD::Pref::Combo1", "MyHUD::Combo1::onAction();");
 * 
 */
function xPrefs::addComboBox(%objName, %title, %variable, %command, %active, %values) {

	%scriptName = $xPrefs::scriptName;
    if (%scriptName == "") {
        echoc(1, "xPrefs: Function can only be called in initFunc.");
        return;
    }

	if (Control::getId(%objName) != -1) {
		echoc(1, "xPref: Object name [" ~ %objName ~ "] already exists.");
		return;
	}

    if (String::FindSubStr(%objName, " ") != -1) {
        echoc(1, "xPrefs: Object name cannot contain spaces.");
		return;
    }

    if (String::len(%command) > 80) {
        echoc(1, "xPrefs: Command string length cannot be greater than 80.");
		return;
    }

    if (%values != "") {
        if (!String::Starts(%values, "$")) {
            echoc(1, "xPrefs: Values must be a variable that begins with $.");
            return;
        }

        %element = %values ~ "0";
        %val = *%element;
        if (%val == "") {
            echoc(1, "xPrefs: Values array must have at least one element.");
            return;
        }
    }

	%y = xPrefs:_lineOffset(%scriptName, 0, 2);

    instant FearGui::FGStandardComboBox %objName {
        position = "32 " ~ %y;
        extent = "265 " ~ $pref::xPrefs::lineHeight;
        horizSizing = "right";
        vertSizing = "bottom";
        consoleVariable = %variable != "" ? %variable : "";
        consoleCommand = %command != "" ? %command : "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "0 0 0";
        border = "False";
        borderColor = "0 0 0";
        selectBorderColor = "0 0 0";
        ghostBorderColor = "0 0 0";
        visible = "True";
        tag = "";
        active = %active != "" ? %active : "True";
        messageTag = "";
        fontNameTag = "IDFNT_10_STANDARD";
        fontNameTagHL = "IDFNT_10_HILITE";
        fontNameTagDisabled = "IDFNT_10_DISABLED";
        textTag = "";
        text = "";
        align = "left";
        textVPosDelta = "0";
        comboTitle = %title;
    };

    xPrefs::_addObjectToPanel(%scriptName, %objName);

    if (%values != "") {
        for (%i = 0; true; %i++) {
            %element = %values ~ %i;
            %val = *%element;

            if (%val != "") {
                FGCombo::addEntry(%objName, %val, %i);
            } else {
                break;
            }
        }

        if (%variable != "") {
            FGCombo::setSelected(%objName, FGCombo::FindEntry(%objName, *%variable));
        }
    }
}

/**
 * xPrefs::addBindCommand(map, desc, make, [break])
 *
 * Add a bind to the scripts binds page
 *
 * Params:
 * map - ActionMap name to apply the bind.
 * desc - Description of the bind.
 * make - Command to execute when the bind is depressed.
 * [break] - Command to execute when the bind is released. Optional parameter.
 *
 * Returns: None
 * 
 * Remarks:
 * This function can only be run inside the initFunc provided to xPrefs::Create
 * 
 * Example:
 * xPrefs::addBindCommand("actionMap.sae", "Bind Command 1", "MyHUD::Bind1();");
 * xPrefs::addBindCommand("playMap.sae", "Bind Command 2", "MyHUD::Bind2::On();", "MyHUD::Bind2::Off();");
 * 
 */
function xPrefs::addBindCommand(%map, %desc, %make, %break) {

    %scriptName = $xPrefs::scriptName;
    if (%scriptName == "") {
        echoc(1, "xPrefs: Function can only be called in initFunc.");
        return;
    }

    if (%desc == "") {
        echoc(1, "xPrefs: desc cannot be blank.");
        return;
    }

    if (%make == "") {
        echoc(1, "xPrefs: make cannot be blank.");
        return;
    }

    // TODO: Check if %map is a valid ActionMap
    // $ActionMaps[0] = "actionMap.sae";
    // $ActionMaps::Count = "5";

    %bindCount = $xPrefs::Script[%scriptName, bindCount];

    $xPrefs::Script[%scriptName, bind, %bindCount, map] = %map;
    $xPrefs::Script[%scriptName, bind, %bindCount, desc] = %desc;
    $xPrefs::Script[%scriptName, bind, %bindCount, make] = %make;
    $xPrefs::Script[%scriptName, bind, %bindCount, break] = %break;

    $xPrefs::Script[%scriptName, bindCount]++;
}

/**
 * xPrefs::addHelpSection(title, text)
 *
 * Add a section to the scripts Help tab
 *
 * Params:
 * title - Title of the help section
 * text - Text to be displayed
 *
 * Returns: None
 *
 * Remarks:
 * This function can only be run inside the initFunc provided to xPrefs::Create
 *
 * Example:
 * xPrefs::addHelpSection("Setting 1", "Setting 1 is for enabling or disabling the script.");
 *
 */
function xPrefs::addHelpSection(%title, %text) {

    %scriptName = $xPrefs::scriptName;
    if (%scriptName == "") {
        echoc(1, "xPrefs: Function can only be called in initFunc.");
        return;
    }

    if (%title == "") {
        echoc(1, "xPrefs: Title cannot be blank.");
		return;
    }

    if (%text == "") {
        echoc(1, "xPrefs: Text cannot be blank.");
		return;
    }

    $xPrefs::Script[%scriptName, helpText] = $xPrefs::Script[%scriptName, helpText] ~ "<f2>" ~ %title ~ "\n<f1>" ~ %text ~ "\n\n";

}

/**
 * xPrefs::Active([targetObj], [sourceObj])
 *
 * Helper function that copies the sourceObj active status to the targetObj
 *
 * Params:
 * targetObj - Object to set the active status.
 * sourceObj - Object from which to copy the active status.
 *
 * Returns: None
 *
 * Remarks:
 * This helper function is primarily to be used in %command parameters for the checkbox control
 * The %command parameter is limited to 80 characters so this shortens the logic
 *
 */
function xPrefs::Active(%targetObj, %sourceObj) {
    Control::setActive(%targetObj, Control::getValue(%sourceObj));
}

/**
 * xPrefs::Deactive([targetObj], [sourceObj])
 *
 * Helper function that copies the opposite of the sourceObj active status to the targetObj
 *
 * Params:
 * targetObj - Object to set the active status.
 * sourceObj - Object from which to copy the opposite of the active status.
 *
 * Returns: None
 *
 * Remarks:
 * This helper function is primarily to be used in %command parameters for the checkbox control
 * The %command parameter is limited to 80 characters so this shortens the logic
 *
 */
function xPrefs::Deactive(%targetObj, %sourceObj) {
    Control::setActive(%targetObj, !Control::getValue(%sourceObj));
}

// =====================================================================================================
// Private Functions
// =====================================================================================================

function xPrefs::Setup() {
    return true;
}

function xPrefs::_Init() after OptionsGui::onOpen {

    if ($xPrefs::Init)
        return;

    $xPrefs::scriptCount = 0;

    IDCTG_MENU_PAGE_20          = 00140682, "";
    IDCTG_MENU_PAGE_21          = 00140683, "";
    IDCTG_MENU_PAGE_22          = 00140684, "";
    IDCTG_MENU_PAGE_23          = 00140685, "";
    IDSTR_200                   = 00132201, "Scripts";
    IDSTR_201                   = 00132202, "Options";
    IDSTR_202                   = 00132203, "Binds";
    IDSTR_203                   = 00132203, "Help";

    instant FearGui::FGMenuItem ".scripts" {
        position = "0 114";
        extent = "129 19";
        horizSizing = "right";
        vertSizing = "bottom";
        consoleVariable = "";
        consoleCommand = "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0 0 0";
        ghostFillColor = "0.27451 0.556863 0.666667";
        border = "False";
        borderColor = "0 0 0";
        selectBorderColor = "0.772549 0.796078 0.788235";
        ghostBorderColor = "0 0 0";
        visible = "True";
        tag = "";
        active = "True";
        messageTag = "";
        fontNameTag = "IDFNT_FONT_DEFAULT";
        fontNameTagHL = "IDFNT_FONT_DEFAULT";
        fontNameTagDisabled = "IDFNT_FONT_DEFAULT";
        textTag = "IDSTR_200";
        text = "Scripts";
        align = "left";
        textVPosDelta = "0";
        pageTag = "IDCTG_MENU_PAGE_20";
        set = "False";
        expanded = "False";
    };
    instant FearGui::FGControl "OptionsGui::Scripts" {
        position = "133 3";
        extent = "422 402";
        horizSizing = "width";
        vertSizing = "height";
        consoleVariable = "";
        consoleCommand = "";
        altConsoleCommand = "";
        deleteOnLoseContent = "True";
        ownObjects = "True";
        opaque = "False";
        fillColor = "0 0 0";
        selectFillColor = "0.14902 0.07451 0.019608";
        ghostFillColor = "0.14902 0.07451 0.019608";
        border = "True";
        borderColor = "0.172549 0.6 0.070588";
        selectBorderColor = "0.14902 0.07451 0.019608";
        ghostBorderColor = "0.333333 0.333333 0.333333";
        visible = "False";
        tag = "IDCTG_MENU_PAGE_20";
        instant FearGui::FGControl {
            position = "0 0";
            extent = "345 402";
            horizSizing = "center";
            vertSizing = "center";
            consoleVariable = "";
            consoleCommand = "";
            altConsoleCommand = "";
            deleteOnLoseContent = "True";
            ownObjects = "True";
            opaque = "False";
            fillColor = "0 0 0";
            selectFillColor = "0 0 0";
            ghostFillColor = "1 0 0";
            border = "False";
            borderColor = "0 0 0";
            selectBorderColor = "0.772549 0.796078 0.788235";
            ghostBorderColor = "0 0 0";
            visible = "True";
            tag = "IDCTG_GS_PAGE_1";
            instant FearGui::FGStandardComboBox "OptionsGui::ScriptSelect" {
                position = "0 0";
                extent = "345 19";
                horizSizing = "right";
                vertSizing = "bottom";
                consoleVariable = "";
                consoleCommand = "xPrefs::_onSelect();";
                altConsoleCommand = "";
                deleteOnLoseContent = "True";
                ownObjects = "True";
                opaque = "False";
                fillColor = "0 0 0";
                selectFillColor = "0 0 0";
                ghostFillColor = "0 0 0";
                border = "False";
                borderColor = "0 0 0";
                selectBorderColor = "0 0 0";
                ghostBorderColor = "0 0 0";
                visible = "True";
                tag = "";
                active = "True";
                messageTag = "";
                fontNameTag = "IDFNT_10_STANDARD";
                fontNameTagHL = "IDFNT_10_HILITE";
                fontNameTagDisabled = "IDFNT_10_DISABLED";
                textTag = "";
                text = "";
                align = "left";
                textVPosDelta = "0";
                comboTitle = "Script";
            };
            instant FearGui::FGTabMenu "OptionsGui::ScriptOptions" {
                position = "0 25";
                extent = "345 377";
                horizSizing = "right";
                vertSizing = "bottom";
                consoleVariable = "";
                consoleCommand = "";
                altConsoleCommand = "";
                deleteOnLoseContent = "True";
                ownObjects = "True";
                opaque = "False";
                fillColor = "0 0 0";
                selectFillColor = "0.070588 0.807843 0.8";
                ghostFillColor = "0.070588 0.807843 0.8";
                border = "False";
                borderColor = "0 0 0";
                selectBorderColor = "0.070588 0.807843 0.8";
                ghostBorderColor = "0.070588 0.807843 0.8";
                visible = "True";
                tag = "";
                active = "True";
                messageTag = "";
                rowCount = "1";
                instant FearGui::FGTab "OptionsGui::ScriptOptionsTab" {
                    position = "0 0";
                    extent = "82 32";
                    horizSizing = "right";
                    vertSizing = "bottom";
                    consoleVariable = "";
                    consoleCommand = "";
                    altConsoleCommand = "";
                    deleteOnLoseContent = "True";
                    ownObjects = "True";
                    opaque = "False";
                    fillColor = "0 0 0";
                    selectFillColor = "0.070588 0.807843 0.8";
                    ghostFillColor = "0.070588 0.807843 0.8";
                    border = "False";
                    borderColor = "0 0 0";
                    selectBorderColor = "0.070588 0.807843 0.8";
                    ghostBorderColor = "0.070588 0.807843 0.8";
                    visible = "True";
                    tag = "";
                    active = "False";
                    messageTag = "";
                    fontNameTag = "IDFNT_HUD_10_STANDARD";
                    fontNameTagHL = "IDFNT_HUD_10_HILITE";
                    fontNameTagDisabled = "IDFNT_HUD_6_DISABLED";
                    textTag = "IDSTR_201";
                    text = "Options";
                    align = "left";
                    textVPosDelta = "0";
                    tabTag = "IDCTG_MENU_PAGE_21";
                    set = "True";
                };
                instant FearGui::FGTab "OptionsGui::ScriptBindsTab" {
                    position = "82 0";
                    extent = "102 32";
                    horizSizing = "right";
                    vertSizing = "bottom";
                    consoleVariable = "";
                    consoleCommand = "";
                    altConsoleCommand = "";
                    deleteOnLoseContent = "True";
                    ownObjects = "True";
                    opaque = "False";
                    fillColor = "0 0 0";
                    selectFillColor = "0.070588 0.807843 0.8";
                    ghostFillColor = "0.070588 0.807843 0.8";
                    border = "False";
                    borderColor = "0 0 0";
                    selectBorderColor = "0.070588 0.807843 0.8";
                    ghostBorderColor = "0.070588 0.807843 0.8";
                    visible = "True";
                    tag = "";
                    active = "False";
                    messageTag = "";
                    fontNameTag = "IDFNT_HUD_10_STANDARD";
                    fontNameTagHL = "IDFNT_HUD_10_HILITE";
                    fontNameTagDisabled = "IDFNT_HUD_10_DISABLED";
                    textTag = "IDSTR_202";
                    text = "Binds";
                    align = "left";
                    textVPosDelta = "0";
                    tabTag = "IDCTG_MENU_PAGE_22";
                    set = "False";
                };
                instant FearGui::FGTab "OptionsGui::ScriptHelpTab" {
                    position = "114 0";
                    extent = "102 32";
                    horizSizing = "right";
                    vertSizing = "bottom";
                    consoleVariable = "";
                    consoleCommand = "";
                    altConsoleCommand = "";
                    deleteOnLoseContent = "True";
                    ownObjects = "True";
                    opaque = "False";
                    fillColor = "0 0 0";
                    selectFillColor = "0.070588 0.807843 0.8";
                    ghostFillColor = "0.070588 0.807843 0.8";
                    border = "False";
                    borderColor = "0 0 0";
                    selectBorderColor = "0.070588 0.807843 0.8";
                    ghostBorderColor = "0.070588 0.807843 0.8";
                    visible = "True";
                    tag = "";
                    active = "False";
                    messageTag = "";
                    fontNameTag = "IDFNT_HUD_10_STANDARD";
                    fontNameTagHL = "IDFNT_HUD_10_HILITE";
                    fontNameTagDisabled = "IDFNT_HUD_10_DISABLED";
                    textTag = "IDSTR_203";
                    text = "Help";
                    align = "left";
                    textVPosDelta = "0";
                    tabTag = "IDCTG_MENU_PAGE_23";
                    set = "False";
                };
                instant FearGui::FearGuiScrollCtrl {
                    position = "6 33";
                    extent = "335 343";
                    horizSizing = "right";
                    vertSizing = "bottom";
                    consoleVariable = "";
                    consoleCommand = "";
                    altConsoleCommand = "";
                    deleteOnLoseContent = "True";
                    ownObjects = "True";
                    opaque = "False";
                    fillColor = "0 0 0";
                    selectFillColor = "0.070588 0.807843 0.8";
                    ghostFillColor = "0.070588 0.807843 0.8";
                    border = "False";
                    borderColor = "0.172549 0.6 0.070588";
                    selectBorderColor = "0.172549 0.6 0.070588";
                    ghostBorderColor = "0.333333 0.333333 0.333333";
                    visible = "True";
                    tag = "IDCTG_MENU_PAGE_21";
                    pbaTag = "IDPBA_SCROLL2_SHELL";
                    handleArrowKeys = "True";
                    constantHeightThumb = "False";
                    horizontalScrollBar = "dynamic";
                    verticalScrollBar = "dynamic";
                    headerSize = "0 0";
                    borderThickness = "0";
                    instant SimGui::ScrollContentCtrl "ScriptOptionsPanelContainer" {
                        position = "0 0";
                        extent = "345 343";
                        horizSizing = "width";
                        vertSizing = "height";
                        consoleVariable = "";
                        consoleCommand = "";
                        altConsoleCommand = "";
                        deleteOnLoseContent = "True";
                        ownObjects = "True";
                        opaque = "False";
                        fillColor = "0 0 0";
                        selectFillColor = "0.070588 0.807843 0.8";
                        ghostFillColor = "0.070588 0.807843 0.8";
                        border = "False";
                        borderColor = "0 0 0";
                        selectBorderColor = "0.070588 0.807843 0.8";
                        ghostBorderColor = "0.070588 0.807843 0.8";
                        visible = "True";
                        tag = "";
                        instant FearGui::FGControl "OptionsGui::None" {
                            position = "0 0";
                            extent = "316 327";
                            horizSizing = "right";
                            vertSizing = "bottom";
                            consoleVariable = "";
                            consoleCommand = "";
                            altConsoleCommand = "";
                            deleteOnLoseContent = "True";
                            ownObjects = "True";
                            opaque = "False";
                            fillColor = "0 0 0";
                            selectFillColor = "0 0 0";
                            ghostFillColor = "1 0 0";
                            border = "False";
                            borderColor = "0 0 0";
                            selectBorderColor = "0.772549 0.796078 0.788235";
                            ghostBorderColor = "0 0 0";
                            visible = "False";
                            tag = "";
                            instant FearGui::FGSimpleText "OptionsGui::NoneText" {
                                position = "6 6";
                                extent = "345 " ~ $pref::xPrefs::lineHeight;
                                horizSizing = "right";
                                vertSizing = "bottom";
                                consoleVariable = "";
                                consoleCommand = "";
                                altConsoleCommand = "";
                                deleteOnLoseContent = "True";
                                ownObjects = "True";
                                opaque = "False";
                                fillColor = "0 0 0";
                                selectFillColor = "0 0 0";
                                ghostFillColor = "0.819608 0.945098 0.952941";
                                border = "False";
                                borderColor = "0 0 0";
                                selectBorderColor = "0.745098 0.811765 0.870588";
                                ghostBorderColor = "0 0 0";
                                visible = "True";
                                tag = "";
                                active = "True";
                                messageTag = "";
                                fontNameTag = "IDFNT_10_HILITE";
                                fontNameTagHL = "IDFNT_10_SELECTED";
                                fontNameTagDisabled = "IDFNT_10_DISABLED";
                                textTag = "";
                                text = "None";
                                align = "left";
                                textVPosDelta = "0";
                            };
                        };
                    };
                };
                instant FearGui::FearGuiScrollCtrl {
                    position = "6 33";
                    extent = "335 343";
                    horizSizing = "right";
                    vertSizing = "bottom";
                    consoleVariable = "";
                    consoleCommand = "";
                    altConsoleCommand = "";
                    deleteOnLoseContent = "True";
                    ownObjects = "True";
                    opaque = "False";
                    fillColor = "0 0 0";
                    selectFillColor = "0.070588 0.807843 0.8";
                    ghostFillColor = "0.070588 0.807843 0.8";
                    border = "False";
                    borderColor = "0.172549 0.6 0.070588";
                    selectBorderColor = "0.172549 0.6 0.070588";
                    ghostBorderColor = "0.333333 0.333333 0.333333";
                    visible = "False";
                    tag = "IDCTG_MENU_PAGE_22";
                    pbaTag = "IDPBA_SCROLL2_SHELL";
                    handleArrowKeys = "True";
                    constantHeightThumb = "False";
                    horizontalScrollBar = "dynamic";
                    verticalScrollBar = "dynamic";
                    headerSize = "0 23";
                    borderThickness = "0";
                    instant SimGui::ScrollContentCtrl {
                        position = "0 0";
                        extent = "322 327";
                        horizSizing = "right";
                        vertSizing = "bottom";
                        consoleVariable = "";
                        consoleCommand = "";
                        altConsoleCommand = "";
                        deleteOnLoseContent = "True";
                        ownObjects = "True";
                        opaque = "False";
                        fillColor = "0 0 0";
                        selectFillColor = "0.070588 0.807843 0.8";
                        ghostFillColor = "0.070588 0.807843 0.8";
                        border = "False";
                        borderColor = "0 0 0";
                        selectBorderColor = "0.070588 0.807843 0.8";
                        ghostBorderColor = "0.070588 0.807843 0.8";
                        visible = "True";
                        tag = "";
                        instant FearGui::ActionMapList "OptionsGui::scriptsMap" {
                            position = "0 0";
                            extent = "344 344";
                            horizSizing = "right";
                            vertSizing = "bottom";
                            consoleVariable = "";
                            consoleCommand = "";
                            altConsoleCommand = "";
                            deleteOnLoseContent = "True";
                            ownObjects = "True";
                            opaque = "False";
                            fillColor = "0 0 0";
                            selectFillColor = "0.070588 0.807843 0.8";
                            ghostFillColor = "0.070588 0.807843 0.8";
                            border = "False";
                            borderColor = "0 0 0";
                            selectBorderColor = "0.070588 0.807843 0.8";
                            ghostBorderColor = "0.070588 0.807843 0.8";
                            visible = "True";
                            tag = "";
                            active = "True";
                            messageTag = "";
                        };
                    };
                };
                instant FearGui::FearGuiScrollCtrl {
                    position = "6 33";
                    extent = "335 343";
                    horizSizing = "right";
                    vertSizing = "bottom";
                    consoleVariable = "";
                    consoleCommand = "";
                    altConsoleCommand = "";
                    deleteOnLoseContent = "True";
                    ownObjects = "True";
                    opaque = "False";
                    fillColor = "0 0 0";
                    selectFillColor = "0.070588 0.807843 0.8";
                    ghostFillColor = "0.070588 0.807843 0.8";
                    border = "False";
                    borderColor = "0.172549 0.6 0.070588";
                    selectBorderColor = "0.172549 0.6 0.070588";
                    ghostBorderColor = "0.333333 0.333333 0.333333";
                    visible = "False";
                    tag = "IDCTG_MENU_PAGE_23";
                    pbaTag = "IDPBA_SCROLL2_SHELL";
                    handleArrowKeys = "True";
                    constantHeightThumb = "False";
                    horizontalScrollBar = "dynamic";
                    verticalScrollBar = "dynamic";
                    headerSize = "0 0";
                    borderThickness = "0";
                    instant SimGui::ScrollContentCtrl {
                        position = "0 0";
                        extent = "322 327";
                        horizSizing = "right";
                        vertSizing = "bottom";
                        consoleVariable = "";
                        consoleCommand = "";
                        altConsoleCommand = "";
                        deleteOnLoseContent = "True";
                        ownObjects = "True";
                        opaque = "False";
                        fillColor = "0 0 0";
                        selectFillColor = "0.070588 0.807843 0.8";
                        ghostFillColor = "0.070588 0.807843 0.8";
                        border = "False";
                        borderColor = "0 0 0";
                        selectBorderColor = "0.070588 0.807843 0.8";
                        ghostBorderColor = "0.070588 0.807843 0.8";
                        visible = "True";
                        tag = "";
                        instant FearGuiFormattedText "OptionsGui::HelpText" {
                            position = "6 0";
                            extent = "316 327";
                            horizSizing = "width";
                            vertSizing = "height";
                            consoleVariable = "";
                            consoleCommand = "";
                            altConsoleCommand = "";
                            deleteOnLoseContent = "True";
                            ownObjects = "True";
                            opaque = "False";
                            fillColor = "0 0 0";
                            selectFillColor = "0 0 0";
                            ghostFillColor = "0 0 0";
                            border = "True";
                            borderColor = "0 1 0";
                            selectBorderColor = "0 0 0";
                            ghostBorderColor = "0 0 0";
                            visible = "True";
                            tag = "";
                            active = "False";
                            messageTag = "";
                        };
                    };
                };
            };
        };
    };

    ActionMapList::clearBinds("OptionsGui::scriptsMap");

    %general = Control::getId(".general");
    %guiBox = Group::getObject(Group::getObject("OptionsGui", 0), 0); // FearGui::FearGuiBox

    addToSet(%general, ".scripts");
    addToSet(%guiBox, "OptionsGui::Scripts");

    $xPrefs::Init = true;
    xPrefs::Setup(); // Setup the user scripts

	if ($xPrefs::scriptCount > 0) {
		for (%i = 0; %i < $xPrefs::scriptCount; %i++) {
			FGCombo::addEntry("OptionsGui::ScriptSelect", $xPrefs::Script[%i], %i);
		}
	} else {
		FGCombo::addEntry("OptionsGui::ScriptSelect", "None", 0);
	}

	FGCombo::selectNext("OptionsGui::ScriptSelect");

}

function xPrefs::_onSelect() {

	%selectedScript = FGCombo::getSelectedText("OptionsGui::ScriptSelect");

	if (%selectedScript == "None")
		return;

	xPrefs::_loadScript(%selectedScript);

}

function xPrefs::_loadScript(%scriptName) {

	if (%scriptName == "")
		return;

	if ($xPrefs::Script[%scriptName] != true)
		return;

	if (%scriptName == $xPrefs::loadedScript)
		return;

	xPrefs::_unloadScript();

    %bindCount = $xPrefs::Script[%scriptName, bindCount];
    %helpText = $xPrefs::Script[%scriptName, helpText];

    if (%bindCount == 0) {
        Control::setText("OptionsGui::NoneText", "None");
        removeFromSet(Control::getId("OptionsGui::ScriptOptions"), Control::getId("OptionsGui::ScriptBindsTab"));
    } else {
        Control::setText("OptionsGui::NoneText", "See Binds");
        addToSet(Control::getId("OptionsGui::ScriptOptions"), Control::getId("OptionsGui::ScriptBindsTab"));
    }

    removeFromSet(Control::getId("OptionsGui::ScriptOptions"), Control::getId("OptionsGui::ScriptHelpTab")); // Ensure Help tab is last
    if (%helpText != "") {
        addToSet(Control::getId("OptionsGui::ScriptOptions"), Control::getId("OptionsGui::ScriptHelpTab"));
        Control::setValue("OptionsGui::HelpText", %helpText);
        //%helpCtrl = Control::getId("OptionsGui::HelpText");
        //Control::setVisible("OptionsGui::HelpText", false);
        //%helpCtrl.position = "6 0";
        //Control::setVisible("OptionsGui::HelpText", true);
        //%helpPanel = Control::getId(IDCTG_MENU_PAGE_23);
        //%scrollPanel = Group::getObject(%helpPanel, 0);
    }

	if ($xPrefs::Script[%scriptName, objCount] == 0) {
        Control::setVisible("OptionsGui::None", true);
    } else {
        %panelName = $xPrefs::Script[%scriptName, panelName];
        %panel = Control::getId(%panelName);
        %container = Control::getID("ScriptOptionsPanelContainer");
        addToSet(%container, %panel);

        // Force FearGuiScrollCtrl to recalculate scrollbars
        Control::setExtent(%panelName, getWord(%panel.extent, 0), getWord(%panel.extent, 1));
    }

    for (%i = 0; %i < %bindCount; %i++) {
        %desc = $xPrefs::Script[%scriptName, bind, %i, desc];
        %make = $xPrefs::Script[%scriptName, bind, %i, make];
        %break = $xPrefs::Script[%scriptName, bind, %i, break];
        %map = $xPrefs::Script[%scriptName, bind, %i, map];

        %cmd = sprintf( "ActionMapList::addBindableCommand( \"%1\", \"%2\", \"%3\", \"%4\"",
                "OptionsGui::scriptsMap", %map, %desc, String::Escape( %make ) );
        if ( %break != "" )
            %cmd = %cmd @ sprintf( ", \"%1\"", String::Escape( %break ) );
        eval( %cmd @ ");" );
    }

	$xPrefs::loadedScript = %scriptName;

}

function xPrefs::_unloadScript() {

	if ($xPrefs::loadedScript == "")
		return;

    %scriptName = $xPrefs::loadedScript;

    if ($xPrefs::Script[%scriptName, objCount] == 0) {
        Control::setVisible("OptionsGui::None", false);
    } else {
        %panelName = $xPrefs::Script[%scriptName, panelName];
        %panel = Control::getId(%panelName);
        %container = Control::getID("ScriptOptionsPanelContainer");
        removeFromSet(%container, %panel);
    }

    // Reset back to Options tab
    %optionsTab = Control::getId("OptionsGui::ScriptOptionsTab");
    %bindsTab = Control::getId("OptionsGui::ScriptBindsTab");
    %helpTab = Control::getId("OptionsGui::ScriptHelpTab");
    %optionsPanel = Control::getId(IDCTG_MENU_PAGE_21);
    %bindsPanel = Control::getId(IDCTG_MENU_PAGE_22);
    %helpPanel = Control::getId(IDCTG_MENU_PAGE_23);
    %bindsTab.set = false;
    %helpTab.set = false;
    %optionsTab.set = true;
    %bindsPanel.visible = false;
    %helpPanel.visible = false;
    %optionsPanel.visible = true;

	ActionMapList::clearBinds("OptionsGui::scriptsMap");
    Control::setValue("OptionsGui::HelpText", "");

	$xPrefs::loadedScript = "";

}

function xPrefs::_addObjectToPanel(%scriptName, %objName) {

    if ($xPrefs::Script[%scriptName] != true)
		return;

    %objId = Control::getId(%objName);

    if (%objId == -1)
		return;

    %panelName = $xPrefs::Script[%scriptName, panelName];
    %panel = Control::getID(%panelName);
    addToSet(%panel, %objId);

    $xPrefs::Script[%scriptName, objCount]++;

    %lineOffset = $xPrefs::Script[%scriptName, lineOffset];
    if (%lineOffset > 327) {
        Control::setExtent(%panelName, getWord(%panel.extent, 0), %lineOffset);
    }
}

function xPrefs:_lineOffset(%scriptName, %preInc, %postInc) {

    if ($xPrefs::Script[%scriptName] != true)
		return -1;

    if (%preInc == "")
        %preInc = 0;

    if (%postInc == "")
        %postInc = 0;

	%offset = $xPrefs::Script[%scriptName, lineOffset] + %preInc;
	$xPrefs::Script[%scriptName, lineOffset] += ($pref::xPrefs::lineHeight + %preInc + %postInc);

    return %offset;

}

// =====================================================================================================
// Preferences/Variables
// =====================================================================================================

$xPrefs::Installed = true;

$pref::xPrefs::defaultLineHeight = $pref::xPrefs::defaultLineHeight == "" ? True : $pref::xPrefs::defaultLineHeight;
$pref::xPrefs::lineHeight = $pref::xPrefs::lineHeight == "" ? 17 : $pref::xPrefs::lineHeight;
$pref::xPrefs::Debug = $pref::xPrefs::Debug == "" ? False : $pref::xPrefs::Debug;

if ($pref::xPrefs::defaultLineHeight)
    $pref::xPrefs::lineHeight = 17;

function xPrefs::xSetup() after xPrefs::Setup {
    xPrefs::Create("xPrefs", "xPrefs::xInit");
}

function xPrefs::xInit() {
    xPrefs::addText("xPrefs::Header1", "xPrefs Script Preference System", "True", 40, "", 0);
    xPrefs::addText("xPrefs::Header2", "by Smokey", "True", 120, 0, 0);
    xPrefs::addText("xPrefs::Header3", "Version v" ~ $xPrefs::Version, "False", 110, 0);

    xPrefs::addText("xPrefs::Header4", "Preferences");
    xPrefs::addCheckbox("xPrefs::Checkbox1", "Default Font Height", "$pref::xPrefs::defaultLineHeight", "xPrefs::Deactive(xPrefs::Height, xPrefs::Checkbox);");
    xPrefs::addTextEdit("xPrefs::Height", "Font Height", "$pref::xPrefs::lineHeight", "True", "10", !$pref::xPrefs::defaultLineHeight);
    xPrefs::addText("xPrefs::Note1", "Update if using custom font sizes.", "False", 18);
    xPrefs::addCheckbox("xPrefs::Checkbox2", "Debug Mode", "$pref::xPrefs::Debug");
    xPrefs::addText("xPrefs::Note2", "Restart Tribes to take effect.", "False", 18, 10);
}

// =====================================================================================================
// Debug Examples
// =====================================================================================================

$Debug1::Pref::Checkbox1 = True;
$Debug1::Pref::Checkbox2 = False;
$Debug1::Pref::Checkbox3 = True;
$Debug1::Pref::TextEdit1 = "Input1";
$Debug1::Pref::TextEdit2 = "3.14159";
$Debug1::Pref::Checkbox4 = False;
$Debug1::Pref::TextEdit3 = "Tribes";
$Debug1::Pref::Combo1 = "Option 2";
$Debug1::Pref::Combo2 = "Item 3";
$Debug1::Array[0] = "Item 1";
$Debug1::Array[1] = "Item 2";
$Debug1::Array[2] = "Item 3";

function Debug::xSetup() after xPrefs::Setup {
    if (!$pref::xPrefs::Debug)
        return;

    xPrefs::Create("Debug1", "Debug1::xInit");
    xPrefs::Create("Debug2", "Debug2::xInit");
    xPrefs::Create("Debug3", "Debug3::xInit");
    xPrefs::Create("Debug4", "Debug4::xInit");
}

function Debug1::xInit() {
    xPrefs::addText("Debug1::Header_Checkboxes", "Checkboxes");
    xPrefs::addCheckbox("Debug1::Checkbox1", "Checkbox 1", "$Debug1::Pref::Checkbox1");
    xPrefs::addCheckbox("Debug1::Checkbox2", "Checkbox 2", "$Debug1::Pref::Checkbox2", "echoc(1, \"Checkbox2 Clicked!\");");
    xPrefs::addCheckbox("Debug1::Checkbox3", "Checkbox 3", "$Debug1::Pref::Checkbox3", "echoc(1, \"Checkbox3 Set!\");", "echoc(1, \"Checkbox3 Unset!\");");

    xPrefs::addText("Debug1::Header_TextEdits", "Text Edits");
    xPrefs::addTextEdit("Debug1::TextEdit1", "Text Edit", "$Debug1::Pref::TextEdit1");
    xPrefs::addTextEdit("Debug1::TextEdit2", "Numeric Text Edit", "$Debug1::Pref::TextEdit2", "True", "10");

    xPrefs::addText("Debug1::Header_Conditional", "Conditional Text Input");
    xPrefs::addCheckbox("Debug1::Checkbox4", "Check Box", "$Debug1::Pref::Checkbox4", "xPrefs::Active(Debug1::TextEdit3, Debug1::Checkbox4);");
    xPrefs::addTextEdit("Debug1::TextEdit3", "Text Edit", "$Debug1::Pref::TextEdit3", "False", "255", $Debug1::Pref::Checkbox4);

    xPrefs::addText("Debug1::Header_Combo1", "Combo Box 1");
    xPrefs::addComboBox("Debug1::Combo1", "Combo 1", "$Debug1::Pref::Combo1", "Debug1::Combo1::onAction();");
    FGCombo::addEntry("Debug1::Combo1", "Option 1", 0);
    FGCombo::addEntry("Debug1::Combo1", "Option 2", 1);
    FGCombo::addEntry("Debug1::Combo1", "Option 3", 2);
    FGCombo::setSelected("Debug1::Combo1", FGCombo::FindEntry("Debug1::Combo1", $Debug1::Pref::Combo1));

    xPrefs::addText("Debug1::Header_Combo2", "Combo Box 2");
    xPrefs::addComboBox("Debug1::Combo2", "Combo 2", "$Debug1::Pref::Combo2", "Debug1::Combo2::onAction();", true, "$Debug1::Array");

    xPrefs::addText("Debug1::Header_More", "More Controls");
    xPrefs::addCheckbox("Debug1::Checkbox5", "Checkbox 5", "$Debug1::Pref::Checkbox5");
    xPrefs::addCheckbox("Debug1::Checkbox6", "Checkbox 6", "$Debug1::Pref::Checkbox6");
    xPrefs::addCheckbox("Debug1::Checkbox7", "Checkbox 7", "$Debug1::Pref::Checkbox7");
    xPrefs::addCheckbox("Debug1::Checkbox8", "Checkbox 8", "$Debug1::Pref::Checkbox8");

    xPrefs::addBindCommand("actionMap.sae", "Bind Command 1", "Debug1::Test1();");
    xPrefs::addBindCommand("actionMap.sae", "Bind Command 2", "Debug1::Test2();");
    xPrefs::addBindCommand("playMap.sae", "Bind Command 3", "Debug1::Test3::In();", "Debug1::Test3::Out();");

    xPrefs::addHelpSection("Setting 1", "Setting 1 is for enabling or disabling the script.");
    xPrefs::addHelpSection("Setting 2", "Setting 2 is for changing the mounted weapon.");

}

function Debug2::xInit() {
    return;
}

function Debug3::xInit() {
    xPrefs::addBindCommand("actionMap.sae", "Bind Command 1", "Debug3::Command();");
}

function Debug4::xInit() {
    xPrefs::addText("Debug4::Header", "Formatted Text");
    xPrefs::AddTextFormat("Debug4::Text1", "<jc><f2>Formatted <f1>Text");
    xPrefs::AddTextFormat("Debug4::Text2", "<jc><f2>Formatted <f1>Text", 18, True, "", 10);
}

function Debug1::Combo1::onAction() {
    echoc(3, "Debug1::Combo1::onAction");
}

function Debug1::Combo2::onAction() {
    echoc(3, "Debug1::Combo2::onAction");
}
