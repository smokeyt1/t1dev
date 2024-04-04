// Autokit
// Supports xEvent.dll
// By Smokey
// v0.1

function Auto::Health(%health) {
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

if ($xEvent::Loaded) {
	Event::Attach(eventPlayerDamage, Auto::Health); // Requires xEvent.dll
} else {
	Event::Attach(eventConnected, Auto::Health); // Run with Schedule
}
