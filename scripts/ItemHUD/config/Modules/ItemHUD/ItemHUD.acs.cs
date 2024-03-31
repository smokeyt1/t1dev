function ItemHUD::Init() {
	if ($ItemHUD::Loaded)
		return;
	$ItemHUD::Loaded = true;

	$ItemHUD::Awake = false;
	$ItemHUD::Grenades = 0;
	$ItemHUD::Kits = 0;
	
	HUD::New( "ItemHUD::Container", 0, 0, 120, 50, ItemHUD::Wake, ItemHUD::Sleep );
	newObject( "ItemHUD::Text", FearGuiFormattedText, 0, 0, 120, 50 );
	HUD::Add( "ItemHUD::Container", "ItemHUD::Text" );
}

function ItemHUD::Wake() {
	$ItemHUD::Awake = true;
	ItemHUD::Update();
}

function ItemHUD::Sleep() {
	Schedule::Cancel("ItemHUD::Update();");
	$ItemHUD::Awake = false;
}

function ItemHUD::Update() {
	if ( !$ItemHUD::Awake )
		return;

	if (!$xEvent::Loaded)
		Schedule::Add("ItemHUD::Update();", 1); // Use Schedule if xEvent.dll is not loaded
	
	%text = "";
	%kits = getItemCount("Repair Kit");
	%Grenades = getItemCount("Grenade");

	//dont bother updating if count hasnt changed
	if ( ( %kits == $ItemHUD::Kits ) && ( %Grenades == $ItemHUD::Grenades ) )
		return;

	$ItemHUD::Kits = %kits;
	$ItemHUD::Grenades = %Grenades;
 
	%kits = ( %kits > 0 ) ? "kitdot.png" : "blankdot.png";

	%text = "<B0,0:Modules/ItemHUD/" @ %kits @ ">";
	for ( %i = 0; %i < %Grenades; %i++ )
		%text = %text @ "<B0,0:Modules\\ItemHUD\\grendot.png>";
	
	Control::SetValue( "ItemHUD::Text", %text );
}

ItemHUD::Init();

function ItemHUD::ItemUpdate(%item, %delta) {
    %desc = getItemDesc(%item);

    switch (%desc) {
        case "Repair Kit":
        case "Grenade":
            ItemHUD::Update();
    }
}

Event::Attach(eventItemCountUpdate, ItemHUD::ItemUpdate); // Requires xEvent.dll
