// Vote sounds based on T2 sounds
// By Smokey
// v0.1

function voteprint(%manager, %msg, %timeout, %type) before remoteBP {
	if (String::FindSubStr(%msg, "<f0>initiated a vote to <f1>") != -1) {
		localSound("vote_initiated.wav");
	}
}

function votechat(%cl, %msg, %type) before onClientMessage {
	if (%type != 0)
		return;

	if (String::FindSubStr(%msg, "Vote to ") != -1) {
		if (String::FindSubStr(%msg, " passed: ") != -1) {
			localSound("vote_passes.wav");
		} else if (String::FindSubStr(%msg, " did not pass: ") != -1) {
			localSound("vote_fails.wav");
		}
	}
}
