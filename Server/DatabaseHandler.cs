using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public class DatabaseHandler : Handler {
		public override int TPS => 1000;
		public DatabaseHandler() {
		}

		public override void FixedUpdate(long dt) {
			//Console.WriteLine("DatabaseHandler update");
		}
	}
}
