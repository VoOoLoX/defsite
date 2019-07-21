namespace Server {
	public enum MessageType : byte {
		Unknown = 0x00,
		Init = 0x41,
		Login,
		Broadcast,
		Text
	}
}