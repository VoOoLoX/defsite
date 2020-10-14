namespace Server {
	public class DatabaseHandler : Handler {
		public DatabaseHandler() {
		}

		protected override int TPS => 1000;

		protected override void FixedUpdate(object state) {
			//Console.WriteLine("DatabaseHandler update");
		}
	}
}