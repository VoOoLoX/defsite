namespace Server;

public enum MessageType : byte {
	Unknown = 0x00,

	//Incoming
	Init = 0x41,
	Login,
	Broadcast,

	//Outgoing
	Text
}
