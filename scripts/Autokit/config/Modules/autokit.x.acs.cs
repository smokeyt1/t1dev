// Autokit
// Supports xEvent.dll
// By Smokey
// v0.3

function Auto::Health(%health, %prevHealth) {
	if ($Station::Type == "Inventory" || $Station::Type == "RemoteInventory") {
		Schedule::Cancel("Auto::Health();");
		Schedule::Add("Auto::Health();", 9);
		return;
	}

	%kits = getItemCount("Repair Kit");

	if (%health == "") {
		%health = $Health;
	}

	if (%kits > 0 && %health < 65 && %health > 0)
		use("Repair Kit");

	if (!$xEvent::Loaded)
		Schedule::Add("Auto::Health();", 0.1);
}

function Auto::ItemUpdate(%item, %delta) {
    %desc = getItemDesc(%item);

    if (%desc == "Repair Kit")
		Auto::Health();

}

Event::Attach(eventConnected, Auto::Health);
Event::Attach(eventHealthUpdate, Auto::Health); // Requires xEvent.dll
Event::Attach(eventItemCountUpdate, Auto::ItemUpdate); // Requires xEvent.dll
