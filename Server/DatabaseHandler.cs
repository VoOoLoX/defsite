namespace Server {
	public class DatabaseHandler : Handler {
		public DatabaseHandler() {
		}

		protected override int TPS => 1000;

		protected override void FixedUpdate(long dt) {
			//Console.WriteLine("DatabaseHandler update");
		}
	}
}