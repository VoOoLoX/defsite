using System;

using Common;

using Raven.Client.Documents;
using Raven.Client.Documents.Session;

using Server.Database;

namespace Server {
	public class DatabaseHandler : Handler {
		protected override int TicksPerSecond => 1;

		protected override void Update() {
			base.Update();
			var p = new Player() {
				Name = Guid.NewGuid().ToString("n").Substring(0, 8)
			};
			Log.Info($"{p.GUID} - {p.Name}");
			session.Store(p, p.GUID.ToString());
		}

		IDocumentSession session = DocumentStoreHolder.Store.OpenSession();
		protected override void FixedUpdate(object state) {
			session.SaveChanges();
		}
	}
}