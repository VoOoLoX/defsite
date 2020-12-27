namespace Server {
	public class DatabaseHandler : Handler {
		protected override int TPS => 1000;

		protected override void FixedUpdate(object state) {
			//Console.WriteLine("DatabaseHandler update");
		}
	}
}